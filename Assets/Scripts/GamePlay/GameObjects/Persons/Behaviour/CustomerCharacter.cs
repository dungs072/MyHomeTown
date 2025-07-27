using System.Collections.Generic;
using UnityEngine;

public class CustomerCharacter : BaseCharacter
{
    protected override bool TryToMeetConditionsToDoStep()
    {
        var personStatus = person.PersonStatus;
        var currentTaskPerformer = personStatus.CurrentTaskPerformer;
        var step = currentTaskPerformer.GetCurrentStepPerformer();
        var selectedWK = personStatus.CurrentWorkContainer;
        var needItems = step.NeedItems;
        if (needItems == null || needItems.Count == 0) return true;
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

    protected override void HandleWithItems()
    {
        TakeItemsFromWorkContainer();
    }
    private void TakeItemsFromWorkContainer()
    {

        var personStatus = person.PersonStatus;
        var currentTaskPerformer = personStatus.CurrentTaskPerformer;
        var step = currentTaskPerformer.GetCurrentStepPerformer();
        var selectedWK = personStatus.CurrentWorkContainer;
        var needItems = step.NeedItems;
        if (needItems == null || needItems.Count == 0) return;
        foreach (var needItem in needItems)
        {
            var itemKey = needItem.itemData.itemKey;
            if (!selectedWK.ItemsInContainer.TryGetValue(itemKey, out int amount)) return;
            var requiredAmount = needItem.itemData.amount;
            if (amount < requiredAmount) return;
            selectedWK.AddItemToContainer(itemKey, -requiredAmount);
            AddOwningItem(itemKey, requiredAmount);
        }
    }
}
