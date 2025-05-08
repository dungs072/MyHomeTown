using UnityEngine;

public class Node
{
    private int gridX;
    private int gridY;
    private Vector3 position;
    private GameObject owner;

    public Node(int gridX, int gridY, Vector3 position)
    {

        this.gridX = gridX;
        this.gridY = gridY;
        this.position = position;
    }

    public void SetOwner(GameObject owner)
    {
        this.owner = owner;
    }

    public int GridX => gridX;
    public int GridY => gridY;
    public Vector3 Position => position;

    public bool IsOccupied => owner != null;
    public GameObject Owner => owner;


    private void OnDrawGizmos()
    {
        if (!IsOccupied) return;
        Gizmos.color = Color.red;
        Gizmos.DrawCube(position, Vector3.one);
    }
}
