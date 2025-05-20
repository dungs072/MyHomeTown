using System.Collections.Generic;
using UnityEngine;
// remove all task and step pls. Update new logic with none of task or step there
public class TaskManager : MonoBehaviour
{
    [Header("Tasks")]
    [SerializeField] private List<TaskData> tasksData;

    private Dictionary<TaskData, Task> tasks;
    private ManagerSingleton singleton;

    public List<TaskData> TasksData => tasksData;
    void Awake()
    {
        tasks = new Dictionary<TaskData, Task>();
        WorkContainerManager.OnWorkContainerAdded += HandleAddWorkContainer;
        WorkContainerManager.OnWorkContainerRemoved += HandleRemoveWorkContainer;

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
        TransformTasks();
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
            var task = new Task(taskData);
            foreach (var stepData in taskData.Steps)
            {
                var step = new Step(stepData);
                task.PushBack(step);
            }
            tasks.Add(taskData, task);
        }
    }
    public void AddWorkContainer(WorkContainer workContainer)
    {
        if (tasks.Count == 0)
        {
            Debug.LogWarning("No task to add work container");
            return;
        }

        foreach (var task in tasks)
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
        if (tasks.Count == 0)
        {
            Debug.LogWarning("No task to remove work container");
            return;
        }
        foreach (var task in tasks)
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

    public Task GetTask(TaskData taskData)
    {
        if (tasks.ContainsKey(taskData))
        {
            return tasks[taskData];
        }
        return null;
    }
}
