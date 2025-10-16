using System.Collections.Generic;
using UnityEngine;
using static ManagerSingleton;
[RequireComponent(typeof(Person))]
public class TaskHandler : MonoBehaviour
{
    [SerializeField] private List<TaskName> taskNames = new();
    [SerializeField] private bool isAutoDoTaskAgain = false;
    private TaskPerformer taskPerformer;
    private int currentTaskIndex = 0;
    private Person person;

    public TaskPerformer CurrentTaskPerformer => taskPerformer;
    public StepPerformer CurrentStepPerformer => taskPerformer?.CurStep;
    public bool IsFree()
    {
        return person.PersonStatus.CurrentTaskPerformer == null;
    }
    void Awake()
    {
        person = GetComponent<Person>();
    }

    private void OnEnable()
    {
        if (taskNames.Count == 0) return;
        InitNewTask();
    }

    private void InitNewTask()
    {
        if (currentTaskIndex == -1)
        {
            taskPerformer = null;
            return;
        }
        var taskManager = EmpireInstance.TaskManager;
        var task = taskManager.TasksDict[taskNames[currentTaskIndex]];
        if (task == null) return;
        taskPerformer = new TaskPerformer();
        taskPerformer.SetTask(task);
        EventBus.Publish(GameEvents.TaskHandlerEvents.OnTaskAvailable, taskPerformer);
    }
    public void AddTask(TaskName taskName)
    {
        var taskManager = EmpireInstance.TaskManager;
        var task = taskManager.TasksDict[taskName];
        if (task == null) return;
        var personStatus = person.PersonStatus;
        var currentTask = personStatus.CurrentTaskPerformer;
        if (currentTask == null)
        {
            personStatus.CurrentTaskPerformer = new TaskPerformer();
            person.PersonStatus.CurrentTaskPerformer.SetTask(task);
        }
        else
        {
            taskNames.Add(taskName);
        }
    }

    public void MoveNextTask()
    {

        //? just for test
        currentTaskIndex++;
        if (currentTaskIndex >= taskNames.Count)
        {
            if (isAutoDoTaskAgain)
            {
                currentTaskIndex = 0;
            }
            else
            {
                currentTaskIndex = -1;
            }
        }
        InitNewTask();
    }


}