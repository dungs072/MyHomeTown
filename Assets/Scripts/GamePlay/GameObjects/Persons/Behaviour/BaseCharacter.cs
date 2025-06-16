using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static ManagerSingleton;

[RequireComponent(typeof(Person))]
public class BaseCharacter : MonoBehaviour
{
    [SerializeField] private List<TaskData> tasksData;

    private Person person;

    void Awake()
    {
        person = GetComponent<Person>();
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
            yield return person.DoTask(task);
        }
        OnAllTasksCompleted();
    }
    protected virtual void OnAllTasksCompleted()
    {
        gameObject.SetActive(false);
    }

}
