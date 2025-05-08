using UnityEngine;
/// <summary>
/// Singleton class to manage all the game systems and components.
/// Make sure this script run first when game scene started to game mechanism work properly.
/// </summary>
[RequireComponent(typeof(MapWorld))]
public class ManagerSingleton : MonoBehaviour
{
    [SerializeField] private GridSystem gridSystem;
    [SerializeField] private TaskManager taskManager;
    [SerializeField] private WorkContainerManager workContainerManager;

    [Header("Factory")]
    [SerializeField] private PropertyFactory propertyFactory;
    [Header("UI")]
    [SerializeField] private GamePlayScreen gameUI;
    private static ManagerSingleton instance;
    public static ManagerSingleton Instance => instance;

    public GridSystem GridSystem => gridSystem;
    public TaskManager TaskManager => taskManager;
    public WorkContainerManager WorkContainerManager => workContainerManager;
    // Factory
    public PropertyFactory PropertyFactory => propertyFactory;
    // UI
    public GamePlayScreen GameUI => gameUI;

    // MapWorld
    public MapWorld MapWorld => mapWorld;

    private MapWorld mapWorld;
    void Start()
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
