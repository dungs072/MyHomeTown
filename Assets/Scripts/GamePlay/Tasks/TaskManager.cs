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
            var task = new Task(taskData);
            var rootStepData = taskData.RootStep;
            if (rootStepData == null)
            {
                Debug.LogWarning($"Task {taskData.name} has no root step. Skipping task creation.");
                continue;
            }
            var queue = new Queue<Step>();
            var rootStep = new Step(rootStepData);
            queue.Enqueue(rootStep);
            task.PushBack(rootStep);

            while (queue.Count > 0)
            {
                var currentStep = queue.Dequeue();
                var stepData = currentStep.Data;
                var childrenSteps = taskData.StepsDictionary[stepData];

                if (stepData.TaskName != TaskName.NONE)
                {

                }
                foreach (var childStepData in childrenSteps)
                {
                    var childStep = new Step(childStepData);
                    queue.Enqueue(childStep);
                }
            }

        }
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
