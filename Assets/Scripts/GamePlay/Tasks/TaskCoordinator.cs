using System.Collections.Generic;
using Unity.Entities.UniversalDelegates;
using Unity.VisualScripting;
using UnityEngine;
using static ManagerSingleton;
public class TaskCoordinator : MonoBehaviour
{
    private AgentManager agentManager;

    private List<BaseCharacter> characters = new();
    void Start()
    {
        agentManager = EmpireInstance.AgentManager;
        var agentTypes = AgentTypeList.AgentTypes;
        foreach (var agentType in agentTypes)
        {
            if (!agentManager.AgentsDict.ContainsKey(agentType)) continue;
            var agents = agentManager.AgentsDict[agentType];
            if (agents == null || agents.Count == 0) continue;
            foreach (var agent in agents)
            {
                if (!agent.TryGetComponent(out BaseCharacter baseCharacter)) continue;
                characters.Add(baseCharacter);
            }
        }
    }

    void Update()
    {
        foreach (var agents in agentManager.AgentsDict.Values)
        {
            foreach (var agent in agents)
            {
                if (agent.TryGetComponent(out BaseCharacter baseCharacter))
                {
                    var isFinishedPatrolling = baseCharacter.StartPatrollingOverTime();
                    if (!isFinishedPatrolling) continue;
                    baseCharacter.UpdateHandleTask();
                }
            }
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
