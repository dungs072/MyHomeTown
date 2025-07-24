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

    // owning items
    protected List<ItemKey> owningItemsList = new();
    protected Dictionary<ItemKey, int> owningItemsDict = new();
    public Dictionary<ItemKey, int> OwningItemsDict => owningItemsDict;
    public List<ItemKey> OwningItemsList => owningItemsList;

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
        //StartCoroutine(HandlePreTasks());
    }

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
            Debug.Log($"<color=#17037c>personStatus.CurrentWorkContainer: {personStatus.CurrentWorkContainer}</color>");

        }

        // do their own job
        if (!TryToMoveToTarget()) return;
        if (!TryToMeetConditionsToDoStep()) return;
        HandleStep();
        if (currentStep.IsFinished)
        {
            HandleFinishedTask();

        }
        if (taskPerformer.IsFinished())
        {
            taskHandler.MoveNextTask();
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
    protected void HandleFinishedTask()
    {
        //var itemsInContainer = selectedWK.ItemsInContainer;
        var personStatus = person.PersonStatus;
        var selectedWK = personStatus.CurrentWorkContainer;
        var taskPerformer = personStatus.CurrentTaskPerformer;

        HandleWithItems();
        //var currentStep = taskPerformer.GetCurrentStepPerformer();
        //var createdItems = currentStep.Step.Data.PossibleCreateItems;
        // PutNeedItemsToDoStep(person, currentStep, selectedWK);
        // PutPossibleItemToContainer(baseCharacter, selectedWK);
        // // items are need to be taken
        // baseCharacter.TakeNeedItems(itemsInContainer);
        // // items are created by finishing the step
        // baseCharacter.AddOwningItems(createdItems);

        taskPerformer.MoveToNextStep();
        selectedWK.RemovePersonFromWorkContainer(person);
        personStatus.CurrentWorkContainer = null;
    }
    protected virtual void HandleWithItems()
    {
        // Override this method to handle items after finishing the task
        // For example, you can take items from the work container or add created items to the character's inventory
    }



    protected virtual void OnAllTasksCompleted()
    {
        gameObject.SetActive(false);
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
