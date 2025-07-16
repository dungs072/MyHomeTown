using System;
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
    public static event Action<Person> OnPersonStatusChanged;

    [SerializeField] private PersonData personData;
    [SerializeField] private MeshRenderer meshRenderer;
    [SerializeField] private InfoPersonUI infoPersonUI;
    private ManagerSingleton singleton;
    private AgentController agentController;
    private PersonStatus personStatus;

    public PersonData PersonData => personData;
    public InfoPersonUI InfoPersonUI => infoPersonUI;


    public AgentController AgentController => agentController;
    public PersonStatus PersonStatus => personStatus;

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

    public void TakeNeedItems(Dictionary<ItemKey, int> items)
    {
        var needItems = GetNeedItemsFromCurrentToEndStep();
        if (needItems == null || needItems.Count == 0) return;
        foreach (var needItem in needItems)
        {
            var itemKey = needItem.itemData.itemKey;
            var requiredAmount = needItem.itemData.amount;
            if (items.TryGetValue(itemKey, out int amount))
            {
                var gainedAmount = Mathf.Min(amount, requiredAmount);
                needItem.gainedAmount += gainedAmount;
                items[itemKey] -= gainedAmount;
            }
        }
    }
    //! must edit the way to get step of specific task
    //! temporary get index = 0
    private List<GatheredItem> GetNeedItemsFromCurrentToEndStep()
    {
        List<GatheredItem> items = new();
        var currentTask = personStatus.CurrentTaskPerformer;
        var currentStep = currentTask.GetCurrentStepPerformer();
        if (currentStep == null) return null;
        for (int i = currentTask.CurrentStepIndex; i < currentTask.StepPerformers.Count; i++)
        {
            var step = currentTask.StepPerformers[i];
            if (step == null) continue;
            var stepNeedItems = step.NeedItems;
            if (stepNeedItems == null || stepNeedItems.Count == 0) continue;
            items.AddRange(stepNeedItems);
        }
        return items;
    }
}
