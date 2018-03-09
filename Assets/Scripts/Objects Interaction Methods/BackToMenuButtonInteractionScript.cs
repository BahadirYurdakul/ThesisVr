using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackToMenuButtonInteractionScript : MonoBehaviour {

    public GameObject VrMain;
    public float lookTime = 3f;
    private bool isLooking = false;
    private float timer = 0f;
    public GameObject MagnetButton;

    void OnDisable()
    {
        Debug.Log("Disabled");
        resetTheGaze();
    }

    void OnEnable()
    {
        Debug.Log("Enabled");
        resetTheGaze();
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
        VrMain.GetComponent<ChangeSceneScript>().changeSceneToRoom();
        Debug.Log("CursorClick");
    }

    private void resetTheGaze()
    {
        timer = 0f;
        isLooking = false;
    }
}
