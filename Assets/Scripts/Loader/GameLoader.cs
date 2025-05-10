using System;
using System.Collections.Generic;
using BaseEngine;
using UnityEngine;
public class GameLoader : Loader
{
    private readonly string propPath = "Props";
    public static GameLoader GameLoaderInstance => (GameLoader)Instance;
    protected override void AddPrefabPathInFolder()
    {
        loadedPrefabAssetsInFolder.Add(propPath);
    }

    public List<PropertyBase> GetPropPrefabs()
    {
        var prefabs = GetPrefabsInFolder(propPath);
        List<PropertyBase> propertyBases = new List<PropertyBase>();
        foreach (var prefab in prefabs)
        {
            if (prefab.TryGetComponent<PropertyBase>(out var propertyBase))
            {
                propertyBases.Add(propertyBase);
            }
        }
        return propertyBases;
    }
}
