using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SwitchTo360Video : MonoBehaviour
{
    public GameObject panoVideo;
    public GameObject VrMain;

    public void Awake()
    {
        if (panoVideo == null)
        {
            Debug.LogError("Video should be pano video...");
            return;
        }

        try
        {
            IntPtr ptr = GvrVideoPlayerTexture.CreateVideoPlayer();
            if (ptr != IntPtr.Zero)
            {
                GvrVideoPlayerTexture.DestroyVideoPlayer(ptr);
            }
        }
        catch (Exception e)
        {
            Debug.LogError(e);
        }
    }

    public void OnVideoClick(string videoPath)
    {
        if (string.IsNullOrEmpty(videoPath))
            return;
        Debug.Log("Video clicked and path: " + videoPath);
        ShowVideo(videoPath);
    }

    private void ShowVideo(string videoPath)
    {
        if (panoVideo == null || videoPath == null)
        {
            Debug.Log("Pano video null");
            return;
        }

        VrMain.GetComponent<ChangeSceneScript>().changeSceneToVideoPlayer();
        //First set the video url, then reinitialize video.
        panoVideo.GetComponentInChildren<GvrVideoPlayerTexture>().VideoPath = videoPath;
        panoVideo.GetComponentInChildren<GvrVideoPlayerTexture>().ReInitializeVideo();
    }
}
