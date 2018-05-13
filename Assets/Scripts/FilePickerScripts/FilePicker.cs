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
        if(videoListMenu == null)
            videoListMenu = GameObject.FindGameObjectWithTag("VideoListMenu");

        try {
            var files = getFiles(videosPath);
            foreach (FileInfo file in files)
                addFileToMenu(file);
        } catch(Exception ex) {
            Debug.LogError(ex.Message);
        }
    }

    private void addFileToMenu(FileInfo file)
    {
        if (!new SupportedVideoExtensions().isVideoFormatSupported(file.Extension))
            return;
        var newButton = AddRemoveParentHelper.Instance.InstantiatePrefab("Video360MenuItem", file.Name, videoListMenu);
        //var newButton = AddRemoveParentHelper.Instance.CloneObject(videoButton, videoListMenu, file.Name);
        VideoInteractionScript buttonScript = newButton.GetComponent<VideoInteractionScript>();
        buttonScript.videoPath = "file:///" + file.FullName;
        Text[] buttonText = newButton.gameObject.GetComponentsInChildren<Text>();
        buttonText[0].text = file.Name.Replace(file.Extension, "");
    }

    private FileInfo[] getFiles(string path)
    {
        var info = new DirectoryInfo(videosPath);
        if (info == null)
            throw new FileNotFoundException("Cannot found video path directory");
        var fileInfo = info.GetFiles();
        if (fileInfo == null)
            throw new FileNotFoundException("Cannot found video path directory");
        return fileInfo;
    }
}
