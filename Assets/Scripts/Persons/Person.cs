using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Person : MonoBehaviour
{
    [SerializeField] private List<TaskData> taskDatas;
    private List<Task> tasks;
    private AgentController agent;

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
        foreach (var task in tasks)
        {
            foreach (var stepHolders in task.GetActionHolderList())
            {
                yield return StartCoroutine(HandleMoveToHolderAndFinish(stepHolders));
            }
        }
        gameObject.SetActive(false);
    }
    private IEnumerator HandleMoveToHolderAndFinish(List<ActionHolder> actionHolders)
    {
        var personHolderData = new PersonActionHolderData();
        ActionHolder actionHolder = GetSuitableHolder(actionHolders);
        personHolderData.holder = actionHolder;
        personHolderData.isFinished = false;
        agent.Resume();
        while (!personHolderData.isFinished)
        {
            var holder = personHolderData.holder;
            if (holder == null || holder.IsBusy)
            {
                agent.Stop();
                var anotherHolder = GetSuitableHolder(actionHolders);
                if (anotherHolder)
                {
                    agent.Resume();
                    personHolderData.holder = anotherHolder;
                }
            }
            else
            {
                var destination = holder.transform.position;

                if (agent.IsReachedDestination(destination))
                {
                    holder.SetBusy(true);
                    yield return new WaitForSeconds(holder.ActionData.FinishTime);
                    personHolderData.isFinished = true;
                    holder.SetBusy(false);
                }
                else
                {
                    agent.SetDestination(destination);
                }
            }
            yield return null;
        }
    }


    private ActionHolder GetSuitableHolder(List<ActionHolder> actions)
    {
        List<ActionHolder> notBusyActions = actions.FindAll(a => !a.IsBusy);
        if (notBusyActions.Count == 0)
        {
            return null;
        }
        ActionHolder shortestPathAction = notBusyActions[0];
        float shortestDistance = Vector3.Distance(transform.position, shortestPathAction.transform.position);
        for (int i = 1; i < notBusyActions.Count; i++)
        {
            ActionHolder action = notBusyActions[i];
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
