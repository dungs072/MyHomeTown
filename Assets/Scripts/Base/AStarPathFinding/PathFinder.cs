using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/*
    * This class is responsible for finding the path between two points.
    * It uses the A* algorithm to find the path. 
 */
public class PathFinder : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 10f;
    private GridSystem grid;

    void Start()
    {
        grid = ManagerSingleton.Instance.GridSystem;
    }

    public void MoveToDestination(Vector3 destination)
    {
        List<AStarNode> path = FindPathTo(destination);
        if (path == null) return;

        StartCoroutine(PlayMovement(path));

    }
    protected IEnumerator PlayMovement(List<AStarNode> nodes)
    {
        foreach (var node in nodes)
        {
            yield return StartCoroutine(PlayMoveTo(node));
        }
    }
    private IEnumerator PlayMoveTo(AStarNode node)
    {
        Vector2Int gridPosition = node.GridPosition;

        Vector3 worldPosition = grid.GetWorldPosition(gridPosition);
        while (!Utils.HasSamePosition(transform.position, worldPosition))
        {
            transform.position = Vector3.MoveTowards(transform.position, worldPosition, moveSpeed * Time.deltaTime);
            yield return null;
        }
    }

    public List<AStarNode> FindPathTo(Vector3 target)
    {
        return FindPath(transform.position, target);
    }

    /// <summary>
    /// Find the path between two points in world position. 
    /// Please for best performance use the FindPath(Vector2Int, Vector2Int) method.
    /// </summary>
    /// <param name="start"></param>
    /// <param name="target"></param>
    /// <returns></returns>
    public List<AStarNode> FindPath(Vector3 start, Vector3 target)
    {
        Vector2Int startGridPos = grid.GetGridPosition(start);
        Vector2Int targetGridPos = grid.GetGridPosition(target);
        return FindPath(startGridPos, targetGridPos);
    }


    /// <summary>
    /// Find the path between two points in grid system.
    /// </summary>
    /// <param name="start"> grid position in grid system </param>
    /// <param name="target"> grid position in grid system </param>
    /// <returns></returns>
    public List<AStarNode> FindPath(Vector2Int start, Vector2Int target)
    {
        PriorityQueue<AStarNode> openSet = new PriorityQueue<AStarNode>();
        HashSet<Vector2Int> closedSet = new HashSet<Vector2Int>();
        Dictionary<Vector2Int, AStarNode> allNodes = new Dictionary<Vector2Int, AStarNode>();
        AStarNode startNode = new(start)
        {
            GCost = 0,
            HCost = GetHeuristic(start, target)
        };

        openSet.Enqueue(startNode, startNode.FCost);
        allNodes[start] = startNode;

        AStarNode closestNode = startNode;

        while (openSet.Count > 0)
        {
            AStarNode currentNode = openSet.Dequeue();
            closedSet.Add(currentNode.GridPosition);

            // Update closest node based on heuristic
            if (GetHeuristic(currentNode.GridPosition, target) < GetHeuristic(closestNode.GridPosition, target))
            {
                closestNode = currentNode;
            }

            if (currentNode.GridPosition == target && grid.IsValidGridPosition(target))
            {
                return RetracePath(currentNode);
            }

            foreach (Vector2Int direction in AStarConstants.DIRECTIONS)
            {
                Vector2Int neighborPos = currentNode.GridPosition + direction;
                if (!grid.IsValidGridPosition(neighborPos) || closedSet.Contains(neighborPos))
                {
                    continue;
                }

                int moveCost = currentNode.GCost + GetMoveCost(currentNode.GridPosition, neighborPos);

                if (!allNodes.TryGetValue(neighborPos, out AStarNode neighborNode))
                {
                    neighborNode = new AStarNode(neighborPos);
                    allNodes[neighborPos] = neighborNode;
                }

                if (moveCost < neighborNode.GCost || !openSet.Contains(neighborNode))
                {
                    neighborNode.GCost = moveCost;
                    neighborNode.HCost = GetHeuristic(neighborPos, target);
                    neighborNode.Parent = currentNode;

                    if (!openSet.Contains(neighborNode))
                    {
                        openSet.Enqueue(neighborNode, neighborNode.FCost);
                    }
                }
            }
        }

        // Return path to closest node if target is unreachable
        return RetracePath(closestNode);
    }


    private int GetMoveCost(Vector2Int from, Vector2Int to)
    {
        return (from.x != to.x && from.y != to.y) ? 14 : 10; // Diagonal = 14, Straight = 10
    }


    private List<AStarNode> RetracePath(AStarNode node)
    {
        List<AStarNode> path = new List<AStarNode>();
        AStarNode currentNode = node;
        while (currentNode != null)
        {
            path.Add(currentNode);
            currentNode = currentNode.Parent;
        }
        path.Reverse();
        return path;
    }
    private int GetHeuristic(Vector2Int a, Vector2Int b)
    {
        return Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y);
    }






}

internal class PriorityQueue<T1, T2>
{
}