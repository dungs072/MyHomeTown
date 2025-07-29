using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Entities.UniversalDelegates;
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
    public static event Action<Person> OnPersonStatusChanged;

    [SerializeField] private PersonData personData;
    [SerializeField] private MeshRenderer meshRenderer;
    [SerializeField] private InfoPersonUI infoPersonUI;
    private ManagerSingleton singleton;
    private AgentController agentController;
    private PersonStatus personStatus;
    private IPersonBehaviour personBehaviour;
    public PersonData PersonData => personData;
    public InfoPersonUI InfoPersonUI => infoPersonUI;
    public AgentController AgentController => agentController;
    public PersonStatus PersonStatus => personStatus;

    public IPersonBehaviour PersonBehaviour => personBehaviour;

    void Awake()
    {
        singleton = EmpireInstance;
        agentController = GetComponent<AgentController>();
    }

    void Start()
    {
        var agentType = agentController.AgentType;
        SetBehavior(agentType);
        SetRandomColor();
    }
    public void SetBehavior(AgentType agentType)
    {
        personBehaviour = agentType switch
        {
            AgentType.CUSTOMER => new CustomerBehaviour(this),
            AgentType.SERVER => new ServerBehaviour(this),
            AgentType.RECEPTIONIST => new ServerBehaviour(this),
            _ => new BaseBehaviour(this),
        };
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
        personData = new PersonData(personName, personAge);
        personStatus = new();
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
            case AgentType.RECEPTIONIST:
                return Color.green;
            // Add more cases as needed
            default:
                return Color.white;
        }
    }
    public void SwitchState(PersonState newState)
    {
        personStatus.CurrentState = newState;
        // You can add additional logic here if needed when the state changes
        OnPersonStatusChanged?.Invoke(this);
    }

}
