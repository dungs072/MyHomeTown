using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static ManagerSingleton;

public class PersonData
{
    public string Name { get; set; }
    public int Age { get; set; }

    public PersonData(string name, int age)
    {
        Name = name;
        Age = age;
    }
}

public class Person : MonoBehaviour
{
    [SerializeField] private List<TaskData> tasksData;
    [SerializeField] private PersonData personData;
    [SerializeField] private MeshRenderer meshRenderer;
    [SerializeField] private InfoPersonUI infoPersonUI;
    private ManagerSingleton singleton;
    private AgentController agentController;
    private PersonStatus personStatus;

    public PersonData PersonData => personData;
    public InfoPersonUI InfoPersonUI => infoPersonUI;

    public PersonStatus PersonStatus => personStatus;
    public List<TaskData> TasksData => tasksData;

    private int currentTaskIndex = 0;


    void Awake()
    {
        singleton = EmpireInstance;
        agentController = GetComponent<AgentController>();
    }

    void Start()
    {
        SetRandomColor();
    }

    void OnEnable()
    {
        CreatePersonData();
        SetInitTasks();
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
        personData = new PersonData(personName, personAge);
        personStatus = new();
    }
    private void SetInitTasks()
    {
        // temporary code to set initial tasks
        var taskManager = singleton.TaskManager;
        var task = taskManager.TasksDict[tasksData[currentTaskIndex]];
        if (task == null) return;
        personStatus.CurrentTaskPerformer = new TaskPerformer();
        personStatus.CurrentTaskPerformer.SetTask(task);
        personStatus.CurrentState = PersonState.IDLE;
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
        personStatus.CurrentState = newState;
        // You can add additional logic here if needed when the state changes
    }

    public void MoveNextTask()
    {
        currentTaskIndex++;
        if (currentTaskIndex >= tasksData.Count)
        {
            SwitchState(PersonState.IDLE);
            gameObject.SetActive(false);
        }
        else
        {
            personStatus.CurrentTaskPerformer = new TaskPerformer();
            var taskManager = singleton.TaskManager;
            var task = taskManager.TasksDict[tasksData[currentTaskIndex]];
            personStatus.CurrentTaskPerformer.SetTask(task);
        }
    }




}
