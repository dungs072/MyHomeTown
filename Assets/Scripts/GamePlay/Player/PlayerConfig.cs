using System.Runtime.InteropServices;

public class PlayerConfig
{

}

public class CameraConfig
{
    public const float MAX_MOVEMENT_SPEED = 25f;
    public const float MIN_MOVEMENT_SPEED = 5f;
    public const float ZOOM_SPEED = 60f;

    public const float ROTATION_SPEED = 10F;
    public static int[] CameraZAngles = { 0, 90, 180, 270 };
    public const float MOVEMENT_ACCELERATION = -7.5f;
    public const float ZOOM_ACCELERATION = -7.5f;

    public const float MIN_CAMERA_HEIGHT = 10f;
    public const float MAX_CAMERA_HEIGHT = 50f;

    public const float BORDER_THICKNESS = 10f;

    public const float CAMERA_MOUSE_BORDER_MOVEMENT = 2f;
}
