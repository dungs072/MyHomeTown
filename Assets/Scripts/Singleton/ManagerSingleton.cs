using UnityEngine;
/// <summary>
/// Singleton class to manage all the game systems and components.
/// Make sure this script run first when game scene started to game mechanism work properly.
/// </summary>
[RequireComponent(typeof(MapWorld))]
public class ManagerSingleton : MonoBehaviour
{
    [Header("Systems")]
    [SerializeField] private GridSystem gridSystem;
    [SerializeField] private PatrollingSystem patrollingSystem;
    [Header("Manager")]
    [SerializeField] private TaskManager taskManager;
    [SerializeField] private WorkContainerManager workContainerManager;
    [SerializeField] private AgentManager agentManager;
    [Header("Factory")]
    [SerializeField] private PropertyFactory propertyFactory;
    [SerializeField] private ItemFactory itemFactory;
    [Header("Player")]
    [SerializeField] private PlayerController player;
    [Header("Level Generator")]
    [SerializeField] private LevelGenerator levelGenerator;
    private static ManagerSingleton instance;
    public static ManagerSingleton EmpireInstance => instance;

    public GridSystem GridSystem => gridSystem;
    public PatrollingSystem PatrollingSystem => patrollingSystem;
    // managers
    public TaskManager TaskManager => taskManager;
    public WorkContainerManager WorkContainerManager => workContainerManager;
    public AgentManager AgentManager => agentManager;
    // Factory
    public PropertyFactory PropertyFactory => propertyFactory;
    public ItemFactory ItemFactory => itemFactory;
    // MapWorld
    public MapWorld MapWorld => mapWorld;
    public GamePlay GamePlay => gamePlay;
    public PlayerController Player => player;
    public LevelGenerator LevelGenerator => levelGenerator;
    private MapWorld mapWorld;
    private GamePlay gamePlay;
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
        gamePlay = GetComponent<GamePlay>();
    }
}
