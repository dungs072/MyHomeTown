using System;
using System.Collections.Generic;
using System.Linq;
using BaseEngine;
using UnityEngine;
public class GameLoader : Loader
{
    private readonly string propLabel = "PropPrefabs";
    private readonly string itemLabel = "ItemPrefabs";
    public static GameLoader GameLoaderInstance => (GameLoader)Instance;
    protected override void AddAddressableLabels()
    {
        loadedAddressableLabels.Add(propLabel);
        loadedAddressableLabels.Add(itemLabel);
    }
    public void HandleWhenPropPrefabsLoaded(Action onLoaded)
    {
        HandleWhenPrefabsLoaded(propLabel, onLoaded);
    }
    public void HandleWhenItemPrefabsLoaded(Action onLoaded)
    {
        HandleWhenPrefabsLoaded(itemLabel, onLoaded);
    }


    public List<PropertyBase> GetPropPrefabs()
    {
        var prefabs = loadedAddressableAssets[propLabel].prefabs;
        List<PropertyBase> propPrefabs = new();
        foreach (var prefab in prefabs)
        {
            if (prefab.TryGetComponent<PropertyBase>(out var propertyBase))
            {
                propPrefabs.Add(propertyBase);
            }
        }
        return propPrefabs;
    }
    public List<BaseItem> GetItemPrefabs()
    {
        var prefabs = loadedAddressableAssets[itemLabel].prefabs;
        List<BaseItem> itemPrefabs = new();
        foreach (var prefab in prefabs)
        {
            if (prefab.TryGetComponent<BaseItem>(out var baseItem))
            {
                itemPrefabs.Add(baseItem);
            }
        }
        return itemPrefabs;
    }
}
