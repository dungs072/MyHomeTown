using UnityEngine;

public class MapWorld : MonoBehaviour
{
    [SerializeField] private float leftBound = -50f;
    [SerializeField] private float rightBound = 50f;
    [SerializeField] private float topBound = 50f;
    [SerializeField] private float bottomBound = -50f;

    public float LeftBound => leftBound;
    public float RightBound => rightBound;
    public float TopBound => topBound;
    public float BottomBound => bottomBound;
}
