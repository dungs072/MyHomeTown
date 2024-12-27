using UnityEngine;

public class PathNodes : MonoBehaviour
{
    [SerializeField] private PointNode[] pointNodes;

    [Header("Test")]
    [SerializeField]
    private Color gizmosColor = Color.white;
    [SerializeField] private int additionalHeight = 2;

    public Vector3[] GetPath()
    {
        Vector3[] path = new Vector3[pointNodes.Length];
        for (int i = 0; i < pointNodes.Length; i++)
        {
            path[i] = pointNodes[i].transform.position;
        }
        return path;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = gizmosColor;
        for (int i = 0; i < pointNodes.Length; i++)
        {
            if (i + 1 < pointNodes.Length)
            {
                Vector3 startPoint = new Vector3(pointNodes[i].transform.position.x, pointNodes[i].transform.position.y + additionalHeight, pointNodes[i].transform.position.z);
                Vector3 endPoint = new Vector3(pointNodes[i + 1].transform.position.x, pointNodes[i + 1].transform.position.y + additionalHeight, pointNodes[i + 1].transform.position.z);
                Gizmos.DrawLine(startPoint, endPoint);
            }
        }
    }
}
