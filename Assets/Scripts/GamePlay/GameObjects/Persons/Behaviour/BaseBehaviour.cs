using System.Collections.Generic;
using UnityEngine;
using static ManagerSingleton;

public class BaseBehaviour : IPersonBehaviour
{
    protected Person person;
    protected TaskHandler taskHandler;
    protected AgentController agent;
    protected PatrollingSystem patrollingSystem;

    // owning items
    protected List<ItemKey> owningItemsList = new();
    protected Dictionary<ItemKey, int> owningItemsDict = new();
    public Dictionary<ItemKey, int> OwningItemsDict => owningItemsDict;
    public List<ItemKey> OwningItemsList => owningItemsList;

    // need items
    protected Dictionary<ItemKey, int> needItemsDict = new();
    public Dictionary<ItemKey, int> NeedItemsDict => needItemsDict;
    protected List<ItemKey> needItemsList = new();
    public List<ItemKey> NeedItemsList => needItemsList;

    protected int currentWaitPointIndex = 0;
    private TaskPerformer previousTaskPerformer;
    public BaseBehaviour(Person person)
    {
        this.person = person;
        taskHandler = person.GetComponent<TaskHandler>();
        agent = person.GetComponent<AgentController>();
        patrollingSystem = EmpireInstance.PatrollingSystem;
    }
    public void ExecuteBehaviour()
    {
        if (!StartPatrollingOverTime()) return;
        UpdateHandleTask();
    }

    #region Patrolling
    protected virtual bool StartPatrollingOverTime()
    {
        if (!patrollingSystem) return true;
        var patrollingPath = patrollingSystem.PathDictionary[PatrollingPathKey.DefaultPath];
        if (patrollingPath == null || patrollingPath.Waypoints.Length == 0) return true;

        var maxIndex = patrollingPath.Waypoints.Length - 1;
        if (currentWaitPointIndex >= maxIndex) return true;

        var targetPosition = patrollingPath.Waypoints[currentWaitPointIndex].position;
        person.SwitchState(PersonState.MOVE);

        if (!agent.IsReachedDestination(targetPosition))
        {
            agent.SetDestination(targetPosition);
        }
        else
        {
            currentWaitPointIndex = (currentWaitPointIndex + 1) % patrollingPath.Waypoints.Length;
        }

        return false;
    }
    #endregion

    #region Task Handling
    protected virtual void UpdateHandleTask()
    {
        var personStatus = person.PersonStatus;
        var taskPerformer = personStatus.CurrentTaskPerformer;
        if (taskPerformer == null || taskPerformer.IsFinished()) return;
        var currentStep = taskPerformer.GetCurrentStepPerformer();

        if (!personStatus.CurrentWorkContainer)
        {
            personStatus.CurrentWorkContainer = TaskCoordinator.GetSuitableWorkContainer(currentStep.Step.Data.WorkContainerType, person);
        }

        if (previousTaskPerformer != taskPerformer)
        {
            HandleStartTask();
            previousTaskPerformer = taskPerformer;
        }

        if (!TryToMoveToTarget()) return;
        if (!TryToMeetConditionsToDoStep()) return;

        HandleStep();

        if (currentStep.IsFinished)
        {
            HandleFinishedStep();
        }

        if (taskPerformer.IsFinished())
        {
            taskHandler.MoveNextTask();
            previousTaskPerformer = null;
            needItemsDict.Clear();
            needItemsList.Clear();
        }
    }

    protected virtual void HandleStartTask()
    {
        var needItems = GetNeedItemsFromCurrentToEndStep();
        if (needItems == null || needItems.Count == 0) return;

        foreach (var needItem in needItems)
        {
            var itemData = needItem.itemData;
            if (itemData == null) continue;
            var itemKey = itemData.itemKey;
            var itemAmount = itemData.amount;
            AddNeedItem(itemKey, itemAmount);

            if (!needItemsList.Contains(itemKey))
            {
                needItemsList.Add(itemKey);
            }
        }
    }

    protected bool TryToMoveToTarget()
    {
        var selectedWK = person.PersonStatus.CurrentWorkContainer;
        if (selectedWK == null) return false;

        var targetPosition = GetTargetPosition();

        if (!agent.IsReachedDestination(targetPosition))
        {
            agent.SetDestination(targetPosition);
            person.SwitchState(PersonState.MOVE);
            return false;
        }

        return true;
    }

    protected virtual Vector3 GetTargetPosition()
    {
        var selectedWK = person.PersonStatus.CurrentWorkContainer;
        selectedWK.AddPersonToWorkContainer(person);
        return selectedWK.GetWaitingPosition(person);
    }

