using System.Collections.Generic;
using UnityEngine;

public class PatrollingSystem : MonoBehaviour
{
    [SerializeField] private List<PatrollingPath> patrollingPaths;
    private Dictionary<string, PatrollingPath> pathDictionary;
    public IReadOnlyDictionary<string, PatrollingPath> PathDictionary => pathDictionary;
    private void Awake()
    {
        pathDictionary = new Dictionary<string, PatrollingPath>();
        foreach (var path in patrollingPaths)
        {
            if (path != null && !string.IsNullOrEmpty(path.name))
            {
                pathDictionary[path.name] = path;
            }
        }
    }
}
