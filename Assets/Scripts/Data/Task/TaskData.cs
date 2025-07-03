
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TaskData", menuName = "Data/Task/TaskData")]
public class TaskData : ScriptableObject
{
    [SerializeField] private string taskName;
    [SerializeField]
    private string description = null;
    [SerializeField] private List<StepData> steps;
#if UNITY_EDITOR
    void Awake()
    {
        InitDefaultStep();
    }
#endif
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

    public string TaskName => taskName;
    public string Description => description;
    public List<StepData> Steps => steps;
}