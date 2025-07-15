using System.Collections.Generic;
using UnityEngine;

public class ServerCharacter : BaseCharacter
{
    protected List<ItemKey> owningItemsList = new();
    protected Dictionary<ItemKey, int> owningItemsDict = new();
    public Dictionary<ItemKey, int> OwningItemsDict => owningItemsDict;
    public List<ItemKey> OwningItemsList => owningItemsList;
    public void AddNeedObject(ItemKey key, int amount)
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

        var itemName = ItemKeyNames.ToName(key);
        person.InfoPersonUI.SetInfoText($"Has {itemName} x{owningItemsDict[key]}");
    }
    public void RemoveNeedObject(ItemKey key, int amount)
    {
        if (owningItemsDict.ContainsKey(key))
        {
            owningItemsDict[key] -= amount;
            owningItemsList.Remove(key);
            if (owningItemsDict[key] <= 0)
            {
                owningItemsDict.Remove(key);
            }

        }
    }
}
