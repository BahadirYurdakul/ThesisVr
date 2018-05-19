using UnityEngine;

public class Frame {
    public Quaternion CameraAngle { get; set; }
    public int FrameCount { get; set; }

    public Frame(int frameCount, Quaternion cameraAngle) {
        CameraAngle = cameraAngle;
        FrameCount = frameCount;
    }
}