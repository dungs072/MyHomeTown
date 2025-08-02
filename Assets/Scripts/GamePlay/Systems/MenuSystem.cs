using System.Collections.Generic;
using UnityEngine;

public class MenuSystem : MonoBehaviour
{
    private List<ItemKey> menuItems = new();

    void Awake()
    {
        InitializeMenu();
    }
    public List<ItemRequirement> GetMenuItems()
    {
        List<ItemRequirement> gatheredItems = new();
        foreach (var itemKey in menuItems)
        {
            var shouldAdd = Random.Range(0, 2) == 0; // Randomly decide to add item or not
            if (!shouldAdd) continue;
            var randomAmount = Random.Range(1, 3); // Random amount between 1 and 2
            var itemRequirement = new ItemRequirement
            {
                itemKey = itemKey,
                amount = randomAmount
            };
            gatheredItems.Add(itemRequirement);
        }
        return gatheredItems;
    }
    private void InitializeMenu()
    {
        // please add items to the menu
        menuItems.Add(ItemKey.DISH_1);
        menuItems.Add(ItemKey.DISH_2);
    }

    public void AddRangeItems(List<ItemKey> items)
    {
        foreach (var item in items)
        {
            AddItem(item);
        }
    }
    public void AddItem(ItemKey item)
    {
        if (menuItems.Contains(item)) return;
        menuItems.Add(item);
    }
    public void RemoveItems(List<ItemKey> items)
    {
        foreach (var item in items)
        {
            RemoveItem(item);
        }
    }
    public void RemoveItem(ItemKey item)
    {
        if (!menuItems.Contains(item)) return;
        menuItems.Remove(item);
    }


}
