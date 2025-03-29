using System;
using System.Collections.Generic;
using UnityEngine;

public class PropertyFactory : BaseFactory
{
    void Awake()
    {
        Loader.OnAssetLoaded += OnAssetLoaded;
    }
    void OnDestroy()
    {
        Loader.OnAssetLoaded -= OnAssetLoaded;
    }
    private void OnAssetLoaded()
    {
        RegisterPrefabs();
    }
    private void RegisterPrefabs()
    {
        var productPrefabs = Loader.Instance.PropList;
        for (int i = 0; i < productPrefabs.Count; i++)
        {
            string productName = productPrefabs[i].ProductName;
            RegisterProduct(productName, productPrefabs[i]);
        }
    }


}
