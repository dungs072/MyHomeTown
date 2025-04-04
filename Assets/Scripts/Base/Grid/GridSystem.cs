using System;
using System.Collections.Generic;
using UnityEngine;
//! The origin of the grid is the center (0,0) of the grid
[ExecuteInEditMode]
public class GridSystem : MonoBehaviour
{
    [Range(2, 10000)]
    [SerializeField] protected int totalHorizontalSlot = 10;
    [Range(2, 10000)]
    [SerializeField] protected int totalVerticalSlot = 10;
    [Range(1, 10)]
    [SerializeField] private float size = 10;
    [SerializeField] private Color gridColor = Color.blue;
    [SerializeField] private Color occupiedColor = Color.red;
    [SerializeField] private bool showDebugger = true;



    protected Node[,] grid;

    private Vector3 startPosition;

    public int TotalHorizontalSlot => totalHorizontalSlot;
    public int TotalVerticalSlot => totalVerticalSlot;
    public float Size => size;

    public Vector2Int GetGridPosition(Node node)
    {
        int x = node.GridX;
        int y = node.GridY;
        return new Vector2Int(x, y);
    }

    /// <summary>
    /// This is temporary method to get the grid position of a point in the world. May be it can be wrong
    /// </summary>
    /// <param name="position"></param>
    /// <returns></returns>
    public Vector2Int GetGridPosition(Vector3 position)
    {
        int offsetX = totalHorizontalSlot / 2;
        int offsetY = totalVerticalSlot / 2;
        int x = Mathf.RoundToInt((position.x - startPosition.x) / size);
        int y = Mathf.RoundToInt((position.z - startPosition.z) / size);
        return new Vector2Int(x + offsetX, y + offsetY);
    }
    public Vector3 GetWorldPosition(Vector2Int gridPosition)
    {
        int x = gridPosition.x;
        int y = gridPosition.y;
        return grid[x, y].Position;
    }
    public Node GetNode(int x, int y)
    {
        return grid[x, y];
    }
    public bool IsValidGridPosition(Vector2Int gridPos)
    {
        bool isOutOfRange = IsOutOfRange(gridPos.x, gridPos.y);
        if (isOutOfRange) return false;
        Node node = GetNode(gridPos.x, gridPos.y);
        return !node.IsOccupied;
    }

    private void Awake()
    {
        InitGridData();
    }
    protected virtual void InitGridData()
    {
        SetUpPropertiesOfGrid();
        SetStartGridPosition();
        CreateGrid();
    }
    private void OnValidate()
    {
        SetUpPropertiesOfGrid();
        SetStartGridPosition();
        CreateGrid();
    }
    private void SetStartGridPosition()
    {
        startPosition = transform.position;
    }
    private void SetUpPropertiesOfGrid()
    {
        //this.size = GridConstants.SMALL_DISTANCE;
    }
    private void CreateGrid()
    {
        int biggestCenterX = Mathf.CeilToInt(totalHorizontalSlot / 2f);
        int biggestCenterY = Mathf.CeilToInt(totalVerticalSlot / 2f);
        int smallestCenterX = Mathf.FloorToInt(totalHorizontalSlot / 2f);
        int smallestCenterY = Mathf.FloorToInt(totalVerticalSlot / 2f);
        grid = new Node[totalHorizontalSlot, totalVerticalSlot];
        for (int i = -biggestCenterX; i < biggestCenterX; i++)
        {
            for (int j = -biggestCenterY; j < biggestCenterY; j++)
            {
                Vector3 position = startPosition + new Vector3(i * size, 0, j * size);
                int indexX = i + smallestCenterX;
                int indexY = j + smallestCenterY;
                grid[indexX, indexY] = new Node(indexX, indexY, position);
            }
        }
    }

    private void OnDrawGizmos()
    {
        if (!showDebugger) return;
        if (grid == null)
        {
            return;
        }
        foreach (var node in grid)
        {

            if (node != null)
            {
                if (node.IsOccupied)
                {
                    Gizmos.color = occupiedColor;
                }
                else
                {
                    Gizmos.color = gridColor;
                }
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

    // horizontal is x, vertical is y
    public List<Node> FindOccupyingNodes(int width, int height, Occupier occupier)
    {
        List<Node> occupiedNodes = new List<Node>();
        float smallestDistance = float.MaxValue;
        Vector3 occupierPosition = occupier.transform.position;
        float offsetX = (width / 2f) * size;
        float offsetY = (height / 2f) * size;
        Vector3 bottomLeft = occupierPosition + new Vector3(-offsetX, 0, -offsetY);
        if (IsOutRealGridWorldSize(bottomLeft))
        {
            return occupiedNodes;
        }
        int targetX = -1;
        int targetY = -1;
        for (int i = 0; i < totalHorizontalSlot; i++)
        {
            for (int j = 0; j < totalVerticalSlot; j++)
            {
                Node node = grid[i, j];
                float distance = Vector3.Distance(node.Position, bottomLeft);
                if (distance >= smallestDistance) continue;
                smallestDistance = distance;
                targetX = i;
                targetY = j;
            }
        }
        for (int i = targetX; i < targetX + width; i++)
        {
            for (int j = targetY; j < targetY + height; j++)
            {
                Node node = grid[i, j];
                occupiedNodes.Add(node);
            }
        }

        return occupiedNodes;
    }

    public bool IsOutOfRange(int x, int y)
    {
        return x < 0 || x >= totalHorizontalSlot || y < 0 || y >= totalVerticalSlot;
    }
    public bool IsOutRealGridWorldSize(Vector3 position)
    {
        float halfWidth = (totalHorizontalSlot / 2f) * size;
        float halfHeight = (totalVerticalSlot / 2f) * size;
        return position.x < -halfWidth || position.x > halfWidth || position.z < -halfHeight || position.z > halfHeight;
    }

    public void SnapToGridPoint(Transform target, Vector3 targetPosition)
    {

        float smallestDistance = float.MaxValue;
        Node nearestNode = null;
        for (int i = 0; i < totalHorizontalSlot; i++)
        {
            for (int j = 0; j < totalVerticalSlot; j++)
            {
                Node node = grid[i, j];
                float distance = Vector3.Distance(node.Position, targetPosition);
                if (distance >= smallestDistance) continue;
                smallestDistance = distance;
                nearestNode = node;
            }
        }
        if (nearestNode == null) return;
        target.position = nearestNode.Position;

    }

}
