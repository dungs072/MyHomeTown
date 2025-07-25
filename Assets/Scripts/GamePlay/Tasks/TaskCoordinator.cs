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
        Debug.Log($"<color=#bcbedd>agentManager: {agentManager}</color>");
        var agentTypes = AgentTypeList.AgentTypes;
        foreach (var agentType in agentTypes)
        {
            Debug.Log($"<color=#2ad14a>agentType: {agentType}</color>");
            Debug.Log($"<color=#3b8b81>agentManager.AgentsDict.ContainsKey(agentType): {agentManager.AgentsDict.ContainsKey(agentType)}</color>");
            if (!agentManager.AgentsDict.ContainsKey(agentType)) continue;
            var agents = agentManager.AgentsDict[agentType];
            Debug.Log($"<color=#0ddd57>agents: {agents}</color>");
            if (agents == null || agents.Count == 0) continue;
            foreach (var agent in agents)
            {
                if (!agent.TryGetComponent(out BaseCharacter baseCharacter)) continue;
                Debug.Log($"<color=#e3d9d7>baseCharacter: {baseCharacter}</color>");
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
                    baseCharacter.UpdateHandleTask();
                }
            }
        }
        // foreach (var character in characters)
        // {
        //     character.UpdateHandleTask();
        // }
    }
    private void DoTask(AgentController agent)
    {

        if (!agent.TryGetComponent(out Person person)) return;
        var taskHandler = person.GetComponent<TaskHandler>();
        var personStatus = person.PersonStatus;
        var taskPerformer = personStatus.CurrentTaskPerformer;
        if (taskPerformer == null || taskPerformer.IsFinished()) return;
        var currentStep = taskPerformer.GetCurrentStepPerformer();
        WorkContainer selectedWK = personStatus.CurrentWorkContainer;
        if (selectedWK == null)
        {
            selectedWK = GetSuitableWorkContainer(currentStep.Step.Data.WorkContainerType, person);
        }
        if (selectedWK == null) return;
        var targetPosition = GetTargetPosition(currentStep, selectedWK, person);
        if (!agent.IsReachedDestination(targetPosition))
        {
            agent.SetDestination(targetPosition);
            person.SwitchState(PersonState.MOVE);
            return;
        }
        if (!TryToMeetConditionsToDoStep(person, currentStep, selectedWK)) return;
        HandleStep(person, selectedWK, currentStep);
        if (currentStep.IsFinished)
        {
            var itemsInContainer = selectedWK.ItemsInContainer;
            var baseCharacter = person.BaseCharacter;
            var createdItems = currentStep.Step.Data.PossibleCreateItems;
            PutNeedItemsToDoStep(person, currentStep, selectedWK);
            PutPossibleItemToContainer(baseCharacter, selectedWK);
            // items are need to be taken
            baseCharacter.TakeNeedItems(itemsInContainer);
            // items are created by finishing the step
            baseCharacter.AddOwningItems(createdItems);

            taskPerformer.MoveToNextStep();
            selectedWK.RemovePersonFromWorkContainer(person);
            personStatus.CurrentWorkContainer = null;
        }
        if (taskPerformer.IsFinished())
        {
            taskHandler.MoveNextTask();
        }
    }
    private Vector3 GetTargetPosition(StepPerformer step, WorkContainer selectedWK, Person person)
    {
        var isWorkHereInfinite = step.IsWorkHereInfinite();
        var isPuttingStation = step.Step.Data.WorkContainerType == WorkContainerType.PUTTING_STATION; ;
        if (isWorkHereInfinite)
        {
            // Handle infinite work here
            selectedWK.SetServerPerson(person);
            return selectedWK.GetServerPosition();
        }
        else if (isPuttingStation && person.TryGetComponent(out ServerCharacter server))
        {
            return selectedWK.GetPuttingPosition();
        }
        else
        {
            selectedWK.AddPersonToWorkContainer(person);
            return selectedWK.GetWaitingPosition(person);
        }
    }
    private bool TryToMeetConditionsToDoStep(Person person, StepPerformer step, WorkContainer selectedWK)
    {
        var baseCharacter = person.BaseCharacter;
        var needItems = step.NeedItems;
        if (needItems == null || needItems.Count == 0) return true;
        if (baseCharacter is ServerCharacter)
        {
            foreach (var needItem in needItems)
            {
                var itemKey = needItem.itemData.itemKey;
                if (!baseCharacter.OwningItemsDict.ContainsKey(itemKey) ||
                    baseCharacter.OwningItemsDict[itemKey] < needItem.itemData.amount)
                {
                    return false;
                }
            }
            return true;
        }
        else if (baseCharacter is CustomerCharacter)
        {
            foreach (var needItem in needItems)
            {
                var requiredAmount = needItem.itemData.amount;
                var itemKey = needItem.itemData.itemKey;
                if (!selectedWK.ItemsInContainer.ContainsKey(itemKey)) return false;
                var amountInWC = selectedWK.ItemsInContainer[itemKey];
                if (amountInWC < requiredAmount)
                {
                    return false;
                }
            }
            return true;
        }
        //default case
        return false;

    }
    private void PutNeedItemsToDoStep(Person person, StepPerformer step, WorkContainer selectedWK)
    {
        var baseCharacter = person.BaseCharacter;
        var needItems = step.NeedItems;
        if (needItems == null || needItems.Count == 0) return;
        if (baseCharacter is ServerCharacter)
        {
            foreach (var needItem in needItems)
            {
                var itemKey = needItem.itemData.itemKey;
                if (!baseCharacter.OwningItemsDict.TryGetValue(itemKey, out int amount)) return;
                var requiredAmount = needItem.itemData.amount;
                if (amount < requiredAmount) return;
                baseCharacter.RemoveOwningItem(itemKey, requiredAmount);
            }
        }
        else if (baseCharacter is CustomerCharacter)
        {
            foreach (var needItem in needItems)
            {
                var itemKey = needItem.itemData.itemKey;
                if (!selectedWK.ItemsInContainer.TryGetValue(itemKey, out int amount)) return;
                var requiredAmount = needItem.itemData.amount;
                if (amount < requiredAmount) return;
                selectedWK.AddItemToContainer(itemKey, -requiredAmount);
                baseCharacter.AddOwningItem(itemKey, requiredAmount);
            }
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

    public static WorkContainer GetSuitableWorkContainer(WorkContainerType type, Person person)
    {
        var workContainers = EmpireInstance.WorkContainerManager.WorkContainers
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
