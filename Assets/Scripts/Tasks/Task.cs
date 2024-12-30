using System.Collections.Generic;
using UnityEngine;

public class Task
{
    private List<List<ActionHolder>> actionHoldersList;
    private bool isTaskCompleted = false;

    public bool IsTaskCompleted => isTaskCompleted;

    public Task(List<List<ActionHolder>> list)
    {
        actionHoldersList = list;
    }

    public void SetTaskCompleted(bool isTaskCompleted)
    {
        this.isTaskCompleted = isTaskCompleted;
    }
    public List<List<ActionHolder>> GetActionHolderList()
    {
        return actionHoldersList;
    }
    public Task(List<ActionHolder> actionHolders)
    {
        this.actionHoldersList.Add(actionHolders);
    }


}
