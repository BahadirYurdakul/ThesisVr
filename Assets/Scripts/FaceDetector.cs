using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
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
    private List<FrameWithCameraAngleModel> frameWithCameraAngleList;
    public GameObject mainCamera;
    private GameObject locator;
    private GameObject container;
    private bool anyOpenCvPlugins;

    public GameObject g;

    private int w;
    private int h;

    private const int ResMult = 4;

    // Use this for initialization
    void Start() {
        frameWithCameraAngleList = new List<FrameWithCameraAngleModel>();
        if (mainCamera == null)
            mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
        locator = GameObject.FindGameObjectWithTag("VideoSpriteLocator");
        container = GameObject.FindGameObjectWithTag("VideoSpriteContainer");

        //test için
        //addRoundObjectToFace(344, 37, 0, 0, 0, 500, 300); //344.45, 37.4103, 0
        //addRoundObjectToFace(0, 0, 0); //344.45, 37.4103, 0
        //test

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
        //Debug.Log("Cam angle is x: " + cam.transform.eulerAngles.x + "  y: " + cam.transform.eulerAngles.y + " z: " + cam.transform.eulerAngles.z);
        //Debug.Log("Main camera angle is x: " + mainCamera.transform.eulerAngles.x + "  y: " + mainCamera.transform.eulerAngles.y + " z: " + mainCamera.transform.eulerAngles.z);

        framec++;
        var frameId = framec / 30;

        foreach (var model in frameWithCameraAngleList) {
            var allPluginShapes = frameDist.CallStatic<string>("getFrameShapes", model.frameCount);
            if (allPluginShapes == null || allPluginShapes.Equals(""))
                continue;
            foreach (var shapes in allPluginShapes.Split(';')) {
                foreach (var shape in shapes.Split(':')) {
                    var attrs = shape.Split(',');
                    switch (attrs[0]) {
                        case "rectangle":
                            var x = float.Parse(attrs[1], CultureInfo.InvariantCulture.NumberFormat);
                            var y = float.Parse(attrs[2], CultureInfo.InvariantCulture.NumberFormat);
                            var width = float.Parse(attrs[3], CultureInfo.InvariantCulture.NumberFormat);
                            var height = float.Parse(attrs[4], CultureInfo.InvariantCulture.NumberFormat);
                            addRoundObjectToFace(model.x, model.y, 0, x, y, width, height);
                            // TODO draw 
                            break;
                        case "text":
                            break;
                        default:
                            continue;
                    }
                }
            }
        }

        //Screen.dpi;

        if (framec % 30 == 0)
            StartCoroutine(Detect(frameId));

        // Camera.main.transform.rotation
    }

    void OnDisable() {
        Debug.Log("Disabled....");
        AddRemoveParentHelper.Instance.ClearGameObjectChildren(container);
    }

    IEnumerator Detect(int frameId) {
        var sw = new System.Diagnostics.Stopwatch();
        sw.Start();
        yield return new WaitForEndOfFrame();

        cam.Render();

        var ms = sw.ElapsedMilliseconds;

        var temp = RenderTexture.active;
        RenderTexture.active = rt;

        float nearPlane = cam.nearClipPlane;
        /*var botLeft = cam.ViewportToWorldPoint(new Vector3(0, 0, nearPlane));
        var topLeft = cam.ViewportToWorldPoint(new Vector3(0, 1, nearPlane));
        var botRight = cam.ViewportToWorldPoint(new Vector3(1, 0, nearPlane));
        var topRight = cam.ViewportToWorldPoint(new Vector3(1, 1, nearPlane));
        Debug.Log("tl: " +topLeft + " bl: " + botLeft + " br: " + botRight + " tr: " + topRight);
        Instantiate(g, )*/

        var ms2 = sw.ElapsedMilliseconds;

        // put buffer into texture
        texture.ReadPixels(rect, 0, 0);

        var ms3 = sw.ElapsedMilliseconds;

        RenderTexture.active = temp;
        yield return null;

        texture.Apply();

        var ms4 = sw.ElapsedMilliseconds;

        yield return null;

        var eulerAngles = mainCamera.transform.eulerAngles;
        frameWithCameraAngleList.Add(new FrameWithCameraAngleModel(eulerAngles.x, eulerAngles.y, frameId));

        using (var mat = new AndroidJavaObject("org.opencv.core.MatOfByte", texture.EncodeToJPG(70))) {
            frameDist.CallStatic("distribute", frameId, mat);
        }

        yield return null;

        Debug.Log("\nelapsed render camera: " + ms + "ms\n" +
                  "elapsed switch textures: " + (ms2 - ms) + "ms\n" +
                  "elapsed read pixels: " + (ms3 - ms2) + "ms\n" +
                  "elapsed switch textures and apply: " + (ms4 - ms3) + "ms\n" +
                  "elapsed total: " + sw.ElapsedMilliseconds + "ms");
    }

    private void addRoundObjectToFace(float rotationX, float rotationY, float rotationZ, float posX, float posY,
        float width, float height)
    {

        float initialX = -0.5f;
        float initialY = 0.5f;

        float normalizedPositionX = posX * 4 / Screen.width;
        float normalizedPositionY = -(posY * 4 / Screen.height);

        normalizedPositionX += initialX;
        normalizedPositionY += initialY;

        float normalizedWidth = width * 4 / Screen.width;
        float normalizedHeight = height * 4 / Screen.height;
        float normalizedScale = normalizedHeight > normalizedWidth ? normalizedHeight : normalizedWidth;
        normalizedScale += 50;

        Debug.Log("normalizedPositionX: " + normalizedPositionX +
            ", normalizedPositionY: " + normalizedPositionY +
            ", normalizedWidth: " + normalizedWidth +
            ", normalizedHeight: " + normalizedHeight +
            ", normalizedScale: " + normalizedScale
            );
        var newRoundObject = AddRemoveParentHelper.Instance.InstantiatePrefab("FaceRoundPrefab", "newObject", locator);
        locator.transform.Rotate(new Vector3(rotationX, rotationY, rotationZ));
        normalizedPositionX += newRoundObject.transform.position.x;
        normalizedPositionY += newRoundObject.transform.position.y;
        Debug.Log("Pos x: " + newRoundObject.transform.position.x + ", Pos y: " + newRoundObject.transform.position.y);
        newRoundObject.transform.position =
            new Vector3(normalizedPositionX, normalizedPositionY, newRoundObject.transform.position.z);
        newRoundObject.transform.localScale = new Vector3(normalizedScale, normalizedScale, normalizedScale);
        AddRemoveParentHelper.Instance.SetParentObject(newRoundObject, container);
        locator.transform.Rotate(new Vector3(-rotationX, -rotationY, -rotationZ));
    }
}