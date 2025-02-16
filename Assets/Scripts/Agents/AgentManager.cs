using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AgentManager : MonoBehaviour
{
    [SerializeField] private Transform startPoint;
    [SerializeField] private float radius = 5f;
    [Header("Agents")]
    [SerializeField] private AgentController agentPrefab;
    [SerializeField] private int maxAgents = 10;
    private List<AgentController> agents;


    private void Awake()
    {
        agents = new List<AgentController>();
    }

    private void Start()
    {
        StartCoroutine(SpawnAgents());
    }

    private IEnumerator SpawnAgents()
    {
        int count = 0;
        while (count < maxAgents)
        {
            AgentController agent = GetAgent();
            agent.transform.position = GetRandomStartSpawnPoint();
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

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(startPoint.position, radius);
    }

}
