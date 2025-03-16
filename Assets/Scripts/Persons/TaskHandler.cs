using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEngine;
//! This will handle all the tasks that are assigned to a person    
[RequireComponent(typeof(AgentController))]
public class TaskHandler : MonoBehaviour
{
    private List<TaskPerformer> taskPerformers;
    private AgentController agent;
    private void Awake()
    {
        taskPerformers = new List<TaskPerformer>();
        agent = GetComponent<AgentController>();
    }

    public void AddTask(Task task)
    {
        var taskPerformer = new TaskPerformer();
        taskPerformer.SetTask(task);
        taskPerformers.Add(taskPerformer);
    }

    public void RemoveTask(Task task)
    {
        for (int i = 0; i < taskPerformers.Count; i++)
        {
            var taskPerformer = taskPerformers[i];
            if (taskPerformer.Task == task)
            {
                taskPerformers.RemoveAt(i);
                return;
            }
        }
    }

    public IEnumerator HandleFirstTask()
    {
        if (taskPerformers.Count == 0) yield break;
        var taskPerformer = taskPerformers[0];
        while (!taskPerformer.IsFinished)
        {
            var stepPerformer = taskPerformer.GetFirstStepPerformer();
            if (stepPerformer == null) yield break;
            var step = stepPerformer.Step;
            var workContainer = GetTheShortestWorkContainer(step);
            if (workContainer == null) yield break;
            while (!stepPerformer.IsFinished)
            {
                agent.SetDestination(workContainer.transform.position);
                if (Utils.HasSamePosition(transform.position, workContainer.transform.position))
                {
                    stepPerformer.SetIsFinished(true);
                }
                yield return null;
            }
            yield return null;
            taskPerformer.RemoveFirstStepPerformer();

        }
    }

    private WorkContainer GetTheShortestWorkContainer(Step step)
    {
        var places = step.WorkContainers;

        if (places.Count == 0) return null;
        WorkContainer shortestPlace = places[0];
        float shortestDistance = float.MaxValue;
        for (int i = 0; i < places.Count; i++)
        {
            var place = places[i];
            var distance = Vector3.SqrMagnitude(place.transform.position - agent.transform.position);
            if (distance < shortestDistance)
            {
                shortestDistance = distance;
                shortestPlace = place;
            }
        }
        return shortestPlace;
    }




}