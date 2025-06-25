using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static ManagerSingleton;

[RequireComponent(typeof(Person))]
public class BaseCharacter : MonoBehaviour
{
    [SerializeField] private List<TaskData> tasksData;

    protected Person person;
    protected TaskHandler taskHandler;

    protected List<NeedObject> needObjects;

    void Awake()
    {
        InitComponents();
    }

    protected virtual void InitComponents()
    {
        person = GetComponent<Person>();
        taskHandler = GetComponent<TaskHandler>();
    }

    void Start()
    {
        StartCoroutine(HandlePreTasks());
    }
    protected virtual IEnumerator HandlePreTasks()
    {
        yield return new WaitForSeconds(10f);
        var taskManager = EmpireInstance.TaskManager;
        foreach (var taskData in tasksData)
        {
            var task = taskManager.TasksDict[taskData];
            if (task == null)
            {
                Debug.LogError($"Task {taskData.TaskName} not found in TaskManager.");
                continue;
            }
            yield return DoTask(task);
        }
        OnAllTasksCompleted();
    }
    public IEnumerator DoTask(Task task)
    {
        taskHandler.AddTask(task);
        HandleAddAdditionalItemRequired();
        yield return taskHandler.HandleAllAssignedTask();
        taskHandler.RemoveTask(task);

    }
    protected virtual void HandleAddAdditionalItemRequired()
    {
        var taskPerformers = taskHandler.TaskPerformers;
        if (taskPerformers.Count == 0) return;
        var newestTask = taskPerformers[taskPerformers.Count - 1];
        //! Todo
    }
    protected virtual void OnAllTasksCompleted()
    {
        gameObject.SetActive(false);
    }

    public void AddNeedObject(NeedItemData item, int neededAmount)
    {
        needObjects ??= new List<NeedObject>();

        var needObject = new NeedObject
        {
            itemData = item,
            neededAmount = neededAmount,
            gainedAmount = 0
        };

        needObjects.Add(needObject);
    }

}
