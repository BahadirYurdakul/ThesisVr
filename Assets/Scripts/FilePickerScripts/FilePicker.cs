using UnityEngine;
using System.IO;
using System;
using UnityEngine.UI;
using CardboardVr;

public class FilePicker : MonoBehaviour {

    public string videosPath;
    public GameObject videoListMenu;
    public Component videoButton;

    void Start () {
        try
        {
            var info = new DirectoryInfo(videosPath);
            if (info == null)
                throw new FileNotFoundException("Cannot found video path directory");
            var fileInfo = info.GetFiles();

            if (fileInfo == null)
                return;

            foreach (FileInfo file in fileInfo)
            {
                if (!new SupportedVideoExtensions().isVideoFormatSupported(file.Extension))
                    continue;
                var newButton = CloneObject(videoButton, videoListMenu, file.Name);
                VideoInteractionScript buttonScript = newButton.GetComponent<VideoInteractionScript>();
                buttonScript.videoPath = "file:///" + file.FullName;
                Text[] buttonText = newButton.gameObject.GetComponentsInChildren<Text>();
                buttonText[0].text = file.Name.Replace(file.Extension, "");
            }
        } catch(Exception ex)
        {
            Debug.LogError(ex.Message);
        }
    }

    public void SetParentObject(Component childObject,GameObject parentObject)
    {
        childObject.transform.parent = parentObject.transform;
    }

    public Component CloneObject(Component objectToClone, GameObject parentObject, string newObjectName)
    {
        Component cp = Instantiate(objectToClone, parentObject.transform.position, parentObject.transform.rotation);
        cp.name = newObjectName;
        SetParentObject(cp, parentObject);
        cp.transform.localScale = objectToClone.gameObject.transform.localScale;
        cp.gameObject.SetActive(true);
        return cp;
    }
}
