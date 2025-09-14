using UnityEngine;
using UnityEngine.UI;
using System.IO;
using UnityEngine.Video;

public class Manager : MonoBehaviour
{
    public VideoPlayer content;
    public RenderTexture rt;
    public string[] videoURLs;
    void Start()
    {
        GetVideosFromLocal();
    }

    public void LocationButton(int buttonID)
    { 
        content.url = videoURLs[buttonID];
        rt.Release();
        content.gameObject.SetActive(true);
        content.Play();
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
