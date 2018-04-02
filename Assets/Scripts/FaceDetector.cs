using System;
using System.Collections;
using UnityEngine;

public class FaceDetector : MonoBehaviour {
    /*AndroidJavaClass jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer"); 
AndroidJavaObject jo = jc.GetStatic<AndroidJavaObject>("currentActivity");*/

    public Camera cam;
    private Texture2D texture;
    private int framec = 0;
    private AndroidJavaObject context;
    private AndroidJavaClass frameDist;
    private RenderTexture rt;

    // Use this for initialization
    void Start() {
        rt = new RenderTexture(Screen.width, Screen.height, 24);
        cam.targetTexture = rt;
        texture = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);
        Debug.Log("starting detection");

        using (var jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer")) {
            context = jc.GetStatic<AndroidJavaObject>("currentActivity");
            frameDist = new AndroidJavaClass("tr.edu.iyte.vrxd.unityhook.FrameDistributer");
            frameDist.CallStatic("loadPlugins", context);
        }
    }

    // Update is called once per frame
    void Update() {
        framec++;
        if (framec % 20 == 0)
            StartCoroutine(Detect());
    }

    IEnumerator Detect() {
        var sw = new System.Diagnostics.Stopwatch();
        sw.Start();
        yield return new WaitForEndOfFrame();

        cam.Render();
        var temp = RenderTexture.active;
        RenderTexture.active = rt;
        // put buffer into texture
        texture.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);

        RenderTexture.active = temp;
        yield return null;

        texture.Apply();

        yield return null;

        frameDist.CallStatic("distribute", texture.EncodeToPNG());
        Debug.Log("elapsed: " + sw.ElapsedMilliseconds + "ms");
    }
}