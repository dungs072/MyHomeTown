using System;
using System.Collections.Generic;
using UnityEngine;
[Serializable]
public class AgentSpawnerData
{
    public AgentController AgentPrefab;
    public int Amount;
    [HideInInspector]
    public List<AgentController> AgentPool = new List<AgentController>();


}
public class AgentManager : MonoBehaviour
{
    [SerializeField] private List<Transform> spawnPoints;
    [SerializeField] private float radius = 5f;
    [Header("Agents")]
    [SerializeField] private List<AgentSpawnerData> agentSpawnersData;

    [Header("Debugger")]
    [SerializeField] private Transform target;

    public void SpawnAgents(int amount)
    {
        int count = 0;
        while (count < amount)
        {
            AgentController agent = GetAgent();
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
    private AgentController GetAgent()
    {
        AgentController agent = agents.Find(a => !a.gameObject.activeSelf);
        if (agent == null)
        {
            agent = Instantiate(agentPrefab, transform);
            // agent.SetCanGoBack(true);
            // agent.SetCanUsePool(true);
            agents.Add(agent);
        }
        else
        {
            agent.gameObject.SetActive(true);
        }
        return agent;
    }

    public bool IsAllAgentsLessThan(int amount)
    {
        int count = 0;
        foreach (var agent in agents)
        {
            if (agent.gameObject.activeSelf)
            {
                count++;
            }
        }
        return count < amount;
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
