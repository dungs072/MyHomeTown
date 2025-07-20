using System.Collections.Generic;
using UnityEngine;
// remove all task and step pls. Update new logic with none of task or step there
public class TaskManager : MonoBehaviour
{
    [Header("Tasks")]
    [SerializeField] private List<TaskData> tasksData;

    private Dictionary<TaskName, Task> tasksDict;
    private ManagerSingleton singleton;
    public Dictionary<TaskName, Task> TasksDict => tasksDict;
    void Awake()
    {
        tasksDict = new Dictionary<TaskName, Task>();
        WorkContainerManager.OnWorkContainerAdded += HandleAddWorkContainer;
        WorkContainerManager.OnWorkContainerRemoved += HandleRemoveWorkContainer;
        TransformTasks();
    }

    void OnDestroy()
    {
        WorkContainerManager.OnWorkContainerAdded -= HandleAddWorkContainer;
        WorkContainerManager.OnWorkContainerRemoved -= HandleRemoveWorkContainer;
    }
    void Start()
    {
        singleton = ManagerSingleton.EmpireInstance;
        //? just transform from the data to real tasks

        AddExistingWorkContainers();
    }
    private void AddExistingWorkContainers()
    {
        var workContainers = singleton.WorkContainerManager.WorkContainers;
        foreach (var workContainer in workContainers)
        {
            AddWorkContainer(workContainer);
        }
    }

    private void HandleAddWorkContainer(WorkContainer workContainer)
    {
        AddWorkContainer(workContainer);
    }

    private void HandleRemoveWorkContainer(WorkContainer workContainer)
    {
        //TODO : remove work container from the task
    }


    private void TransformTasks()
    {
        foreach (var taskData in tasksData)
        {
            var rootStepData = taskData.RootStep;
            if (rootStepData == null)
            {
                Debug.LogWarning($"Task {taskData.name} has no root step. Skipping task creation.");
                continue;
            }
            var stack = new Stack<(Step, List<Step>)>();
            var rootStep = new Step(rootStepData);
            stack.Push((rootStep, new List<Step>() { rootStep }));

            while (stack.Count > 0)
            {
                var (currentStep, previousSteps) = stack.Pop();
                var stepData = currentStep.Data;
                var childrenSteps = taskData.StepsDictionary[stepData];

                if (childrenSteps == null || childrenSteps.Count == 0)
                {
                    var taskName = GetTaskNameFromList(previousSteps);
                    if (!tasksDict.ContainsKey(taskName))
                    {
                        var newTask = new Task(taskName);
                        newTask.PushBack(previousSteps);
                        tasksDict[taskName] = newTask;
                        DebugTask(newTask);
                    }
                }
                else
                {
                    for (int i = 0; i < childrenSteps.Count; i++)
                    {
                        var childStepData = childrenSteps[i];
                        var childStep = new Step(childStepData);
                        var newSteps = new List<Step>(previousSteps) { childStep };
                        stack.Push((childStep, newSteps));
                    }
                }

            }

        }
    }
    private TaskName GetTaskNameFromList(List<Step> steps)
    {
        if (steps == null || steps.Count == 0)
        {
            return TaskName.NONE;
        }
        foreach (var step in steps)
        {
            if (step.Data.TaskName != TaskName.NONE)
            {
                return step.Data.TaskName;
            }
        }
        return TaskName.NONE;
    }
    private void DebugTask(Task task)
    {
        var stringBuilder = new System.Text.StringBuilder();
        stringBuilder.AppendLine($"Task: {task.TaskName.ToName()}");
        foreach (var step in task.Steps)
        {
            stringBuilder.Append($" Step: {step.Data.StepName},");
        }
        Debug.Log(stringBuilder.ToString());
    }

    public void AddWorkContainer(WorkContainer workContainer)
    {
        if (tasksDict.Count == 0)
        {
            Debug.LogWarning("No task to add work container");
            return;
        }

        foreach (var task in tasksDict)
        {
            foreach (var step in task.Value.Steps)
            {
                if (step.WorkContainerType == workContainer.WorkContainerType)
                {
                    step.AddWorkContainer(workContainer);
                }
            }
        }
    }

    public void RemoveWorkContainer(WorkContainer workContainer)
    {
        if (tasksDict.Count == 0)
        {
            Debug.LogWarning("No task to remove work container");
            return;
        }
        foreach (var task in tasksDict)
        {
            foreach (var step in task.Value.Steps)
            {
                if (step.WorkContainerType == workContainer.WorkContainerType)
                {
                    step.WorkContainers.Remove(workContainer);
                }
            }
        }
    }

    public Task GetTask(TaskName taskName)
    {
        if (tasksDict.ContainsKey(taskName))
        {
            return tasksDict[taskName];
        }
        return null;
    }
}
