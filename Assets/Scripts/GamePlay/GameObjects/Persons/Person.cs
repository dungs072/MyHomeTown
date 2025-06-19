using System.Collections;
using UnityEngine;
using static ManagerSingleton;

public class PersonData
{
    public string Name { get; set; }
    public int Age { get; set; }
    public PersonState State { get; set; }

    public PersonData(string name, int age, PersonState state)
    {
        Name = name;
        Age = age;
        State = state;
    }
}

public class Person : MonoBehaviour
{
    [SerializeField] private PersonData personData;
    [SerializeField] private MeshRenderer meshRenderer;
    private TaskHandler taskHandler;
    private ManagerSingleton singleton;
    private AgentController agentController;

    public PersonData PersonData => personData;


    void Awake()
    {
        singleton = EmpireInstance;
        taskHandler = GetComponent<TaskHandler>();
        agentController = GetComponent<AgentController>();
    }

    void Start()
    {
        SetRandomColor();
    }

    void OnEnable()
    {
        CreatePersonData();
    }
    // reset the person here to reuse it again
    void OnDisable()
    {
        agentController.ResetAgent();
    }
    private void CreatePersonData()
    {
        string personName = PersonDataGenerator.GenerateName();
        int personAge = PersonDataGenerator.GenerateAge();
        personData = new PersonData(personName, personAge, PersonState.Idle);
    }
    private void SetRandomColor()
    {
        var agentType = agentController.AgentType;
        Color randomColor = GetColor(agentType);
        meshRenderer.material.color = randomColor;
    }
    public static Color GetColor(AgentType type)
    {
        switch (type)
        {
            case AgentType.CUSTOMER:
                return new Color(0.2f, 0.7f, 1f); // Example: light blue
            case AgentType.SERVER:
                return Color.yellow;
            // Add more cases as needed
            default:
                return Color.white;
        }
    }

    public void SwitchState(PersonState newState)
    {
        personData.State = newState;
        // You can add additional logic here if needed when the state changes
    }

    private IEnumerator BehaveLikeNormalPerson()
    {
        SwitchState(PersonState.Walking);
        yield return FollowPath();
        DemoAddMoneyWhenFinished();
        gameObject.SetActive(false);
    }
    private void DemoAddMoneyWhenFinished()
    {
        var player = EmpireInstance.Player;
        if (!player.TryGetComponent(out PlayerWallet playerWallet)) return;
        playerWallet.AddMoney(5);
    }

    private IEnumerator FollowPath()
    {
        var patrollingSystem = singleton.PatrollingSystem;
        var patrollingPath = patrollingSystem.PathDictionary[PatrollingPathKey.DefaultPath];
        if (patrollingPath == null)
        {
            Debug.LogWarning("Patrolling path not found!");
            yield break;
        }
        var points = patrollingPath.Waypoints;
        for (int i = 0; i < points.Length; i++)
        {
            var position = points[i].position;
            yield return agentController.MoveToPosition(position);
        }
    }

    public IEnumerator DoTask(Task task)
    {
        taskHandler.AddTask(task);
        yield return taskHandler.HandleAllAssignedTask();
        taskHandler.RemoveTask(task);

    }
}
