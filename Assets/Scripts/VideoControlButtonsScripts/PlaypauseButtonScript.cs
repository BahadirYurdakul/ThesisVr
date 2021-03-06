﻿using GoogleVR.VideoDemo;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaypauseButtonScript : MonoBehaviour {
    public float lookTime = 3f;
    private bool isLooking = false;
    private float timer = 0f;
    public GameObject MagnetButton;
    public GameObject frameDistributer;
    public Canvas VideoDisplayCanvas;

    void OnDisable() {
        resetTheGaze();
    }

    void OnEnable() {
        resetTheGaze();
    }

    // Update is called once per frame
    void Update() {
        if (MagnetButton.GetComponent<MagnetSensor>().isPressedMagnetButton) {
            CursorClick();
            return;
        }

        if (isLooking) {
            timer += Time.deltaTime;
            if (timer >= lookTime) {
                CursorClick();
            }
        }
    }

    public void CursorEnter() {
        timer = 0f;
        isLooking = true;
        Debug.Log("CursorEnter");
    }

    public void CursorExit() {
        resetTheGaze();
        Debug.Log("CursorExit");
    }

    public void CursorClick() {
        timer = 0f;
        VideoDisplayCanvas.GetComponent<VideoControlsManager>().OnPlayPause();
        frameDistributer.SetActive(!frameDistributer.activeSelf);
        Debug.Log("CursorClick");
    }

    private void resetTheGaze() {
        timer = 0f;
        isLooking = false;
    }
}