
using System;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

public class TaskPerformer
{
    private Task task;

    private List<StepPerformer> stepPerformers;


    private bool isFinished = false;
    public Task Task => task;
    public bool IsFinished => isFinished;
    public void SetTask(Task task)
    {
        this.task = task;
        ParseStepIntoPerformers(task);
    }

    private void ParseStepIntoPerformers(Task task)
    {
        var steps = task.Steps;
        for (int i = 0; i < steps.Count; i++)
        {
            var step = steps[i];
            var stepPerformer = new StepPerformer(step);
            stepPerformers.Add(stepPerformer);
        }
    }

    public TaskPerformer()
    {
        stepPerformers = new List<StepPerformer>();
    }


    public StepPerformer GetFirstStepPerformer()
    {
        if (stepPerformers.Count == 0) return null;
        return stepPerformers[0];
    }

    public void RemoveFirstStepPerformer()
    {
        if (stepPerformers.Count == 0) return;
        stepPerformers.RemoveAt(0);
    }

}