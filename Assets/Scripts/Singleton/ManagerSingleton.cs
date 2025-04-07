using UnityEngine;
/// <summary>
/// Singleton class to manage all the game systems and components.
/// Make sure this script run first when game scene started to game mechanism work properly.
/// </summary>
[RequireComponent(typeof(MapWorld))]
public class ManagerSingleton : CoreBehavior
{
    public override int Priority => 0;
    public override bool IsExisting => true;
    [SerializeField] private GridSystem gridSystem;
    [SerializeField] private TaskManager taskManager;
    [SerializeField] private WorkContainerManager workContainerManager;

    [Header("Factory")]
    [SerializeField] private PropertyFactory propertyFactory;
    [Header("UI")]
    [SerializeField] private GameUI gameUI;
    private static ManagerSingleton instance;
    public static ManagerSingleton Instance => instance;

    public GridSystem GridSystem => gridSystem;
    public TaskManager TaskManager => taskManager;
    public WorkContainerManager WorkContainerManager => workContainerManager;
    // Factory
    public PropertyFactory PropertyFactory => propertyFactory;
    // UI
    public GameUI GameUI => gameUI;

    // MapWorld
    public MapWorld MapWorld => mapWorld;

    private MapWorld mapWorld;
    public override void OnStart()
    {
        if (instance == null)
        {
            instance = this;
            // force to destroy the game object
        }
        else
        {
            Destroy(gameObject);
        }

        InitComponents();
    }
    private void InitComponents()
    {
        mapWorld = GetComponent<MapWorld>();
    }
}
