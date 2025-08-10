
using System;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

public class TaskPerformer
{
    private Task task;

    private List<StepPerformer> stepPerformers;
    private int currentStepIndex = 0;
    public Task Task => task;
    public int CurrentStepIndex => currentStepIndex;
    public List<StepPerformer> StepPerformers => stepPerformers;

    public StepPerformer GetCurrentStepPerformer()
    {
        return stepPerformers[currentStepIndex];
    }
    public void MoveToNextStep()
    {
        currentStepIndex++;
    }
    public void ResetTask()
    {
        currentStepIndex = 0;
    }
    public bool IsFinished()
    {
        return currentStepIndex >= stepPerformers.Count;
    }
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
            var needObjects = step.Data.NeedItems;
            if (needObjects == null || needObjects.Count == 0) continue;
            foreach (var needObject in needObjects)
            {
                stepPerformer.AddNeedObject(needObject);
            }
        }
    }

    public TaskPerformer()
    {
        stepPerformers = new List<StepPerformer>();
    }




}