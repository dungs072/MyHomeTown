using UnityEngine;

public static class Utils
{
    private readonly static float SMALL_DISTANCE = 1f;
    public static bool HasSamePosition(Vector3 position1, Vector3 position2)
    {
        return Vector3.Distance(position1, position2) < SMALL_DISTANCE;
    }
}
