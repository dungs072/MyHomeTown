using System.Collections.Generic;
using UnityEngine;

public class ServerBehaviour : BaseBehavior
{
    public ServerBehaviour(Person person) : base(person)
    {
        this.person = person;
    }
    protected override bool TryToMeetConditionsToWork()
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
            if (!needItemsPack.HasItem(itemKey)) continue;
            var amount = needItemsPack.GetAmount(itemKey);
            if (amount > itemsInContainer[itemKey])
            {
                return false;
            }
        }
        return true;
    }

    public override void HandleFinishedStep()
    {
        base.HandleFinishedStep();
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
        TakeNeedItemsFrom(itemsInContainer);
        // take items after done step
        var createdItems = currentStep.Step.Data.PossibleCreateItems;
        person.Pack.AddItems(createdItems);
    }

    // protected override bool CanWork()
    // {
    //     var personStatus = person.PersonStatus;
    //     var selectedWK = personStatus.CurrentWorkContainer;
    //     var isPuttingStation = selectedWK.IsPuttingStation();
    //     return base.CanWork() || isPuttingStation;
    // }

    protected void PutItemsToDoStep(List<ItemRequirement> needItems)
    {
        if (needItems == null || needItems.Count == 0) return;

        foreach (var needItem in needItems)
        {
            var itemKey = needItem.itemKey;
            if (!person.Pack.HasItem(itemKey)) continue;
            var amount = person.Pack.GetAmount(itemKey);
            var requiredAmount = needItem.amount;
            if (amount < requiredAmount) continue;
            person.Pack.RemoveItem(itemKey, requiredAmount);
            needItemsPack.RemoveItem(itemKey, requiredAmount);
        }
    }
    private void PutPossibleItemsToPuttingStation(WorkContainer selectedWK)
    {
        var itemsToPut = new List<ItemKey>(person.Pack.Items);
        foreach (var itemKey in itemsToPut)
        {
            var amount = person.Pack.GetAmount(itemKey);
            selectedWK.AddPossibleItemToContainer(itemKey, amount);
            person.Pack.RemoveItem(itemKey, amount);
        }
    }

}
