using System.Collections.Generic;

//! a task will have many steps to implement
//! it has a list of step to handle and you have to use it like a queue data structure
public class Task
{

    private TaskData taskData;
    private List<Step> steps;

    public List<Step> Steps => steps;

    public TaskData TaskData => taskData;

    public Task(TaskData taskData)
    {
        this.taskData = taskData;
        steps = new List<Step>();
    }

    public void PushBack(Step step)
    {
        steps.Add(step);
    }

    public void RemoveFirstStep()
    {
        if (steps.Count == 0) return;
        steps.RemoveAt(0);
    }

    public Step GetFirstStep()
    {
        if (steps.Count == 0) return null;
        return steps[0];
    }

}