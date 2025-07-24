using System.Collections.Generic;
using UnityEngine;

public class ServerCharacter : BaseCharacter
{

    protected override Vector3 GetTargetPosition()
    {
        var personStatus = person.PersonStatus;
        var selectedWK = personStatus.CurrentWorkContainer;
        var taskPerformer = personStatus.CurrentTaskPerformer;
        var currentStep = taskPerformer.GetCurrentStepPerformer();
        var isWorkHereInfinite = currentStep.IsWorkHereInfinite();
        var isPuttingStation = currentStep.Step.Data.WorkContainerType == WorkContainerType.PUTTING_STATION; ;
        if (isWorkHereInfinite)
        {
            selectedWK.SetServerPerson(person);
            return selectedWK.GetServerPosition();
        }
        else if (isPuttingStation)
        {
            return selectedWK.GetPuttingPosition();
        }
        return base.GetTargetPosition();
    }
    protected override bool TryToMeetConditionsToDoStep()
    {
        var personStatus = person.PersonStatus;
        var taskPerformer = personStatus.CurrentTaskPerformer;
        var currentStep = taskPerformer.GetCurrentStepPerformer();
        var needItems = currentStep.NeedItems;
        if (needItems == null || needItems.Count == 0) return true;
        foreach (var needItem in needItems)
        {
            var itemKey = needItem.itemData.itemKey;
            if (!OwningItemsDict.ContainsKey(itemKey) ||
                OwningItemsDict[itemKey] < needItem.itemData.amount)
            {
                return false;
            }
        }
        return true;
    }

    protected override bool CanWork()
    {
        var personStatus = person.PersonStatus;
        var selectedWK = personStatus.CurrentWorkContainer;
        var currentStep = personStatus.CurrentTaskPerformer.GetCurrentStepPerformer();
        var isPuttingStation = selectedWK.IsPuttingStation();
        var isWorkHereInfinite = currentStep.IsWorkHereInfinite();
        return (base.CanWork() || isPuttingStation) && !isWorkHereInfinite;
    }


    protected override void HandleWithItems()
    {
        var personStatus = person.PersonStatus;
        var selectedWK = personStatus.CurrentWorkContainer;
        var taskPerformer = personStatus.CurrentTaskPerformer;
        var currentStep = taskPerformer.GetCurrentStepPerformer();
        if (selectedWK.IsPuttingStation())
        {
            PutPossibleItemsToPuttingStation(selectedWK);
        }
        else
        {
            PutItemsToDoStep(currentStep.NeedItems);
        }
        // take items along
        var itemsInContainer = selectedWK.ItemsInContainer;
        TakeNeedItems(itemsInContainer);
        // take items after done step
        var createdItems = currentStep.Step.Data.PossibleCreateItems;
        AddOwningItems(createdItems);

    }
    private void PutItemsToDoStep(List<GatheredItem> needItems)
    {
        if (needItems == null || needItems.Count == 0) return;
        foreach (var needItem in needItems)
        {
            var itemKey = needItem.itemData.itemKey;
            if (!OwningItemsDict.TryGetValue(itemKey, out int amount)) return;
            var requiredAmount = needItem.itemData.amount;
            if (amount < requiredAmount) return;
            RemoveOwningItem(itemKey, requiredAmount);
        }
    }
    private void PutPossibleItemsToPuttingStation(WorkContainer selectedWK)
    {
        var itemsToPut = new List<ItemKey>(OwningItemsList);
        foreach (var itemKey in itemsToPut)
        {
            var amount = OwningItemsDict[itemKey];
            selectedWK.AddPossibleItemToContainer(itemKey, amount);
            RemoveOwningItem(itemKey, amount);
        }
    }

}
