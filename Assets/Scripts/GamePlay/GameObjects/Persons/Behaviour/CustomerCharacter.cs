using System.Collections.Generic;
using UnityEngine;

public class CustomerCharacter : BaseCharacter
{
    protected List<GatheredItem> needObjects;

    public List<GatheredItem> NeedObjects => needObjects;

    public void AddNeedObject(ItemRequirement item)
    {
        needObjects ??= new List<GatheredItem>();

        var needObject = new GatheredItem
        {
            itemData = item,
            gainedAmount = 0
        };
        var itemName = ItemKeyNames.ToName(item.itemKey);
        person.InfoPersonUI.SetInfoText($"Need {itemName} x{item.amount}");
        needObjects.Add(needObject);
    }


    protected override bool TryToMeetConditionsToDoStep()
    {
        var personStatus = person.PersonStatus;
        var step = personStatus.CurrentStepPerformer;
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
        var step = personStatus.CurrentStepPerformer;
        var selectedWK = personStatus.CurrentWorkContainer;
        var needItems = step.NeedItems;
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
