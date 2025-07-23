using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static ManagerSingleton;

[RequireComponent(typeof(Person))]
public class BaseCharacter : MonoBehaviour
{
    protected Person person;
    protected TaskHandler taskHandler;


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
    }

    void Start()
    {
        //StartCoroutine(HandlePreTasks());
    }
    protected virtual void OnAllTasksCompleted()
    {
        gameObject.SetActive(false);
    }

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



}
