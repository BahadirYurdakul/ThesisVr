using System;
using System.Collections;
using System.Reflection;
using UnityEngine;

public class FaceDetector : MonoBehaviour {
    // Use this for initialization
    void Start() {
        Debug.Log("starting detection");
        StartCoroutine(Detect());
    }

    IEnumerator Detect() {
        var asm = Assembly.LoadFile("/mnt/sdcard/plg/FaceRecognizerPlugin.dll");
        var type = asm.GetType("FaceRecognizerPlugin.Class1");
        var plugin = Activator.CreateInstance(type) as IPlugin;
        if (plugin == null) throw new Exception("fucking plugin is null");
        Debug.Log("plugin loaded!");
        while (true) {
            yield return new WaitForEndOfFrame();

            // create a texture to pass to encoding
            Texture2D texture = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);

            // put buffer into texture
            texture.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
            texture.Apply();
            byte[] bytes = texture.EncodeToPNG();
            Debug.Log(plugin.Start(texture.height, texture.width, bytes));
        }
    }

    // Update is called once per frame
    void Update() { }
}