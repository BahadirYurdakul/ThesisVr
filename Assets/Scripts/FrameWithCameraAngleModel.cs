
public class FrameWithCameraAngleModel {
    float x, y;
    int frameCount;

    public FrameWithCameraAngleModel(float x, float y, int frameCount)
    {
        this.x = x;
        this.y = y;
        this.frameCount = frameCount;
    }

    public override string ToString()
    {
        return "x angle: " + x + ", y angle: " + y +  ", frame count: " + frameCount;
    }
}
