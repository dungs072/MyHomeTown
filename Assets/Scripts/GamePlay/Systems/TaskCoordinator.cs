using System.Collections.Generic;
using Unity.Entities.UniversalDelegates;
using Unity.VisualScripting;
using UnityEngine;
using static ManagerSingleton;
public class TaskCoordinator : MonoBehaviour
{
    private AgentManager agentManager;

    private List<Person> persons = new();

    void Start()
    {
        AgentManager.OnAgentSpawned += OnAgentSpawned;
    }
    private void OnDestroy()
    {
        AgentManager.OnAgentSpawned -= OnAgentSpawned;
    }
    private void OnAgentSpawned(AgentController agent)
    {
        if (agent == null) return;
        var person = agent.GetComponent<Person>();
        if (person == null) return;
        persons.Add(person);
    }

    void Update()
    {
        foreach (var person in persons)
        {
            if (person.PersonBehaviour == null) continue;
            person.PersonBehaviour.ExecuteBehaviour();
        }
    }
    public static WorkContainer GetSuitableWorkContainer(WorkContainerType type, Person person)
    {
        var wkDict = EmpireInstance.WorkContainerManager.WorkContainerDict;
        if (!wkDict.TryGetValue(type, out var workContainers)) return null;
        if (workContainers.Count == 0) return null;
        WorkContainer closest = workContainers[0];
        float minSqrDist = float.MaxValue;
        foreach (var wc in workContainers)
        {
            float sqrDist = (wc.transform.position - person.transform.position).sqrMagnitude;
            if (sqrDist < minSqrDist && !wc.HasPersonWaiting())
            {
                minSqrDist = sqrDist;
                closest = wc;
            }
        }
        return closest;

    }

}
