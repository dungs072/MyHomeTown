using System.Collections.Generic;
using UnityEngine;
using System;
public class Pack
{

    public event Action OnPackChanged;
    private Dictionary<ItemKey, int> itemDictionary;
    private List<ItemKey> items;
    private int maxSize = 0;
    public List<ItemKey> Items => items;
    /// <summary>
    /// Default max size is 10.
    /// If maxSize is -1, there is no limit.
    /// </summary>
    /// <param name="maxSize"></param>
    public Pack(int maxSize = 10)
    {
        this.maxSize = maxSize;
        itemDictionary = new Dictionary<ItemKey, int>();
        items = new List<ItemKey>();
    }
    public int GetAmount(ItemKey itemKey)
    {
        if (itemDictionary.ContainsKey(itemKey))
        {
            return itemDictionary[itemKey];
        }
        return 0;
    }
    public bool HasItem(ItemKey itemKey)
    {
        return itemDictionary.ContainsKey(itemKey);
    }


    public bool AddItems(List<ItemRequirement> itemsToAdd)
    {
        if (itemsToAdd == null || itemsToAdd.Count == 0) return false;
        //! temporary code you have to roll back or do something if you want to add more items than max size
        foreach (var item in itemsToAdd)
        {
            if (item == null) continue;
            if (!AddItem(item.itemKey, item.amount)) return false;
        }
        return true;
    }


    public bool AddItem(ItemKey itemKey, int amount)
    {
        if (amount <= 0) return false;
        var isMaxSize = maxSize != -1 && items.Count >= maxSize;
        if (isMaxSize) return false;
        if (itemDictionary.ContainsKey(itemKey))
        {
            itemDictionary[itemKey] += amount;
        }
        else
        {
            items.Add(itemKey);
            itemDictionary[itemKey] = amount;
        }
        OnPackChanged?.Invoke();
        return true;
    }
    public bool RemoveItem(ItemKey itemKey, int amount)
    {
        if (amount <= 0 || !itemDictionary.ContainsKey(itemKey)) return false;
        if (itemDictionary[itemKey] < amount) return false;
        itemDictionary[itemKey] -= amount;
        if (itemDictionary[itemKey] <= 0)
        {
            itemDictionary.Remove(itemKey);
            items.Remove(itemKey);
        }
        OnPackChanged?.Invoke();
        return true;
    }
    /// <summary>
    /// Remove of a list of items (clear them from dict and list)
    /// </summary>
    /// <param name="itemsToRemove"></param>
    /// <returns></returns>
    public void RemoveItems(List<ItemKey> itemsToRemove)
    {
        if (itemsToRemove == null || itemsToRemove.Count == 0) return;
        foreach (var item in itemsToRemove)
        {
            itemDictionary.Remove(item);
            items.Remove(item);
        }
    }

    public void Clear()
    {
        itemDictionary.Clear();
        items.Clear();
        OnPackChanged?.Invoke();
    }

}
