using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VideoInteractionScript : MonoBehaviour {

    public GameObject MagnetButton;
    public GameObject VideoListMenu;
    public string videoPath;

    public float lookTime = 3f;
    private bool isLooking = false;
    private float timer = 0f;
    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (isLooking)
        {
            if (MagnetButton.GetComponent<MagnetSensor>().isPressedMagnetButton)
                CursorClick();
            timer += Time.deltaTime;
            if (timer >= lookTime)
            {
                CursorClick();
            }
        }
    }

    public void CursorEnter()
    {
        timer = 0f;
        isLooking = true;
        Debug.Log("CursorEnter");
    }

    public void CursorExit()
    {
        resetTheGaze();
        Debug.Log("CursorExit");
    }

    public void CursorClick()
    {
        resetTheGaze();
        VideoListMenu.GetComponent<SwitchTo360Video>().OnVideoClick(videoPath);
        Debug.Log("CursorClick");
    }

    private void resetTheGaze()
    {
        timer = 0f;
        isLooking = false;
    }
}
