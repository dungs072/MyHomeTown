using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AgentManager : MonoBehaviour
{
    [SerializeField] private Transform startPoint;
    [SerializeField] private float radius = 5f;
    [Header("Agents")]
    [SerializeField] private AgentController agentPrefab;

    [Header("Debugger")]
    [SerializeField] private Transform target;
    private List<AgentController> agents;


    private void Awake()
    {
        agents = new List<AgentController>();
    }

    public IEnumerator SpawnAgents(int amount)
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
        yield return null;
    }
    private Vector3 GetRandomStartSpawnPoint()
    {
        Vector3 position = startPoint.position;
        float x = Random.Range(-radius, radius);
        float z = Random.Range(-radius, radius);
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

    public bool IsAllAgentsInPool()
    {
        foreach (var agent in agents)
        {
            if (agent.gameObject.activeSelf)
            {
                return false;
            }
        }
        return true;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(startPoint.position, radius);
    }

}
