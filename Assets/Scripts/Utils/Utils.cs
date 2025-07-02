using UnityEngine;

public static class Utils
{
    private readonly static float SMALL_DISTANCE = 0.5f;
    public static bool HasSamePosition(Vector3 position1, Vector3 position2)
    {
        return Vector3.SqrMagnitude(position1 - position2) < SMALL_DISTANCE * SMALL_DISTANCE;
    }
    public static bool IsInView(Camera cam, Transform objTransform)
    {
        Vector3 viewPos = cam.WorldToViewportPoint(objTransform.position);
        bool isVisible = viewPos.x >= 0 && viewPos.x <= 1 &&
                          viewPos.y >= 0 && viewPos.y <= 1 &&
                          viewPos.z > 0;
        return isVisible;
    }

}
