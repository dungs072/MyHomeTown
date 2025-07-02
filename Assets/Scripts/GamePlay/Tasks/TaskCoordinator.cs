using System.Collections.Generic;
using UnityEngine;
using static ManagerSingleton;
public class TaskCoordinator : MonoBehaviour
{
    private WorkContainerManager workContainerManager;
    private AgentManager agentManager;
    void Start()
    {
        workContainerManager = EmpireInstance.WorkContainerManager;
        agentManager = EmpireInstance.AgentManager;
    }

    void Update()
    {
        var agentTypes = AgentTypeList.AgentTypes;
        foreach (var agentType in agentTypes)
        {
            if (!agentManager.AgentsDict.ContainsKey(agentType)) continue;
            var agents = agentManager.AgentsDict[agentType];
            if (agents == null || agents.Count == 0) continue;
            UpdateAgents(agents);
        }
    }
    private void UpdateAgents(List<AgentController> agents)
    {
        foreach (var agent in agents)
        {
            DoTask(agent);
        }
    }
    private void DoTask(AgentController agent)
    {
        if (!agent.TryGetComponent(out Person person)) return;
        var taskHandler = person.GetComponent<TaskHandler>();
        var personStatus = person.PersonStatus;
        var taskPerformer = personStatus.CurrentTaskPerformer;
        if (taskPerformer == null || taskPerformer.IsFinished()) return;
        var currentStep = taskPerformer.GetCurrentStepPerformer();
        var selectedWK = GetSuitableWorkContainer(currentStep.Step.Data.WorkContainerType, person);
        selectedWK.AddPersonToWorkContainer(person);
        var targetPosition = selectedWK.GetWaitingPosition(person);
        if (!agent.IsReachedDestination(targetPosition))
        {
            agent.SetDestination(targetPosition);
            person.SwitchState(PersonState.MOVE);
            return;
        }
        HandleStep(person, selectedWK, currentStep);
        if (currentStep.IsFinished)
        {
            taskPerformer.MoveToNextStep();
            selectedWK.RemovePersonFromWorkContainer(person);
        }
        if (taskPerformer.IsFinished())
        {
            taskHandler.MoveNextTask();
        }
    }
    private void HandleStep(Person person, WorkContainer selectedWK, StepPerformer currentStep)
    {
        var isNeedItem = currentStep.Step.Data.IsNeedItem;
        var baseCharacter = person.GetComponent<BaseCharacter>();
        var needObjects = baseCharacter.NeedObjects;
        var hasNeedItems = isNeedItem && needObjects != null && needObjects.Count > 0;
        var canWork = selectedWK.IsPersonUse(person) && !hasNeedItems;
        if (canWork)
        {
            var currentProgress = currentStep.Progress;
            currentStep.SetProgress(currentProgress + Time.deltaTime);
            person.SwitchState(PersonState.WORK);
        }
        else if (hasNeedItems)
        {
            person.SwitchState(PersonState.WAIT);

        }
        else
        {
            person.SwitchState(PersonState.WAIT);
            // wait in line
        }
    }

    private WorkContainer GetSuitableWorkContainer(WorkContainerType type, Person person)
    {
        var workContainers = workContainerManager.WorkContainers
      .FindAll(wc => wc.WorkContainerType == type);
        if (workContainers.Count == 0) return null;

        WorkContainer closest = null;
        float minSqrDist = float.MaxValue;
        foreach (var wc in workContainers)
        {
            float sqrDist = (wc.transform.position - person.transform.position).sqrMagnitude;
            if (sqrDist < minSqrDist)
            {
                minSqrDist = sqrDist;
                closest = wc;
            }
        }
        return closest;

    }

}
