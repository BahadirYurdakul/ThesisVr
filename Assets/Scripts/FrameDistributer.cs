using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using UnityEngine;

public class FrameDistributer : MonoBehaviour {
    private int framec = 0;
    private AndroidJavaObject context;
    private AndroidJavaClass frameDist;
    private RenderTexture rt;
    private Rect rect;
    private Texture2D texture;

    private readonly List<Frame> frames = new List<Frame>();
    private readonly List<GameObject> rects = new List<GameObject>();

    private int _w;
    private int _h;

    private const int ResMult = 4;

    public GameObject rectSprite;
    public GameObject container;
    public Camera cam;

    // Use this for initialization
    void Start() {
        _w = Screen.width / ResMult;
        _h = Screen.height / ResMult;
        rt = new RenderTexture(_w, _h, 24);
        rect = new Rect(0, 0, _w, _h);
        texture = new Texture2D(_w, _h, TextureFormat.RGB24, false);
        cam.targetTexture = rt;
        log("starting detection");

        using (var jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer")) {
            context = jc.GetStatic<AndroidJavaObject>("currentActivity");
            frameDist = new AndroidJavaClass("tr.edu.iyte.vrxd.unityhook.FrameDistributer");
            frameDist.CallStatic<bool>("loadPlugins", context);
        }
    }

    // Update is called once per frame
    void Update() {
        framec++;
        var frameId = framec / 30;

        var nearPlane = cam.nearClipPlane;
        foreach (var frame in frames) {
            if (frame.IsDrawn)
                continue;

            var allPluginShapes = frameDist.CallStatic<string>("getFrameShapes", frame.Id);
            if (allPluginShapes == null || allPluginShapes.Equals(""))
                continue;
            log("shapes:", allPluginShapes);
            foreach (var shapes in allPluginShapes.Split(';')) {
                foreach (var shape in shapes.Split(':')) {
                    var attrs = shape.Split(',');
                    switch (attrs[0]) {
                        case "rectangle":
                            var x = float.Parse(attrs[1], CultureInfo.InvariantCulture);
                            var y = float.Parse(attrs[2], CultureInfo.InvariantCulture);
                            var width = float.Parse(attrs[3], CultureInfo.InvariantCulture);
                            var height = float.Parse(attrs[4], CultureInfo.InvariantCulture);

                            var originalRotation = cam.transform.localRotation;
                            cam.transform.localRotation = frame.CameraRotation;
                            // todo fix
                            var worldPoint =
                                cam.ScreenToWorldPoint(new Vector3(x + width / 2, _h - (y + height / 2), nearPlane + 1));

                            if (rects.Any(r => Vector3.Distance(r.transform.position, worldPoint) < .2f))
                                break;

                            rects.Add(Instantiate(rectSprite, worldPoint,
                                Quaternion.LookRotation(-cam.transform.forward), container.transform));
                            cam.transform.localRotation = originalRotation;
                            break;
                        case "text":
                            break;
                        default:
                            continue;
                    }
                }
            }

            frame.IsDrawn = true;
        }

        if (framec % 30 == 0)
            StartCoroutine(Detect(frameId));
    }

    void OnDisable() {
        log("Disabled....");
        AddRemoveParentHelper.Instance.ClearGameObjectChildren(container);
    }

    private void log(params object[] objs) {
        var s = "";
        foreach (var t in objs)
            s += t + " ";
        Debug.Log(s);
    }

    IEnumerator Detect(int frameId) {
        yield return new WaitForEndOfFrame();

        cam.Render();
        var temp = RenderTexture.active;
        RenderTexture.active = rt;

        // put buffer into texture
        texture.ReadPixels(rect, 0, 0);
        RenderTexture.active = temp;
        yield return null;

        texture.Apply();
        yield return null;

        frames.Add(new Frame(frameId, cam.transform.localRotation));

        using (var mat = new AndroidJavaObject("org.opencv.core.MatOfByte", texture.EncodeToJPG(70))) {
            frameDist.CallStatic("distribute", frameId, mat);
        }

        yield return null;
    }
}