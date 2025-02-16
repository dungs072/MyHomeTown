using UnityEngine;

public static class Utils
{
    private readonly static float SMALL_DISTANCE = 0.1f;
    public static bool HasSamePosition(Vector3 position1, Vector3 position2)
    {
        return Vector3.SqrMagnitude(position1 - position2) < SMALL_DISTANCE * SMALL_DISTANCE;
    }

}
