using UnityEngine;

public class GamePlayScreen : BaseScreen
{
    [SerializeField] private Container container;

    public Container Container => container;

    // Singletons
    private Loader loader;


    void Awake()
    {
        Loader.OnAssetLoaded += OnAssetLoaded;
    }


    void OnDestroy()
    {
        Loader.OnAssetLoaded -= OnAssetLoaded;
    }

    void Start()
    {
        loader = Loader.Instance;
    }

    private void OnAssetLoaded()
    {
        if (loader == null)
        {
            loader = Loader.Instance;
        }
        foreach (var prop in loader.PropList)
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
