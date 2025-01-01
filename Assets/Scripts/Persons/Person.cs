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
    // first case: is find the best actionHolder to perform the task
    // second case: while moving, the action holder can be occupied by another person
    // third case: two small case there: return to the first case 
    // if another best action holder is still available 
    // if not, wait for the action holder to be free    
    private IEnumerator PerformTasks()
    {
        foreach (var task in this.tasks)
        {
            var actionHoldersList = task.GetActionHolderList();
            yield return StartCoroutine(HandlePerformTaskAndMove(actionHoldersList));

        }
    }
    private IEnumerator HandlePerformTaskAndMove(List<List<ActionHolder>> actionHoldersList)
    {
        foreach (var actionHolders in actionHoldersList)
        {
            yield return StartCoroutine(HandlePerformAction(actionHolders));
        }
    }

    private IEnumerator HandlePerformAction(List<ActionHolder> actionHolders)
    {
        var suitableActionHolder = GetShortestPathToActionHolder(actionHolders);
        do
        {
            agent.Stop();
            yield return new WaitUntil(() => GetShortestPathToActionHolder(actionHolders) != null);
            suitableActionHolder = GetShortestPathToActionHolder(actionHolders);
        } while (!suitableActionHolder);

        agent.Resume();
        agent.SetDestination(suitableActionHolder.transform.position);
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

    private ActionHolder getFreeActionHolder(List<ActionHolder> actionHolders)
    {
        return actionHolders.Find(a => !a.IsBusy);
    }
}
