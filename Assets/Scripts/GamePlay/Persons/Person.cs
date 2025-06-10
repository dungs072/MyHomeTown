using System.Collections;
using UnityEngine;

public class Person : MonoBehaviour
{
    [SerializeField] private MeshRenderer meshRenderer;
    private TaskHandler taskHandler;
    private ManagerSingleton singleton;
    private AgentController agentController;


    void Awake()
    {
        singleton = ManagerSingleton.EmpireInstance;
        taskHandler = GetComponent<TaskHandler>();
        agentController = GetComponent<AgentController>();
    }

    void Start()
    {
        StartCoroutine(BehaveLikeNormalPerson());
        SetRandomColor();
    }

    private void SetRandomColor()
    {
        Color randomColor = new Color(Random.value, Random.value, Random.value, 1.0f);
        meshRenderer.material.color = randomColor;
    }

    private IEnumerator BehaveLikeNormalPerson()
    {
        yield return FollowPath();
        yield return DoTask();
    }

    private IEnumerator FollowPath()
    {
        var patrollingSystem = singleton.PatrollingSystem;
        var patrollingPath = patrollingSystem.PathDictionary[PatrollingPathKey.DefaultPath];
        if (patrollingPath == null)
        {
            Debug.LogWarning("Patrolling path not found!");
            yield break;
        }
        var points = patrollingPath.Waypoints;
        for (int i = 0; i < points.Length; i++)
        {
            var position = points[i].position;
            yield return agentController.MoveToPosition(position);
        }
    }

    private IEnumerator DoTask()
    {
        var taskManager = singleton.TaskManager;
        var taskData = taskManager.TasksData[0];
        var specificTask = taskManager.GetTask(taskData);
        taskHandler.AddTask(specificTask);
        yield return taskHandler.HandleAllAssignedTask();
        taskHandler.RemoveTask(specificTask);
        gameObject.SetActive(false);
    }
}
