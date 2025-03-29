using System;
using System.Collections.Generic;
using UnityEngine;


// add this in the loading scene
public class Loader : MonoBehaviour
{
    public static event Action OnAssetLoaded;
    public static Loader Instance => instance;
    public List<PropertyBase> PropList => propList;

    private List<PropertyBase> propList = new List<PropertyBase>();
    private readonly string propPath = "Props";
    private static Loader instance;
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }

        LoadPropAssets();
    }
    public void LoadPropAssets()
    {
        var loadedPrefabs = Resources.LoadAll<GameObject>(propPath);

        foreach (GameObject prefab in loadedPrefabs)
        {
            var prop = prefab.GetComponent<PropertyBase>();
            if (prop == null)
            {
                Debug.LogWarning("Prefab: " + prefab.name + " does not have PropertyBase component.");
                continue;
            }
            propList.Add(prop);
            Debug.Log("Loaded Prefab: " + prefab.name);
        }
        OnAssetLoaded?.Invoke();
    }
}
