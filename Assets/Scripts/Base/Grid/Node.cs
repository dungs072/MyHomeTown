using UnityEngine;

public class Node
{
    private int gridX;
    private int gridY;
    private Vector3 position;

    public Node(int gridX, int gridY, Vector3 position)
    {

        this.gridX = gridX;
        this.gridY = gridY;
        this.position = position;
    }

    public int GridX => gridX;
    public int GridY => gridY;
    public Vector3 Position => position;
}
