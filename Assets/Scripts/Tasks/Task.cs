using System.Collections.Generic;
using UnityEngine;

public class Task
{
    private List<ActionHolder> actionHolders;
    private int currentActionIndex = 0;
    private bool isTaskCompleted = false;

    public bool IsTaskCompleted => isTaskCompleted;
    public void SetTaskCompleted(bool isTaskCompleted)
    {
        this.isTaskCompleted = isTaskCompleted;
    }
    public List<ActionHolder> GetActionHolders()
    {
        return actionHolders;
    }
    public Task(List<ActionHolder> actionHolders)
    {
        this.actionHolders = actionHolders;
    }


}
