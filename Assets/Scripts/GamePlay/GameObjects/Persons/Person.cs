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
        Color randomColor = new Color(Random.value, Random.value, Random.value, 1.0f);
        meshRenderer.material.color = randomColor;
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
