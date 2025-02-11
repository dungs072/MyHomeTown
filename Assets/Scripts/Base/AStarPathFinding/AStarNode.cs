using UnityEngine;

public class AStarNode : Node
{
    private int gCost;
    private int hCost;
    private AStarNode parent;

    public AStarNode(int gridX, int gridY, Vector3 position) : base(gridX, gridY, position)
    {
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

}
