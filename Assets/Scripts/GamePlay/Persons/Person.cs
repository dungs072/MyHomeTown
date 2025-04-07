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
        var specificTask = taskManager.GetTask(taskData);
        yield return new WaitForSeconds(5);
        taskHandler.AddTask(specificTask);
        yield return StartCoroutine(taskHandler.HandleAllAssignedTask());
        gameObject.SetActive(false);
        taskHandler.RemoveTask(specificTask);
    }
}
