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
    // MapWorld
    public MapWorld MapWorld => mapWorld;
    public GamePlay GamePlay => gamePlay;
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
