using System.Collections;
using UnityEngine;

public class Person : MonoBehaviour
{
    [SerializeField] private MeshRenderer meshRenderer;
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
        SetRandomColor();
    }

    private void SetRandomColor()
    {
        Color randomColor = new Color(Random.value, Random.value, Random.value, 1.0f);
        meshRenderer.material.color = randomColor;
    }

    private IEnumerator DoTask()
    {
        var taskManager = singleton.TaskManager;
        var taskData = taskManager.TasksData[0];
        var taskData1 = taskManager.TasksData[1];
        var specificTask = taskManager.GetTask(taskData);
        var specificTask1 = taskManager.GetTask(taskData1);
        yield return new WaitForSeconds(5);
        taskHandler.AddTask(specificTask);
        taskHandler.AddTask(specificTask1);
        yield return StartCoroutine(taskHandler.HandleAllAssignedTask());
    }
}
