using UnityEngine;

public static class AStarConstants
{
    public static readonly Vector2Int[] DIRECTIONS = {
        new(0, 1),
        new (1, 0),
        new (0, -1),
        new (-1, 0)
    };
}
