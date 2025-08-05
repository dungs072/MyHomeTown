using System.Collections.Generic;
using UnityEngine;

public class ServerBehaviour : BaseBehaviour
{
    public ServerBehaviour(Person person) : base(person)
    {
        this.person = person;
    }
    // server character does not patrol
    protected override bool StartPatrollingOverTime(string key = PatrollingPathKey.DefaultPath)
    {
        return true;
    }

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
        var enoughItemInWK = IsEnoughNeedItemInWorkContainer();
        return enoughItemInWK;
    }
    private bool IsEnoughNeedItemInWorkContainer()
    {
        var personStatus = person.PersonStatus;
        var selectedWK = personStatus.CurrentWorkContainer;
        var possibleContainItems = selectedWK.PossibleContainItems;
        var itemsInContainer = selectedWK.ItemsInContainer;
        foreach (var itemKey in possibleContainItems)
        {
            if (!NeedItemsDict.TryGetValue(itemKey, out int amount)) continue;
            if (amount > itemsInContainer[itemKey])
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
            return;
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
    protected void PutItemsToDoStep(List<ItemRequirement> needItems)
    {
        if (needItems == null || needItems.Count == 0) return;

        foreach (var needItem in needItems)
        {
            var itemKey = needItem.itemKey;
            if (!OwningItemsDict.TryGetValue(itemKey, out int amount)) continue;
            var requiredAmount = needItem.amount;
            if (amount < requiredAmount) continue;
            RemoveOwningItem(itemKey, requiredAmount);
            AddNeedItem(itemKey, -requiredAmount);
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
