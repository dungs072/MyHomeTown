using System;
using System.Collections.Generic;
using UnityEngine;
//! The origin of the grid is the center (0,0) of the grid
[ExecuteInEditMode]
public class GridSystem : MonoBehaviour
{
    [Range(2, 10000)]
    [SerializeField] private int totalHorizontalSlot = 10;
    [Range(2, 10000)]
    [SerializeField] private int totalVerticalSlot = 10;

    private float size = 10;

    private Node[,] grid;

    private Vector3 startPosition;

    private void Awake()
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
        this.size = GridConstants.SMALL_DISTANCE;
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
        foreach (var node in grid)
        {

            if (node != null)
            {
                if (node.IsOccupied)
                {
                    Gizmos.color = Color.red;
                }
                else
                {
                    Gizmos.color = Color.blue;
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
    public List<Node> FindOccupyingNodes(int xSlot, int ySlot, Occupier occupier)
    {
        List<Node> nodes = new List<Node>();
        Vector3 occupierPosition = occupier.transform.position;
        float smallestDistance = float.MaxValue;
        Node nearestNode = null;
        int indexX = -1;
        int indexY = -1;
        for (int i = 0; i < totalHorizontalSlot; i++)
        {
            for (int j = 0; j < totalVerticalSlot; j++)
            {
                Node node = grid[i, j];
                float distance = Vector3.Distance(node.Position, occupierPosition);
                if (distance >= smallestDistance) continue;
                smallestDistance = distance;
                nearestNode = node;
                indexX = i;
                indexY = j;

            }
        }

        if (nearestNode == null) return null;
        // swap misunderstand coordinates
        int halfXSlot = xSlot / 2;
        int halfYSlot = ySlot / 2;

        int startX = Math.Max(0, indexX - halfXSlot);
        int startY = Math.Max(0, indexY - halfYSlot);

        int endX = Math.Min(totalHorizontalSlot, indexX + halfXSlot);
        int endY = Math.Min(totalVerticalSlot, indexY + halfYSlot);


        for (int i = startX + 1; i < endX; i++)
        {
            for (int j = startY + 1; j < endY; j++)
            {
                Node node = grid[i, j];
                nodes.Add(node);
            }
        }
        UnityEditor.SceneView.RepaintAll();
        return nodes;
    }

    public void SnapToGridPoint(Transform target)
    {
        var targetPosition = target.position;

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
