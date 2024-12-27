using System.Collections.Generic;
using UnityEngine;

public class AgentManager : MonoBehaviour
{
    [Header("Agents")]
    [SerializeField] private AgentController agentPrefab;
    [SerializeField] private int maxAgents = 10;
    [Header("Test")]
    [SerializeField] private PathNodes path;
    private List<AgentController> agents;


    private void Awake()
    {
        agents = new List<AgentController>();
    }

    private void Start()
    {
        for (int i = 0; i < maxAgents; i++)
        {
            AgentController agent = Instantiate(agentPrefab, transform);
            agents.Add(agent);
        }

        AssignPathToAgents(path.GetPath());
    }
    private void AssignPathToAgents(Vector3[] path)
    {
        for (int i = 0; i < agents.Count; i++)
        {
            agents[i].SetAndFollowPath(path);
        }
    }

}
