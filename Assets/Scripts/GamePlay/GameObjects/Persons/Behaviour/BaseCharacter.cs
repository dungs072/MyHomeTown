using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static ManagerSingleton;

[RequireComponent(typeof(Person))]
public class BaseCharacter : MonoBehaviour
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

    // patrolling
    protected int currentWaitPointIndex = 0;

    private TaskPerformer previousTaskPerformer;

    void Awake()
    {
        InitComponents();
    }

    protected virtual void InitComponents()
    {
        person = GetComponent<Person>();
        taskHandler = GetComponent<TaskHandler>();
        agent = GetComponent<AgentController>();
    }

    void Start()
    {
        patrollingSystem = EmpireInstance.PatrollingSystem;
        //StartCoroutine(HandlePreTasks());
    }
    #region Patrolling
    public virtual bool StartPatrollingOverTime()
    {
        if (!patrollingSystem) return true;
        var patrollingPath = patrollingSystem.PathDictionary[PatrollingPathKey.DefaultPath];
        if (patrollingPath == null) return true;
        if (patrollingPath.Waypoints.Length == 0) return true;

        var maxIndex = patrollingPath.Waypoints.Length - 1;
        if (currentWaitPointIndex >= maxIndex)
        {
            return true;
        }
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
    public virtual void UpdateHandleTask()
    {
        var personStatus = person.PersonStatus;
        var taskPerformer = personStatus.CurrentTaskPerformer;
        if (taskPerformer == null || taskPerformer.IsFinished()) return;
        var currentStep = taskPerformer.GetCurrentStepPerformer();
        // find suitable work container
        if (!personStatus.CurrentWorkContainer)
        {
            personStatus.CurrentWorkContainer = TaskCoordinator.GetSuitableWorkContainer(currentStep.Step.Data.WorkContainerType, person);

        }
        if (previousTaskPerformer != taskPerformer)
        {
            HandleStartTask();
            previousTaskPerformer = taskPerformer;
        }

        // do their own job
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
        }
    }
    protected virtual void HandleStartTask()
    {
        // This method can be overridden to handle the start of a task
        var needItems = GetNeedItemsFromCurrentToEndStep();
        if (needItems == null || needItems.Count == 0) return;
        foreach (var needItem in needItems)
        {
            var itemData = needItem.itemData;
            var itemKey = itemData.itemKey;
            var itemAmount = itemData.amount;
            AddNeedItem(itemKey, itemAmount);
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
        // Implement specific conditions to meet prerequisites for the step
        return true;
    }
    protected void HandleStep()
    {
        var selectedWK = person.PersonStatus.CurrentWorkContainer;
        var canWork = CanWork();
        if (canWork)
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
        var canWork = selectedWK.IsPersonUse(person);
        return canWork;
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
        // Override this method to handle items after finishing the task
        // For example, you can take items from the work container or add created items to the character's inventory
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
    //! must edit the way to get step of specific task
    //! temporary get index = 0
    private List<GatheredItem> GetNeedItemsFromCurrentToEndStep()
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
