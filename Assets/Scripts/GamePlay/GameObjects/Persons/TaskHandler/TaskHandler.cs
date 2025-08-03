using System.Collections.Generic;
using UnityEngine;
using static ManagerSingleton;
[RequireComponent(typeof(Person))]
public class TaskHandler : MonoBehaviour
{
    [SerializeField] private List<TaskName> taskNames;

    public List<TaskName> TaskNames => taskNames;
    private int currentTaskIndex = 0;

    private Person person;
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
        person.PersonStatus.CurrentState = PersonState.IDLE;
    }

    public void MoveNextTask()
    {
        currentTaskIndex++;
        if (currentTaskIndex >= taskNames.Count)
        {
            person.SwitchState(PersonState.IDLE);
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