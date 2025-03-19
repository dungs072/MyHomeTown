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
    //! this will run in sequence. So pls do not break it :))
    public IEnumerator HandleAllTask()
    {
        for (int i = 0; i < taskPerformers.Count; i++)
        {
            var taskPerformer = taskPerformers[i];
            yield return HandleTask(taskPerformer.Task);
        }
    }
    private IEnumerator HandleTask(Task task)
    {
        var steps = task.Steps;
        for (int i = 0; i < steps.Count; i++)
        {
            var step = steps[i];
            yield return HandleStep(step);
        }
    }
    private IEnumerator HandleStep(Step step)
    {
        var workContainer = GetTheShortestFreeWorkContainer(step);
        while (workContainer == null)
        {
            var waitingPosition = GetWaitingPosition(step);
            agent.SetDestination(waitingPosition);
            workContainer = GetTheShortestFreeWorkContainer(step);
            yield return null;
        }
        var defaultWorkContainer = step.WorkContainers[0];
        defaultWorkContainer.TryRemovePersonFromWaitingLine(gameObject);
        yield return MoveToWorkContainer(workContainer);
        workContainer.setUsingPerson(gameObject);
        yield return DoStep(step);
        workContainer.setUsingPerson(null);

    }
    private IEnumerator MoveToWorkContainer(WorkContainer workContainer)
    {
        var targetPosition = workContainer.transform.position;
        while (!Utils.HasSamePosition(agent.transform.position, targetPosition))
        {
            yield return TryWaitingInLine(workContainer);
            Debug.Log("MoveToWorkContainer");
            workContainer.TryRemovePersonFromWaitingLine(gameObject);
            agent.SetDestination(targetPosition);
            yield return null;
        }
    }
    private IEnumerator TryWaitingInLine(WorkContainer workContainer)
    {
        while (!workContainer.IsFreeToUse(gameObject))
        {
            var waitingPosition = GetWaitingPosition(workContainer);
            agent.SetDestination(waitingPosition);
            yield return null;
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
        return GetWaitingPosition(workContainer);
    }
    private Vector3 GetWaitingPosition(WorkContainer workContainer)
    {
        workContainer.AddPersonToWaitingLine(gameObject);
        var transformWk = workContainer.transform;
        var distance = 2;
        return transformWk.position + distance * workContainer.GetIndexInWaitingLine(gameObject) * transformWk.forward;
    }




}