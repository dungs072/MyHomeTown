
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TaskData", menuName = "Data/Task/TaskData")]
public class TaskData : ScriptableObject
{
    [SerializeField] private string taskName;
    [SerializeField]
    private string description = null;
    [SerializeField] private List<StepData> steps;
    [SerializeField] private TaskType taskType;

    public string TaskName => taskName;
    public string Description => description;
    public List<StepData> Steps => steps;
    public TaskType TaskType => taskType;
}