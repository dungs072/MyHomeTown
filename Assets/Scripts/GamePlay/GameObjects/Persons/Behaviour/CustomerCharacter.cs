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
}
