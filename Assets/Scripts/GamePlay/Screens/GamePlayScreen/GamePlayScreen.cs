using BaseEngine;
using UnityEngine;
using static ManagerSingleton;
public class GamePlayScreen : BaseScreen
{
    [SerializeField] private GamePlayContainer container;

    public GamePlayContainer Container => container;

    void Awake()
    {
        RegisterButtonEvents();
    }
    private void RegisterButtonEvents()
    {
        var header = container.Header;
        header.RegisterSettingButtonEvent(() =>
        {
            HandleSettingButtonClicked();
        });
    }
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

    #region Button Events
    public void HandleSettingButtonClicked()
    {
        var gamePlay = EmpireInstance.GamePlay;
        gamePlay.HandleOpenSettingScreen();
    }
    #endregion
}
