using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Person : MonoBehaviour
{
    [SerializeField] private List<TaskData> taskDatas;
    private List<Task> tasks;
    private AgentController agent;

    private bool finishedMoving = false;

    private ActionHolder currentActionHolder;

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
            Task task = tasks[i];
            var actionHoldersList = task.GetActionHolderList();
            for (int j = 0; j < actionHoldersList.Count; j++)
            {
                var actionHolders = actionHoldersList[j];
                ActionHolder shortestActionHolder = null;
                while (!shortestActionHolder)
                {
                    agent.Stop();
                    shortestActionHolder = GetShortestPathToActionHolder(actionHolders);
                    Debug.LogWarning("run run");
                    yield return new WaitForEndOfFrame();

                }
                agent.Resume();

                currentActionHolder = shortestActionHolder;

                Vector3 target = currentActionHolder.transform.position;
                agent.SetDestination(target);
                StartCoroutine(ProcessWhileMoving(actionHolders));
                yield return new WaitUntil(() => finishedMoving);
                currentActionHolder.SetBusy(true);
                float timeToComplete = currentActionHolder.ActionData.FinishTime;
                yield return new WaitForSeconds(timeToComplete);
                Debug.LogWarning("finish");
                currentActionHolder.SetBusy(false);
            }

        }
        gameObject.SetActive(false);

    }

    private IEnumerator ProcessWhileMoving(List<ActionHolder> actions)
    {
        finishedMoving = false;
        var target = currentActionHolder.transform.position;
        while (!agent.IsReachedDestination(target))
        {
            if (currentActionHolder.IsBusy)
            {
                currentActionHolder = GetShortestPathToActionHolder(actions);
                if (currentActionHolder == null)
                {
                    agent.Stop();
                }
                else
                {
                    target = currentActionHolder.transform.position;
                    agent.Resume();
                }

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
        if (shortestPathAction.IsBusy)
        {
            return null;
        }
        return shortestPathAction;
    }
}
