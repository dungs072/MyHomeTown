using System.Collections.Generic;
using UnityEngine;
using static ManagerSingleton;
[RequireComponent(typeof(Person))]
public class TaskHandler : MonoBehaviour
{
    [SerializeField] private List<TaskName> taskNames = new();

    public List<TaskName> TaskNames => taskNames;
    private int currentTaskIndex = 0;

    private Person person;

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
        SetInitTasks();
    }

    private void SetInitTasks()
    {
        // temporary code to set initial tasks
        var taskManager = EmpireInstance.TaskManager;
        var task = taskManager.TasksDict[taskNames[currentTaskIndex]];
        if (task == null) return;
        person.PersonStatus.CurrentTaskPerformer = new TaskPerformer();
        person.PersonStatus.CurrentTaskPerformer.SetTask(task);
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
        currentTaskIndex++;
        if (currentTaskIndex >= taskNames.Count)
        {
            currentTaskIndex = 0;
            person.PersonStatus.CurrentTaskPerformer = null;
        }
        else
        {
            CreateNewTask();
        }
    }
    public void CreateNewTask()
    {
        person.PersonStatus.CurrentTaskPerformer = new TaskPerformer();
        var taskManager = EmpireInstance.TaskManager;
        var task = taskManager.TasksDict[taskNames[currentTaskIndex]];
        person.PersonStatus.CurrentTaskPerformer.SetTask(task);
    }


}