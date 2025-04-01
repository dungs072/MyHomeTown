using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum OccupierState
{
    Overlap,
    UnOverlap,
    Normal
}

public class Occupier : MonoBehaviour
{
    [Range(1, 100)]
    [SerializeField] private int widthSlots = 1;

    [Range(1, 100)]
    [SerializeField] private int heightSlots = 1;

    [SerializeField] private bool canShowGridVisualize = false;
    [SerializeField] private bool isMoving = true;

    [Header("Debug only")]
    [SerializeField] private MeshRenderer meshRenderer;
    [SerializeField] private Material overlapMaterial;
    [SerializeField] private Material unOverlapMaterial;
    [SerializeField] private Material normalMaterial;

    private List<Node> occupiedNodes;
    //! use this to visualize the grid in the editor.
    private Node[,] visualizeNodes;

    private ManagerSingleton singleton;

    private OccupierState currentOccupierState = OccupierState.Normal;

    public bool IsOverlap => currentOccupierState == OccupierState.Overlap;

    //! for debug in the editor.

    private void OnValidate()
    {
        if (!canShowGridVisualize) return;
        CreateGrid();
    }

    private void Awake()
    {
        occupiedNodes = new List<Node>();
    }
    private void Start()
    {
        singleton = ManagerSingleton.Instance;
        SetOccupiedSlots();
    }

    // private void Update()
    // {
    //     if (!canMove) return;
    //     singleton.GridSystem.SnapToGridPoint(transform);
    //     if (Utils.HasSamePosition(previousPosition, transform.position)) return;
    //     previousPosition = transform.position;
    //     StartCoroutine(HandleOccupiedSlots());
    // }

    private IEnumerator HandleMovingOnGrid()
    {
        while (isMoving)
        {
            var gridSystem = ManagerSingleton.Instance.GridSystem;
            gridSystem.SnapToGridPoint(transform);
            if (gridSystem == null) yield break;
            ClearOccupiedNodes();
            List<Node> occupyingNodes = gridSystem.FindOccupyingNodes(widthSlots, heightSlots, this);
            TryToOccupyingNodes(occupyingNodes);
            yield return null;
        }
    }

    public void StartMove()
    {
        isMoving = true;
        StartCoroutine(HandleMovingOnGrid());
    }
    public void StopMove()
    {
        isMoving = false;
        StopCoroutine(HandleMovingOnGrid());
        ClearOccupiedNodes();
    }


    public void SetOccupiedSlots()
    {
        var gridSystem = singleton.GridSystem;
        if (gridSystem == null) return;
        ClearOccupiedNodes();
        List<Node> occupyingNodes = gridSystem.FindOccupyingNodes(widthSlots, heightSlots, this);
        TryToOccupyingNodes(occupyingNodes);
    }
    private void TryToOccupyingNodes(List<Node> occupyingNodes)
    {
        if (occupyingNodes == null)
        {
            SetCurrentOccupierState(OccupierState.Overlap);
            return;
        }
        for (int i = 0; i < occupyingNodes.Count; i++)
        {
            var node = occupyingNodes[i];
            if (node.Owner != null)
            {
                SetCurrentOccupierState(OccupierState.Overlap);
                return;
            }
        }
        for (int i = 0; i < occupyingNodes.Count; i++)
        {
            var node = occupyingNodes[i];
            node.SetOwner(gameObject);
            occupiedNodes.Add(node);
        }
        var state = isMoving ? OccupierState.UnOverlap : OccupierState.Normal;
        SetCurrentOccupierState(state);

    }
    private void ClearOccupiedNodes()
    {
        if (occupiedNodes == null) return;
        foreach (var node in occupiedNodes)
        {
            node.SetOwner(null);
        }
        occupiedNodes.Clear();
    }

    private void CreateGrid()
    {
        int biggestCenterX = Mathf.CeilToInt(widthSlots / 2f);
        int biggestCenterY = Mathf.CeilToInt(heightSlots / 2f);
        int smallestCenterX = Mathf.FloorToInt(widthSlots / 2f);
        int smallestCenterY = Mathf.FloorToInt(heightSlots / 2f);
        visualizeNodes = new Node[widthSlots, heightSlots];
        float size = GridConstants.SMALL_DISTANCE;
        for (int i = -biggestCenterX + 1; i < biggestCenterX; i++)
        {
            for (int j = -biggestCenterY + 1; j < biggestCenterY; j++)
            {
                Vector3 position = transform.position + new Vector3(i * size, 0, j * size);
                int indexX = i + smallestCenterX;
                int indexY = j + smallestCenterY;
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


    public void SetCurrentOccupierState(OccupierState state)
    {
        currentOccupierState = state;
        UpdateMaterial();
    }
    private void UpdateMaterial()
    {
        if (meshRenderer == null) return;
        if (currentOccupierState == OccupierState.Normal)
        {
            meshRenderer.material = normalMaterial;
        }
        else if (currentOccupierState == OccupierState.Overlap)
        {
            meshRenderer.material = overlapMaterial;
        }
        else if (currentOccupierState == OccupierState.UnOverlap)
        {
            meshRenderer.material = unOverlapMaterial;
        }
    }
}
