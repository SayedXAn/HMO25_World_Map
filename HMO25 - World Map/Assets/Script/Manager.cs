using UnityEngine;
using System.IO;
using UnityEngine.Video;
using DG.Tweening;
using UnityEngine.UI;
using System.Collections;

public class Manager : MonoBehaviour
{
    public VideoPlayer content;
    public RenderTexture rt;
    public string[] videoURLs;
    public GameObject closeButton;
    void Start()
    {
        GetVideosFromLocal();
        DOTween.Init();
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
}
