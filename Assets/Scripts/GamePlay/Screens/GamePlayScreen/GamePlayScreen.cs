using System.Collections.Generic;
using BaseEngine;
using UnityEngine;

public class GamePlayScreen : BaseScreen
{
    [SerializeField] private GamePlayContainer container;

    public GamePlayContainer Container => container;
    void Start()
    {
        CreatePropsUI();
    }


    private void CreatePropsUI()
    {
        var loader = GameLoader.GameLoaderInstance;
        loader.HandleWhenPropPrefabsLoaded(() =>
        {
            HandleCreatePropsUI();
        });

    }
    private void HandleCreatePropsUI()
    {
        var loader = GameLoader.GameLoaderInstance;
        var props = loader.GetPropPrefabs();
        foreach (var prop in props)
        {
            var propComponent = prop.GetComponent<PropertyBase>();
            if (propComponent == null)
            {
                Debug.LogError($"Prop {prop.name} does not have a Prop component.");
                continue;
            }
            var propName = propComponent.ProductName;
            container.Footer.CreateProp(propName, null);
        }
    }
}
