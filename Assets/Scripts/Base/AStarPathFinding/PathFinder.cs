using System.Collections.Generic;
using UnityEngine;


/*
    * This class is responsible for finding the path between two points.
    * It uses the A* algorithm to find the path. 
 */
public class PathFinder : MonoBehaviour
{
    private GridSystem grid;


    void Awake()
    {
        grid = ManagerSingleton.Instance.GridSystem;

    }

    public List<AStarNode> FindPath(Vector2Int start, Vector2Int target)
    {
        List<AStarNode> openSet = new List<AStarNode>();
        HashSet<Vector2Int> closedSet = new HashSet<Vector2Int>();
        Dictionary<Vector2Int, AStarNode> allNodes = new Dictionary<Vector2Int, AStarNode>();
        AStarNode startNode = new(start);
        AStarNode targetNode = new(target);

        openSet.Add(startNode);
        allNodes[start] = startNode;
        while (openSet.Count > 0)
        {
            openSet.Sort((a, b) => a.FCost.CompareTo(b.FCost));
            AStarNode currentNode = openSet[0];
            openSet.RemoveAt(0);

            closedSet.Add(currentNode.GridPosition);


            if (currentNode.GridPosition == targetNode.GridPosition)
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
                int moveCost = currentNode.GCost + 1; // Assuming uniform cost for each step
                AStarNode neighborNode = null;
                if (!allNodes.TryGetValue(neighborPos, out AStarNode neighborAStarNode))
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
                        openSet.Add(neighborNode);
                    }
                }
            }

        }
        return null;
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
