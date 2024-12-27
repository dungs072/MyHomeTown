using UnityEngine;

public class PointNode : MonoBehaviour
{
    [SerializeField] private Color gizmosColor = Color.white;
    [SerializeField] private float radius = 0.1f;
    [SerializeField] private int additionalHeight = 2;
    private void OnDrawGizmos()
    {
        Gizmos.color = gizmosColor;
        Vector3 centerPoint = new Vector3(transform.position.x, transform.position.y + additionalHeight, transform.position.z);
        Gizmos.DrawSphere(centerPoint, radius);
    }
}
