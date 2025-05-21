using System.Collections;
using UnityEngine;

public class Person : MonoBehaviour
{
    [SerializeField] private MeshRenderer meshRenderer;
    private TaskHandler taskHandler;
    private ManagerSingleton singleton;


    void Awake()
    {
        singleton = ManagerSingleton.EmpireInstance;
        taskHandler = GetComponent<TaskHandler>();

    }

    void OnEnable()
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
        yield return new WaitForSeconds(5);
        var specificTask = taskManager.GetTask(taskData);
        taskHandler.AddTask(specificTask);
        yield return StartCoroutine(taskHandler.HandleAllAssignedTask());
        gameObject.SetActive(false);
        taskHandler.RemoveTask(specificTask);
    }
}
