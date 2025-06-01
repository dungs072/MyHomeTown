using UnityEditor;
using UnityEngine;

[ExecuteInEditMode]
public class PatrollingPath : MonoBehaviour
{
    [Header("Add them in order")]
    [SerializeField] private Transform[] waypoints;
    [SerializeField] private string pathName = "DefaultPath";
    public string PathName => pathName;
    public Transform[] Waypoints => waypoints;

    void OnDrawGizmos()
    {
        if (waypoints == null || waypoints.Length < 2) return;
        if (waypoints.Length == 0)
        {
            Debug.LogWarning("No waypoints assigned to the patrolling path.");
            return;
        }
        int segments = waypoints.Length - 1;

        for (int i = 0; i < waypoints.Length; i++)
        {
            if (waypoints[i] == null) continue;

            float t = (float)i / (segments - 1);
            Gizmos.color = Color.Lerp(Color.red, Color.green, t);
            Gizmos.DrawSphere(waypoints[i].position, 0.5f);

            if (i < segments && waypoints[i + 1] != null)
            {
                Vector3 from = waypoints[i].position;
                Vector3 to = waypoints[i + 1].position;

                float lineT = (float)i / (segments - 1);
                Gizmos.color = Color.Lerp(Color.red, Color.green, lineT);
                Gizmos.DrawLine(from, to);
            }
        }
    }
}
