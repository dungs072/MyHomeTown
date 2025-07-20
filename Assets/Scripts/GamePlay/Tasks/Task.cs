using System.Collections.Generic;

//! a task will have many steps to implement
//! it has a list of step to handle and you have to use it like a queue data structure
public class Task
{
    private TaskName taskName;
    private List<Step> steps;

    public List<Step> Steps => steps;
    public TaskName TaskName => taskName;

    public Task(TaskName taskName)
    {
        this.taskName = taskName;
        steps = new List<Step>();
    }

    public void PushBack(Step step)
    {
        steps.Add(step);
    }
    public void PushBack(List<Step> steps)
    {
        this.steps.AddRange(steps);
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