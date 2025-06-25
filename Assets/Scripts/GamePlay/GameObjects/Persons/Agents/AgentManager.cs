using System;
using System.Collections.Generic;
using UnityEngine;
public class AgentManager : MonoBehaviour
{
    public static event Action<AgentController> OnAgentSpawned;
    [SerializeField] private List<Transform> spawnPoints;
    [SerializeField] private float radius = 5f;
    [Header("Agents")]
    [SerializeField] private List<AgentController> agentPrefabs;

    [Header("Debugger")]
    [SerializeField] private Transform target;

    private Dictionary<AgentType, List<AgentController>> agentsDict = new();

    public void SpawnAgents(AgentType agentType, int amount)
    {
        int count = 0;
        while (count < amount)
        {
            AgentController agent = GetAgent(agentType);
            OnAgentSpawned?.Invoke(agent);
            agent.transform.position = GetRandomStartSpawnPoint();
            // if (target != null)
            // {
            //     agent.SetTarget(target);
            // }
            count++;
        }
    }
    private Vector3 GetRandomStartSpawnPoint()
    {
        int randomIndex = UnityEngine.Random.Range(0, spawnPoints.Count);
        Transform startPoint = spawnPoints[randomIndex];
        Vector3 position = startPoint.position;
        float x = UnityEngine.Random.Range(-radius, radius);
        float z = UnityEngine.Random.Range(-radius, radius);
        position.x += x;
        position.z += z;
        return position;
    }

    // handle pool
    private AgentController GetAgent(AgentType agentType)
    {
        AgentController selectedAgent = null;
        if (agentsDict.TryGetValue(agentType, out List<AgentController> agents))
        {
            var agent = agents.Find(a => !a.gameObject.activeSelf && a.AgentType == agentType);
            if (agent == null)
            {
                agent = CreateAgent(agentType);
                if (agent == null)
                {
                    Debug.LogError($"Failed to create agent of type {agentType}. Check if the prefab is assigned.");
                    return null;
                }
                agents.Add(agent);
                selectedAgent = agent;
            }
            else
            {
                agent.gameObject.SetActive(true);
                selectedAgent = agent;
            }
        }
        else
        {
            agents = new List<AgentController>();
            agentsDict[agentType] = agents;
            var agent = CreateAgent(agentType);
            if (agent == null)
            {
                Debug.LogError($"Failed to create agent of type {agentType}. Check if the prefab is assigned.");
                return null;
            }
            agents.Add(agent);
            agent.gameObject.SetActive(true);
            selectedAgent = agent;
        }
        return selectedAgent;
    }
    private AgentController CreateAgent(AgentType agentType)
    {
        AgentController agentPrefab = agentPrefabs.Find(a => a.AgentType == agentType);
        if (agentPrefab == null)
        {
            Debug.LogError($"Agent prefab for type {agentType} not found.");
            return null;
        }
        AgentController newAgent = Instantiate(agentPrefab, transform);
        return newAgent;
    }

    public bool IsAllAgentsLessThan(int amount)
    {
        int totalAgents = 0;
        foreach (var agents in agentsDict.Values)
        {
            foreach (var agent in agents)
            {
                if (agent.gameObject.activeSelf)
                {
                    totalAgents++;
                }
            }
        }
        return totalAgents < amount;
    }

    void OnDrawGizmos()
    {
        foreach (var startPoint in spawnPoints)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(startPoint.position, radius);
        }
    }

}
