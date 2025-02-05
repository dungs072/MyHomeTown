using UnityEngine;
//! The origin of the grid is the center (0,0) of the grid
[ExecuteInEditMode]
public class GridSystem : MonoBehaviour
{
    [Range(2, 10000)]
    [SerializeField] private int totalHorizontalSlot = 10;
    [Range(2, 10000)]
    [SerializeField] private int totalVerticalSlot = 10;

    [Range(1, 100)]
    [SerializeField] private float size = 10;

    private Node[,] grid;

    private Vector3 startPosition;

    private void Awake()
    {
        SetStartGridPosition();
        CreateGrid();
    }
    private void OnValidate()
    {
        SetStartGridPosition();
        CreateGrid();
    }
    private void SetStartGridPosition()
    {
        startPosition = transform.position;
    }
    private void CreateGrid()
    {
        int centerX = totalHorizontalSlot / 2;
        int centerY = totalVerticalSlot / 2;
        grid = new Node[totalHorizontalSlot, totalVerticalSlot];
        for (int i = -centerX; i < centerX; i++)
        {
            for (int j = -centerY; j < centerY; j++)
            {
                Vector3 position = startPosition + new Vector3(i * size, 0, j * size);
                int indexX = i + centerX;
                int indexY = j + centerY;
                grid[indexX, indexY] = new Node(indexX, indexY, position);
            }
        }
    }

    private void OnDrawGizmos()
    {
        if (grid == null)
        {
            return;
        }
        Gizmos.color = Color.blue;
        foreach (var node in grid)
        {

            if (node != null)
            {
                Vector3 topLeft = node.Position + new Vector3(-size / 2, 0, size / 2);
                Vector3 topRight = node.Position + new Vector3(size / 2, 0, size / 2);
                Vector3 bottomLeft = node.Position + new Vector3(-size / 2, 0, -size / 2);
                Vector3 bottomRight = node.Position + new Vector3(size / 2, 0, -size / 2);

                Gizmos.DrawLine(topLeft, topRight);
                Gizmos.DrawLine(topRight, bottomRight);
                Gizmos.DrawLine(bottomRight, bottomLeft);
                Gizmos.DrawLine(bottomLeft, topLeft);
            }
        }
    }

}
