using UnityEngine;

public class Frame {
    public Quaternion CameraRotation { get; set; }
    public int Id { get; set; }
    public bool IsDrawn { get; set; }

    public Frame(int id, Quaternion cameraRotation) {
        CameraRotation = cameraRotation;
        Id = id;
        IsDrawn = false;
    }
}