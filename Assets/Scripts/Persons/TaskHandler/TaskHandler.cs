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
            var workContainer = GetTheShortestFreeWorkContainer(step);
            while (workContainer == null)
            {
                Debug.Log("No free work container");
                var waitingLinePos = GetWaitingPosition(step);
                agent.SetDestination(waitingLinePos);
                workContainer = GetTheShortestFreeWorkContainer(step);
                yield return null;
            }
            var waitingLineWk = step.WorkContainers[0];
            waitingLineWk.RemovePersonFromWaitingLine(gameObject);
            while (!stepPerformer.IsFinished)
            {
                while (workContainer == null || !workContainer.IsFreeToUse(gameObject))
                {
                    var waitingLinePos = GetWaitingPosition(step);
                    agent.SetDestination(waitingLinePos);
                    workContainer = GetTheShortestFreeWorkContainer(step);
                    yield return null;
                }
                waitingLineWk = step.WorkContainers[0];
                waitingLineWk.RemovePersonFromWaitingLine(gameObject);
                agent.SetDestination(workContainer.transform.position);

                if (Utils.HasSamePosition(transform.position, workContainer.transform.position))
                {
                    workContainer.setUsingPerson(gameObject);
                    yield return StartCoroutine(DoStep(step));
                    workContainer.setUsingPerson(null);
                    stepPerformer.SetIsFinished(true);
                }
                yield return null;
            }
            yield return null;
            taskPerformer.RemoveFirstStepPerformer();

        }
    }
    private IEnumerator DoStep(Step step)
    {
        yield return new WaitForSeconds(step.Data.Duration);
    }

    private WorkContainer GetTheShortestFreeWorkContainer(Step step)
    {
        var places = step.WorkContainers.FindAll(place => place.UsingPerson == null);
        if (places.Count == 0) return null;

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
    //! only use this function when you assure that all the possible work containers are busy
    private Vector3 GetWaitingPosition(Step step)
    {
        var workContainer = step.WorkContainers[0];
        workContainer.AddPersonToWaitingLine(gameObject);
        var transformWk = workContainer.transform;
        var distance = 2;
        return transformWk.position + distance * workContainer.GetIndexInWaitingLine(gameObject) * transformWk.forward;
    }




}