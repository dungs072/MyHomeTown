
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TaskData", menuName = "Data/Task/TaskData")]
public class TaskData : ScriptableObject
{
    [SerializeField] private string taskName;
    [SerializeField]
    private string description = null;
    [SerializeField] private List<StepData> steps;
    private Dictionary<StepData, List<StepData>> stepsDictionary;
    private StepData rootStep;
    void Awake()
    {
        InitRootStep();
        InitDefaultStep();
        InitDefaultData();
    }
    private void InitDefaultStep()
    {
        if (steps.Count > 0) return;
        steps = new List<StepData>
        {
            new ()
            {
                StepName = "Default Step",
                Description = "This is a default step.",
                Duration = 0,
                WorkContainerType = WorkContainerType.COOKING_STATION,
            }
        };

    }
    void OnValidate()
    {
        InitDefaultData();
    }
    private void InitDefaultData()
    {
        stepsDictionary = new Dictionary<StepData, List<StepData>>();
        Debug.Log($"<color=#488b84>stepsDictionary: {stepsDictionary}</color>");

        foreach (var step in steps)
        {
            var stepList = new List<StepData>();
            stepsDictionary.Add(step, stepList);
            foreach (var anotherStep in steps)
            {
                if (anotherStep.UniqueID == step.UniqueID) continue;
                if (step.Children.Contains(anotherStep.UniqueID))
                {
                    stepList.Add(anotherStep);
                }
            }
        }
    }
    private void InitRootStep()
    {
        foreach (var step in steps)
        {
            var isRoot = true;
            foreach (var otherStep in steps)
            {
                if (step == otherStep) continue;
                if (otherStep.Children.Contains(step.UniqueID))
                {
                    isRoot = false;
                    break;
                }
            }
            if (isRoot)
            {
                rootStep = step;
                break;
            }
        }
    }
    public void CreateStep(StepData parentStep, Vector2 position)
    {
        var newStep = new StepData
        {
            StepName = "New Step",
            Description = "This is a new step.",
            Duration = 0,
            WorkContainerType = WorkContainerType.COOKING_STATION,
            Position = position
        };
        AddStep(newStep);
        if (parentStep != null)
        {
            LinkStep(parentStep, newStep);
        }
    }
    public void AddStep(StepData step)
    {
        if (steps.Contains(step))
        {
            Debug.LogWarning("Step already exists in the task.");
            return;
        }
        steps.Add(step);
        stepsDictionary[step] = new List<StepData>();
    }
    public void RemoveStep(StepData step)
    {
        if (!steps.Contains(step))
        {
            Debug.LogWarning("Step does not exist in the task.");
            return;
        }
        foreach (var kvp in stepsDictionary)
        {
            kvp.Value.Remove(step);
            kvp.Key.TryToRemoveChild(step.UniqueID);
        }

        // Remove this step from the dictionary (as a key)
        stepsDictionary.Remove(step);

        // Remove from main list
        steps.Remove(step);
    }
    public void LinkStep(StepData parent, StepData child)
    {
        if (stepsDictionary.ContainsKey(parent) && stepsDictionary[parent].Contains(child))
        {
            Debug.LogWarning("Step already linked.");
            return;
        }
        if (!stepsDictionary.ContainsKey(parent))
        {
            stepsDictionary[parent] = new List<StepData>();
        }
        var result = parent.TryToAddChild(child.UniqueID);
        if (!result) return;
        stepsDictionary[parent].Add(child);
    }
    public void UnlinkStep(StepData parent, StepData child)
    {
        if (!stepsDictionary.ContainsKey(parent) || !stepsDictionary[parent].Contains(child))
        {
            Debug.LogWarning("Step not linked.");
            return;
        }
        var result = parent.TryToRemoveChild(child.UniqueID);
        if (!result) return;
        stepsDictionary[parent].Remove(child);
    }


    public string TaskName => taskName;
    public string Description => description;
    public List<StepData> Steps => steps;
    public Dictionary<StepData, List<StepData>> StepsDictionary => stepsDictionary;
    public StepData RootStep => rootStep;
}