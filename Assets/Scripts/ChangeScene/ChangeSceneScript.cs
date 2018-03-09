using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeSceneScript : MonoBehaviour {

    public List<GameObject> RoomObjects;
    public List<GameObject> VideoPlayerObjects;

    public void changeSceneToVideoPlayer()
    {
        if (RoomObjects != null)
            showRoomObjects(false);

        if (VideoPlayerObjects != null)
            showVideoPlayerObjects(true);
    }

    public void changeSceneToRoom()
    {
        if (VideoPlayerObjects != null)
            showVideoPlayerObjects(false);

        if (RoomObjects != null)
            showRoomObjects(true);
    }

    private void showVideoPlayerObjects(bool enable)
    {
        foreach (GameObject obj in VideoPlayerObjects)
            obj.SetActive(enable);
    }

    private void showRoomObjects(bool enable)
    {
        foreach (GameObject obj in RoomObjects)
            obj.SetActive(enable);
    }
}
