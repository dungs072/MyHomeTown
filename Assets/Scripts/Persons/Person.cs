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
        List<List<ActionHolder>> actionHoldersList = new List<List<ActionHolder>>();
        ActionManager actionManager = ManagerSingleton.Instance.ActionManager;
        foreach (ActionData actionData in actionDatas)
        {
            var actions = actionManager.GetPossibleActions(actionData.ActionName);
            if (actions.Count == 0)
            {
                Debug.LogError("No actions found for action name: " + actionData.ActionName);
                return;
            }
            //ActionHolder action = GetShortestPathToActionHolder(actions);
            actionHoldersList.Add(actions);
        }
        Task task = new Task(actionHoldersList);
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

            //! need to fix there
            // Task task = tasks[i];
            // List<ActionHolder> actionHolders = task.GetActionHolderList();
            // for (int j = 0; j < actionHolders.Count; j++)
            // {
            //     ActionHolder actionHolder = actionHolders[j];
            //     Vector3 target = actionHolder.transform.position;
            //     agent.SetDestination(target);
            //     StartCoroutine(ProcessWhileMoving(target, actionHolder));
            //     yield return new WaitUntil(() => finishedMoving);
            //     actionHolder.SetBusy(true);
            //     float timeToComplete = actionHolder.ActionData.FinishTime;
            //     yield return new WaitForSeconds(timeToComplete);
            //     actionHolder.SetBusy(false);
            // }

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

    private ActionHolder GetShortestPathToActionHolder(List<ActionHolder> actions)
    {
        ActionHolder shortestPathAction = actions[0];
        float shortestDistance = Vector3.Distance(transform.position, shortestPathAction.transform.position);
        print(actions.Count);
        List<ActionHolder> notBusyActions = actions.FindAll(a => !a.IsBusy);
        for (int i = 1; i < notBusyActions.Count; i++)
        {
            ActionHolder action = actions[i];
            float distance = Vector3.Distance(transform.position, action.transform.position);
            if (distance < shortestDistance)
            {
                shortestDistance = distance;
                shortestPathAction = action;
            }
        }
        return shortestPathAction;
    }
}
