using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AgentManager : MonoBehaviour
{
    [Header("Agents")]
    [SerializeField] private AgentController agentPrefab;
    [SerializeField] private int maxAgents = 10;
    [Header("Path")]
    [SerializeField] private PathNodes path;
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
        Vector3 startPath = path.GetPath()[0];
        Vector3 startSpawnPosition = new Vector3(startPath.x, startPath.y + 2, startPath.z);
        while (count < maxAgents)
        {
            AgentController agent = GetAgent();
            agent.transform.position = startSpawnPosition;
            count++;
            yield return new WaitForSeconds(2f);
        }


    }

    private void AssignPathToAgent(AgentController agent)
    {
        agent.SetAndFollowPath(path.GetPath());
    }
    private void AssignPathToAgents(Vector3[] path)
    {
        for (int i = 0; i < agents.Count; i++)
        {
            agents[i].SetAndFollowPath(path);
        }
    }


    // handle pool
    private AgentController GetAgent()
    {
        AgentController agent = agents.Find(a => !a.gameObject.activeSelf);
        if (agent == null)
        {
            agent = Instantiate(agentPrefab, transform);
            agent.SetCanGoBack(true);
            agent.SetCanUsePool(true);
            agents.Add(agent);
        }
        else
        {
            agent.gameObject.SetActive(true);
        }
        return agent;
    }

}
