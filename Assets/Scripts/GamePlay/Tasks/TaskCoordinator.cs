using System.Collections.Generic;
using Unity.Entities.UniversalDelegates;
using Unity.VisualScripting;
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
        if (selectedWK == null) return;
        var targetPosition = GetTargetPosition(currentStep, selectedWK, person);
        if (!agent.IsReachedDestination(targetPosition))
        {
            agent.SetDestination(targetPosition);
            person.SwitchState(PersonState.MOVE);
            return;
        }
        if (!HasEnoughItemToDoStep(person, currentStep)) return;
        HandleStep(person, selectedWK, currentStep);
        if (currentStep.IsFinished)
        {
            var itemsInContainer = selectedWK.ItemsInContainer;
            var baseCharacter = person.BaseCharacter;
            var createdItems = currentStep.Step.Data.PossibleCreateItems;
            PutNeedItemsToDoStep(person, currentStep);
            PutPossibleItemToContainer(baseCharacter, selectedWK);

            baseCharacter.TakeNeedItems(itemsInContainer);
            // items are created by finishing the step
            baseCharacter.AddOwningItems(createdItems);



            taskPerformer.MoveToNextStep();
            selectedWK.RemovePersonFromWorkContainer(person);
        }
        if (taskPerformer.IsFinished())
        {
            taskHandler.MoveNextTask();
        }
    }
    private Vector3 GetTargetPosition(StepPerformer step, WorkContainer selectedWK, Person person)
    {
        var isWorkHereInfinite = step.IsWorkHereInfinite();
        var targetPosition = Vector3.zero;
        var isPuttingStation = step.Step.Data.WorkContainerType == WorkContainerType.PUTTING_STATION; ;
        if (isWorkHereInfinite)
        {
            // Handle infinite work here
            selectedWK.SetServerPerson(person);
            targetPosition = selectedWK.GetServerPosition();
        }
        else if (isPuttingStation && person.TryGetComponent(out ServerCharacter server))
        {
            targetPosition = selectedWK.GetPuttingPosition();
        }
        else
        {
            selectedWK.AddPersonToWorkContainer(person);
            targetPosition = selectedWK.GetWaitingPosition(person);
        }
        return targetPosition;
    }
    private bool HasEnoughItemToDoStep(Person person, StepPerformer step)
    {
        var baseCharacter = person.BaseCharacter;
        var isEnough = true;
        if (step.NeedItems == null || step.NeedItems.Count == 0) return isEnough;
        foreach (var needItem in step.NeedItems)
        {
            var itemKey = needItem.itemData.itemKey;
            if (!baseCharacter.OwningItemsDict.ContainsKey(itemKey) ||
                baseCharacter.OwningItemsDict[itemKey] < needItem.itemData.amount)
            {
                isEnough = false;
                break;
            }
        }
        return isEnough;
    }
    private void PutNeedItemsToDoStep(Person person, StepPerformer step)
    {
        var baseCharacter = person.BaseCharacter;
        var needItems = step.NeedItems;
        if (needItems == null || needItems.Count == 0) return;

        foreach (var needItem in needItems)
        {
            var itemKey = needItem.itemData.itemKey;
            if (!baseCharacter.OwningItemsDict.TryGetValue(itemKey, out int amount)) return;
            var requiredAmount = needItem.itemData.amount;
            if (amount < requiredAmount) return;
            baseCharacter.RemoveOwningItem(itemKey, requiredAmount);
        }
    }
    private void PutPossibleItemToContainer(BaseCharacter baseCharacter, WorkContainer selectedWK)
    {
        if (!selectedWK.IsPuttingStation()) return;
        var itemsToPut = new List<ItemKey>(baseCharacter.OwningItemsList);
        foreach (var itemKey in itemsToPut)
        {
            var amount = baseCharacter.OwningItemsDict[itemKey];
            selectedWK.AddPossibleItemToContainer(itemKey, amount);
            baseCharacter.RemoveOwningItem(itemKey, amount);
        }
    }
    private void HandleStep(Person person, WorkContainer selectedWK, StepPerformer currentStep)
    {
        var canWork = selectedWK.IsPersonUse(person);
        var isPuttingStation = selectedWK.IsPuttingStation();
        var isServer = person.TryGetComponent(out ServerCharacter server);
        var isServerPuttingItems = isPuttingStation && isServer;
        if (canWork || isServerPuttingItems)
        {
            var currentProgress = currentStep.Progress;
            var isWorkHereInfinite = currentStep.IsWorkHereInfinite();
            if (!isWorkHereInfinite)
            {
                var newProgress = currentProgress + Time.deltaTime;
                currentStep.SetProgress(newProgress);
            }
            person.SwitchState(PersonState.WORK);
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
