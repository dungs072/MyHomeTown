using System.Collections.Generic;
using Unity.Entities.UniversalDelegates;
using Unity.VisualScripting;
using UnityEngine;
using static ManagerSingleton;
/// <summary>
/// This will coordinate tasks between from agent to agent (server to customer).
/// </summary>
public class TaskCoordinator : MonoBehaviour
{
    private AgentManager agentManager;

    private List<Person> persons = new();
    private ServerCoordinator serverCoordinator = new();
    private CustomerCoordinator customerCoordinator = new();
    void Start()
    {
        AgentManager.OnAgentSpawned += OnAgentSpawned;
        //WorkContainer.OnAvailable += OnWorkContainerAvailable;
    }
    private void OnDestroy()
    {
        AgentManager.OnAgentSpawned -= OnAgentSpawned;
        //WorkContainer.OnAvailable -= OnWorkContainerAvailable;
    }
    private void OnAgentSpawned(AgentController agent)
    {
        if (agent == null) return;
        var person = agent.GetComponent<Person>();
        if (person == null) return;
        persons.Add(person);
        var taskHandler = person.GetComponent<TaskHandler>();
        if (person.PersonBehavior is ServerBehavior)
        {
            serverCoordinator.AddServer(taskHandler);
        }
        else if (person.PersonBehavior is CustomerBehavior)
        {
            customerCoordinator.AddCustomer(taskHandler);
        }
        AssignRandomTaskToCustomers();
    }
    private void AssignRandomTaskToCustomers()
    {
        customerCoordinator.AddSameTasksToAllCustomers(new List<TaskName> { TaskName.DINNER });
    }

    public static WorkContainer GetSuitableWorkContainer(WorkContainerType type, Transform worker)
    {
        var wkDict = EmpireInstance.WorkContainerManager.WorkContainerDict;
        if (!wkDict.TryGetValue(type, out var workContainers)) return null;
        if (workContainers.Count == 0) return null;
        WorkContainer closest = workContainers[0];
        float minSqrDist = float.MaxValue;
        foreach (var wc in workContainers)
        {
            float sqrDist = (wc.transform.position - worker.position).sqrMagnitude;
            if (sqrDist < minSqrDist && !wc.HasWorkersInQueue())
            {
                minSqrDist = sqrDist;
                closest = wc;
            }
        }
        return closest;
    }

}
