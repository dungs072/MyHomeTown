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
    private Movement movement;
    private PersonStatus personStatus;
    private Pack pack;
    private BaseBehavior personBehavior;

    public PersonData PersonData => personData;
    public InfoPersonUI InfoPersonUI => infoPersonUI;
    public Movement Movement => movement;
    public PersonStatus PersonStatus => personStatus;
    public Pack Pack => pack;

    public BaseBehavior PersonBehavior => personBehavior;

    void Awake()
    {
        InitComponents();
        RegisterEvents();
        InitBehavior();
    }
    private void InitComponents()
    {
        singleton = EmpireInstance;
        movement = GetComponent<Movement>();
        pack = new Pack(100);
        personStatus = new();
    }
    private void RegisterEvents()
    {
        Pack.OnPackChanged += HandlePackChanged;
    }
    private void InitBehavior()
    {
        var agentType = movement.AgentType;
        SetBehavior(agentType);
    }
    void OnDestroy()
    {
        UnregisterEvents();
    }
    private void UnregisterEvents()
    {
        Pack.OnPackChanged -= HandlePackChanged;
    }

    private void HandlePackChanged()
    {
        //? Update the UI or any other logic when the pack changes     
        var stringBuilder = new System.Text.StringBuilder();

        foreach (var item in pack.Items)
        {
            var itemName = ItemKeyNames.ToName(item);
            stringBuilder.AppendLine($"{itemName} x{pack.GetAmount(item)}");
        }

        infoPersonUI.SetInfoText(stringBuilder.ToString());
    }

    void Start()
    {
        SetRandomColor();
    }


    public void SetBehavior(AgentType agentType)
    {
        personBehavior = agentType switch
        {
            AgentType.CUSTOMER => new BaseBehavior(this),
            AgentType.SERVER => new BaseBehavior(this),
            AgentType.RECEPTIONIST => new BaseBehavior(this),
            _ => new BaseBehavior(this),
        };
    }

    void OnEnable()
    {
        CreatePersonData();
    }
    // reset the person here to reuse it again
    void OnDisable()
    {
        movement.Stop();
    }
    private void CreatePersonData()
    {
        string personName = PersonDataGenerator.GenerateName();
        int personAge = PersonDataGenerator.GenerateAge();
        personData = new PersonData(personName, personAge);
    }

    private void SetRandomColor()
    {
        var agentType = movement.AgentType;
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


}
