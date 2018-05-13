using System;
using System.Collections;
using System.Runtime.InteropServices;
using UnityEngine;

public class FaceDetector : MonoBehaviour {
    public Camera cam;
    private int framec = 0;
    private AndroidJavaObject context;
    private AndroidJavaClass frameDist;
    private RenderTexture rt;
    private Rect rect;
    private Texture2D texture;
    private bool anyOpenCvPlugins;

    private int w;
    private int h;

    private const int ResMult = 4;

    // Use this for initialization
    void Start() {
        w = Screen.width / ResMult;
        h = Screen.height / ResMult;
        rt = new RenderTexture(w, h, 24);
        rect = new Rect(0, 0, w, h);
        texture = new Texture2D(w, h, TextureFormat.RGB24, false);
        cam.targetTexture = rt;
        Debug.Log("starting detection");

        using (var jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer")) {
            context = jc.GetStatic<AndroidJavaObject>("currentActivity");
            frameDist = new AndroidJavaClass("tr.edu.iyte.vrxd.unityhook.FrameDistributer");
            anyOpenCvPlugins = frameDist.CallStatic<bool>("loadPlugins", context);
        }
    }

    // Update is called once per frame
    void Update() {
        framec++;

        int count = frameDist.CallStatic<int>("getFrameObj", framec / 30);
        if (count != -1) {
            for (int i = 0; i < count; i++) {
                
            }
        }

        if (framec % 30 == 0)
            // TODO record current camera angle and frameId
            StartCoroutine(Detect());

        // Camera.main.transform.rotation
        // TODO ask for frame ready then collect and draw if any shapes
    }

    IEnumerator Detect() {
        var sw = new System.Diagnostics.Stopwatch();
        sw.Start();
        yield return new WaitForEndOfFrame();

        cam.Render();

        var ms = sw.ElapsedMilliseconds;

        var temp = RenderTexture.active;
        RenderTexture.active = rt;

        var ms2 = sw.ElapsedMilliseconds;

        // put buffer into texture
        texture.ReadPixels(rect, 0, 0);

        var ms3 = sw.ElapsedMilliseconds;

        RenderTexture.active = temp;
        yield return null;

        texture.Apply();

        var ms4 = sw.ElapsedMilliseconds;

        yield return null;

        using (var mat = new AndroidJavaObject("org.opencv.core.MatOfByte", texture.EncodeToJPG(70))) {
            frameDist.CallStatic("distribute", framec / 30, mat);
        }

        yield return null;
        Debug.Log("\nelapsed render camera: " + ms + "ms\n" +
                  "elapsed switch textures: " + (ms2 - ms) + "ms\n" +
                  "elapsed read pixels: " + (ms3 - ms2) + "ms\n" +
                  "elapsed switch textures and apply: " + (ms4 - ms3) + "ms\n" +
                  "elapsed total: " + sw.ElapsedMilliseconds + "ms");
    }
}