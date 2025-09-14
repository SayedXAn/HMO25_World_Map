using UnityEngine;
using UnityEngine.UI;

public class Manager : MonoBehaviour
{
    public Image content;
    public Sprite[] contentImages;
    void Start()
    {
        
    }

    void Update()
    {
        
    }

    public void LocationButton(int buttonID)
    { 
        content.sprite = contentImages[buttonID];
        content.gameObject.SetActive(true);
    }
}
