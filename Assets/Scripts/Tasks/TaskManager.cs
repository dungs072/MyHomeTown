using System.Collections.Generic;
using UnityEngine;

public class TaskManager : MonoBehaviour
{
    [SerializeField] private List<TaskData> tasks;

    private Dictionary<TaskType, List<WorkContainer>> workContainersByTaskType;
    private Dictionary<TaskType, Task> tasksByType;
    void Awake()
    {
        workContainersByTaskType = new Dictionary<TaskType, List<WorkContainer>>();
        tasksByType = new Dictionary<TaskType, Task>();
    }
    void Start()
    {
        //? just transform from the data to real tasks
        TransformTasks();
    }
    private void TransformTasks()
    {
        for (int idx = 0; idx < tasks.Count; idx++)
        {
            TaskData taskData = tasks[idx];
            TaskType taskType = taskData.TaskType;
            Task task = new Task();
            //? set props of the task there
            for (int stepIdx = 0; stepIdx < taskData.Steps.Count; stepIdx++)
            {
                StepData stepData = taskData.Steps[stepIdx];
                Step step = new Step(stepData);
                List<WorkContainer> workContainers = workContainersByTaskType[taskType];
                if (workContainers == null)
                {
                    Debug.LogError("No work container for task type: " + taskType);
                    continue;
                }
                step.SetWorkContainers(workContainers);
                task.PushBack(step);
            }
            tasksByType.Add(taskType, task);
        }
    }


    public void AddWorkContainer(WorkContainer workContainer)
    {
        foreach (var taskType in workContainer.TaskTypes)
        {
            if (!workContainersByTaskType.ContainsKey(taskType))
            {
                workContainersByTaskType.Add(taskType, new List<WorkContainer>());
            }
            workContainersByTaskType[taskType].Add(workContainer);
        }
    }

    public void RemoveWorkContainer(WorkContainer workContainer)
    {
        foreach (var taskType in workContainer.TaskTypes)
        {
            if (workContainersByTaskType.ContainsKey(taskType))
            {
                workContainersByTaskType[taskType].Remove(workContainer);
            }
        }
    }
}
