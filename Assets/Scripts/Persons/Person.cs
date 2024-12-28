using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Person : MonoBehaviour
{
    [SerializeField] private List<TaskData> taskDatas;
    private List<Task> tasks;
    private AgentController agent;

    private bool finishedMoving = false;

    private void Awake()
    {
        tasks = new List<Task>();
        agent = GetComponent<AgentController>();
    }


    private void Start()
    {
        foreach (TaskData taskData in taskDatas)
        {
            ParseTaskToRealTask(taskData);
        }
        HandleTask();

    }
    private void OnEnable()
    {
        HandleTask();
    }

    private void ParseTaskToRealTask(TaskData taskData)
    {
        List<ActionData> actionDatas = taskData.Actions;
        List<ActionHolder> actionHolders = new List<ActionHolder>();
        ActionManager actionManager = ManagerSingleton.Instance.ActionManager;
        foreach (ActionData actionData in actionDatas)
        {
            var actions = actionManager.GetPossibleActions(actionData.ActionName);
            if (actions.Count == 0)
            {
                Debug.LogError("No actions found for action name: " + actionData.ActionName);
                return;
            }
            actionHolders.Add(actions[0]);
        }
        Task task = new Task(actionHolders);
        tasks.Add(task);
    }

    private void HandleTask()
    {
        if (tasks.Count == 0) return;
        StartCoroutine(PerformTasks());
    }
    private IEnumerator PerformTasks()
    {
        for (int i = 0; i < tasks.Count; i++)
        {
            Task task = tasks[i];
            List<ActionHolder> actionHolders = task.GetActionHolders();
            for (int j = 0; j < actionHolders.Count; j++)
            {
                ActionHolder actionHolder = actionHolders[j];
                Vector3 target = actionHolder.transform.position;
                agent.SetDestination(target);
                StartCoroutine(ProcessWhileMoving(target, actionHolder));
                yield return new WaitUntil(() => finishedMoving);
                actionHolder.SetBusy(true);
                float timeToComplete = actionHolder.ActionData.FinishTime;
                yield return new WaitForSeconds(timeToComplete);
                actionHolder.SetBusy(false);
            }

        }
        gameObject.SetActive(false);

    }

    private IEnumerator ProcessWhileMoving(Vector3 target, ActionHolder actionHolder)
    {
        finishedMoving = false;
        while (!agent.IsReachedDestination(target))
        {
            if (actionHolder.IsBusy)
            {
                agent.Stop();
            }
            else
            {
                agent.SetDestination(target);
            }
            yield return null;
        }
        finishedMoving = true;
    }
}
