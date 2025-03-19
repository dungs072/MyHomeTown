using System.Collections;
using UnityEngine;

public class Person : MonoBehaviour
{
    private TaskHandler taskHandler;
    private ManagerSingleton singleton;
    void Awake()
    {
        singleton = ManagerSingleton.Instance;
        taskHandler = GetComponent<TaskHandler>();

    }

    void Start()
    {
        StartCoroutine(DoTask());
    }

    private IEnumerator DoTask()
    {
        var taskManager = singleton.TaskManager;
        var taskData = taskManager.TasksData[0];
        var specificTask = taskManager.GetTask(taskData);
        yield return new WaitForSeconds(5);
        taskHandler.AddTask(specificTask);
        yield return StartCoroutine(taskHandler.HandleFirstTask());
        this.gameObject.SetActive(false);
    }
}
