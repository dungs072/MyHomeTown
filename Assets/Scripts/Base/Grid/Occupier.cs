using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Occupier : MonoBehaviour
{
    [Range(1, 100)]
    [SerializeField] private int widthSlots = 1;

    [Range(1, 100)]
    [SerializeField] private int heightSlots = 1;

    [SerializeField] private bool canShowGridVisualize = false;

    private List<Node> occupiedNodes;
    //! use this to visualize the grid in the editor.
    private Node[,] visualizeNodes;

    private ManagerSingleton singleton;

    private Vector3 previousPosition;

    private void Start()
    {
        singleton = ManagerSingleton.Instance;
        StartCoroutine(HandleOccupiedSlots());
    }

    private void Update()
    {
        if (Utils.HasSamePosition(previousPosition, transform.position)) return;
        previousPosition = transform.position;
        StartCoroutine(HandleOccupiedSlots());
    }
    //! for debug in the editor.

    private void OnValidate()
    {
        if (!canShowGridVisualize) return;
        CreateGrid();
    }
    private IEnumerator HandleOccupiedSlots()
    {
        yield return new WaitForSeconds(1f);
        var gridSystem = singleton.GridSystem;
        if (gridSystem == null) yield break;
        ClearOccupiedNodes();
        occupiedNodes = gridSystem.SetOccupiedNodes(widthSlots, heightSlots, gameObject);
    }
    private void ClearOccupiedNodes()
    {
        if (occupiedNodes == null) return;
        foreach (var node in occupiedNodes)
        {
            node.SetOwner(null);
        }
        occupiedNodes = null;
    }

    private void CreateGrid()
    {
        int centerX = widthSlots / 2;
        int centerY = heightSlots / 2;
        visualizeNodes = new Node[widthSlots, heightSlots];
        float size = GridConstants.SMALL_DISTANCE;
        for (int i = -centerX + 1; i < centerX; i++)
        {
            for (int j = -centerY + 1; j < centerY; j++)
            {
                Vector3 position = transform.position + new Vector3(i * size, 0, j * size);
                int indexX = i + centerX;
                int indexY = j + centerY;
                visualizeNodes[indexX, indexY] = new Node(indexX, indexY, position);
            }
        }
    }



    private void OnDrawGizmos()
    {
        if (visualizeNodes == null)
        {
            return;
        }
        Gizmos.color = Color.cyan;
        float size = GridConstants.SMALL_DISTANCE;
        foreach (var node in visualizeNodes)
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
