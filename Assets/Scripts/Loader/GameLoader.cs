using System;
using System.Collections.Generic;
using System.Linq;
using BaseEngine;
using UnityEngine;
public class GameLoader : Loader
{
    private readonly string propLabel = "PropPrefabs";
    public static GameLoader GameLoaderInstance => (GameLoader)Instance;
    protected override void AddAddressableLabels()
    {
        loadedAddressableLabels.Add(propLabel);
    }
    public void HandleWhenPropPrefabsLoaded(Action onLoaded)
    {
        HandleWhenPropPrefabsLoaded(propLabel, onLoaded);
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
}
