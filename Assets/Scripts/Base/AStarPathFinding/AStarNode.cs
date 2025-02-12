using UnityEngine;

public class AStarNode
{
    private Vector2Int gridPosition;
    private int gCost;
    private int hCost;
    private AStarNode parent;

    public AStarNode(Vector2Int gridPosition)
    {
        this.gridPosition = gridPosition;
        gCost = 0;
        hCost = 0;
        parent = null;
    }

    public int FCost => gCost + hCost;
    public int GCost
    {
        get => gCost;
        set => gCost = value;
    }
    public int HCost
    {
        get => hCost;
        set => hCost = value;
    }
    public AStarNode Parent
    {
        get => parent;
        set => parent = value;
    }
    public Vector2Int GridPosition => gridPosition;

}