    protected virtual bool TryToMeetConditionsToDoStep()
    {
        return true;
    }

    protected void HandleStep()
    {
        var selectedWK = person.PersonStatus.CurrentWorkContainer;
        if (CanWork())
        {
            DoWork();
        }
        else
        {
            Wait();
        }
    }

    protected virtual bool CanWork()
    {
        var selectedWK = person.PersonStatus.CurrentWorkContainer;
        return selectedWK.IsPersonUse(person);
    }

    protected virtual void DoWork()
    {
        var currentStep = person.PersonStatus.CurrentTaskPerformer.GetCurrentStepPerformer();
        var currentProgress = currentStep.Progress;
        var newProgress = currentProgress + Time.deltaTime;
        currentStep.SetProgress(newProgress);
        person.SwitchState(PersonState.WORK);
    }

    protected virtual void Wait()
    {
        person.SwitchState(PersonState.WAIT);
    }

    protected void HandleFinishedStep()
    {
        var personStatus = person.PersonStatus;
        var selectedWK = personStatus.CurrentWorkContainer;
        var taskPerformer = personStatus.CurrentTaskPerformer;

        HandleWithItems();
        taskPerformer.MoveToNextStep();
        selectedWK.RemovePersonFromWorkContainer(person);
        personStatus.CurrentWorkContainer = null;
    }

    protected virtual void HandleWithItems()
    {
        // Overridable logic
    }
    #endregion

    #region Items
    public void AddOwningItem(ItemKey key, int amount)
    {
        if (owningItemsDict.ContainsKey(key))
        {
            owningItemsDict[key] += amount;
        }
        else
        {
            owningItemsDict[key] = amount;
            owningItemsList.Add(key);
        }

        UpdateDisplayInfo();
    }

    public void AddOwningItems(List<ItemRequirement> items)
    {
        if (items == null || items.Count == 0) return;

        foreach (var item in items)
        {
            if (item == null) continue;
            AddOwningItem(item.itemKey, item.amount);
        }
    }

    public void RemoveOwningItem(ItemKey key, int amount)
    {
        if (owningItemsDict.ContainsKey(key))
        {
            owningItemsDict[key] -= amount;
            if (owningItemsDict[key] <= 0)
            {
                owningItemsList.Remove(key);
            }
        }

        UpdateDisplayInfo();
    }

    public void AddNeedItem(ItemKey key, int amount)
    {
        if (needItemsDict.ContainsKey(key))
        {
            needItemsDict[key] += amount;
        }
        else
        {
            needItemsDict[key] = amount;
        }
    }

    private void UpdateDisplayInfo()
    {
        person.InfoPersonUI.SetInfoText($"Has {owningItemsList.Count} items");
        var stringBuilder = new System.Text.StringBuilder();

        foreach (var item in owningItemsDict)
        {
            var itemName = ItemKeyNames.ToName(item.Key);
            stringBuilder.AppendLine($"{itemName} x{item.Value}");
        }

        person.InfoPersonUI.SetInfoText(stringBuilder.ToString());
    }

    public void TakeNeedItems(Dictionary<ItemKey, int> items)
    {
        var needItems = GetNeedItemsFromCurrentToEndStep();
        if (needItems == null || needItems.Count == 0) return;

        foreach (var needItem in needItems)
        {
            var itemKey = needItem.itemData.itemKey;
            var requiredAmount = needItem.itemData.amount;

            if (items.TryGetValue(itemKey, out int amount))
            {
                var gainedAmount = Mathf.Min(amount, requiredAmount);
                AddOwningItem(itemKey, gainedAmount);
                items[itemKey] -= gainedAmount;
            }
        }
    }

    protected virtual List<GatheredItem> GetNeedItemsFromCurrentToEndStep()
    {
        List<GatheredItem> items = new();
        var personStatus = person.PersonStatus;
        var currentTask = personStatus.CurrentTaskPerformer;
        var currentStep = currentTask.GetCurrentStepPerformer();
        if (currentStep == null) return null;

        for (int i = currentTask.CurrentStepIndex; i < currentTask.StepPerformers.Count; i++)
        {
            var step = currentTask.StepPerformers[i];
            if (step == null) continue;
            var stepNeedItems = step.NeedItems;
            if (stepNeedItems == null || stepNeedItems.Count == 0) continue;
            items.AddRange(stepNeedItems);
        }

        return items;
    }
    #endregion


}