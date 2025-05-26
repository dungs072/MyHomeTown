using UnityEngine;

public class PatrollingPath : MonoBehaviour
{
    [Header("Add them in order")]
    [SerializeField] private Transform[] waypoints;
    [SerializeField] private string pathName = "DefaultPath";
    public string PathName => pathName;
    void OnDrawGizmos()
    {
        foreach (var waypoint in waypoints)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(waypoint.position, 0.5f);
        }
    }
}
