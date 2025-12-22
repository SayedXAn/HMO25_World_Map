using UnityEngine;
using System.IO;
using UnityEngine.Video;
using DG.Tweening;
using UnityEngine.UI;
using System.Collections;
using TMPro;
using System.Text;
using UnityEngine.Networking;

public class Manager : MonoBehaviour
{
    public VideoPlayer content;
    public RenderTexture rt;
    public string[] videoURLs;
    public GameObject closeButton;
    public GameObject rfidPanel;
    public TMP_InputField rfid_IF;
    private string rfid;
    private const string url = "https://rfid-scan.wskoly.xyz/api/game/score";
    public int gameID = 5;
    public int score = 0;
    public TMP_Text statusText;
    public TMP_Text scoreText;
    public int[] buttonIds = {0, 0, 0, 0, 0, 0};
    public Button[] buttons;
    
    void Start()
    {
        GetVideosFromLocal();
        DOTween.Init();
        rfidPanel.SetActive(true);
        rfid_IF.ActivateInputField();
        SetButtonOnOff(false);
    }

    public void LocationButton(int buttonID)
    { 
        
        content.url = videoURLs[buttonID];
        rt.Release();
        content.gameObject.SetActive(true);        
        content.GetComponent<RawImage>().DOFade(1f, 0.5f);
        closeButton.SetActive(true);
        closeButton.GetComponent<Image>().DOFade(0.5f, 0.5f);
        content.Play();
        StartCoroutine(isVideoPlaying());
    }

    IEnumerator isVideoPlaying()
    {
        yield return new WaitForSeconds(0.5f);
        if(content.isPlaying)
        {
            StartCoroutine(isVideoPlaying());
        }
        else
        {
            StopCoroutine(isVideoPlaying());
        }
    }

    public void VideoEnded()
    {
        
        content.GetComponent<RawImage>().DOFade(0f, 0.5f);      
        closeButton.GetComponent<Image>().DOFade(0f, 0.5f);      
        StartCoroutine(TurnOnOffContent());
    }
    
    IEnumerator TurnOnOffContent()
    {
        yield return new WaitForSeconds(0.6f);
        content.gameObject.SetActive(false);
        closeButton.SetActive(false);
        rt.Release();
    }
    public void GetVideosFromLocal()
    {               
        for (int i = 0; i < videoURLs.Length; i++)
        {
            string avDir = $"{Application.dataPath}/Videos/{i + 1}.mp4";
            if (File.Exists(avDir))
            {
                videoURLs[i] = avDir;
                Debug.Log("Video "+ (i+1) + " loaded");
            }
        }
    }

    public void SubmitButton()
    {
        if(rfid_IF.text.Length == 10)
        {
            rfid = rfid_IF.text;
            rfidPanel.SetActive(false);
            statusText.text = "";
            SetButtonOnOff(true);
        }
        else
        {
            StartCoroutine(DebugTextOnOff());
            rfid_IF.ActivateInputField();
        }
    }
    IEnumerator DebugTextOnOff()
    {
        statusText.text = "Invalid RFID";
        yield return new WaitForSeconds(2f);
        statusText.text = "";
    }

    public void ResetPanel()
    {
        score = score * 5;
        SendScore();
        score = 0;
        buttonIds = new int[] { 0, 0, 0, 0, 0, 0 };
        rfid = "";
        rfidPanel.SetActive(true);
        rt.Release();
        content.Stop();
        rfid_IF.text = "";
        rfid_IF.ActivateInputField();
        SetButtonOnOff(false);
    }

    public void CalculateScore(int id)
    {
        if (buttonIds[id] == 0)
        {
            buttonIds[id] = 1;
            score++;
        }
    }

    public void SendScore()
    {
        StartCoroutine(PostScore(rfid_IF.text, gameID, score));
    }

    IEnumerator PostScore(string rfid, int gID, int score)
    {
        var gameID = gID + 1;
        // Build JSON manually

        string jsonBody = $"{{\"rfid\": \"{rfid}\", \"scores\": {{\"{gameID}\": {score}}}}}";

        Debug.Log("Sending: " + jsonBody);
        statusText.text = "Posting score...";

        UnityWebRequest request = new UnityWebRequest(url, "POST");
        byte[] jsonToSend = Encoding.UTF8.GetBytes(jsonBody);
        request.uploadHandler = new UploadHandlerRaw(jsonToSend);
        request.downloadHandler = new DownloadHandlerBuffer();

        request.SetRequestHeader("Content-Type", "application/json");
        //request.SetRequestHeader("x-token", token);

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("POST Success: " + request.downloadHandler.text);
            statusText.text = "Score updated successfully!";
            statusText.color = Color.green;
            // Parse JSON response
            UpdateScoreResponse response = JsonUtility.FromJson<UpdateScoreResponse>(request.downloadHandler.text);
            scoreText.text = $"Your total score: {response.total_points}";
            StartCoroutine(ResetScoreText());
        }
        else
        {
            Debug.LogError("POST Failed: " + request.error + "\nResponse: " + request.downloadHandler.text);

            statusText.text = "Failed to update score!";
            statusText.color = Color.red;
        }

        // Optional: fade out after 3 seconds
        yield return new WaitForSeconds(3);
        statusText.text = "";
    }

    public void SetButtonOnOff(bool beOn)
    {
        for (int i = 0; i < buttons.Length; i++)
        {
            buttons[i].interactable = beOn;
        }
    }

    IEnumerator ResetScoreText()
    {
        yield return new WaitForSeconds(5f);
        scoreText.text = "";
    }
}

[System.Serializable]
public class ScoreResponse
{
    public string rfid;
    public int game_id;
    public int score;
    public string user_name;
    public int total_points;
}

[System.Serializable]
public class UpdateScoreResponse
{
    public string rfid;
    public string user_name;
    public string updated_games;
    public int total_points;
    public string message;
}
