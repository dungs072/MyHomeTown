using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEngine;
//! This will handle all the tasks that are assigned to a person    
//! Must use yield return StartCoroutine(YourFunction()) to make sure the coroutine is running in sequence
[RequireComponent(typeof(AgentController))]
public class TaskHandler : MonoBehaviour
{
    private List<TaskPerformer> taskPerformers;
    private AgentController agent;

    private int currentTaskIndex = 0;

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
    public IEnumerator HandleAllAssignedTask()
    {
        if (currentTaskIndex >= taskPerformers.Count)
        {
            StopAllCoroutines();
            yield break;
        }
        yield return StartCoroutine(HandleCurrentTask());
    }
    public IEnumerator HandleCurrentTask()
    {
        var taskPerformer = taskPerformers[currentTaskIndex];
        while (!taskPerformer.IsFinished())
        {
            var stepPerformer = taskPerformer.GetCurrentStepPerformer();
            var workContainer = GetTheShortestFreeWorkContainer(stepPerformer.Step);
            if (workContainer == null)
            {
                yield break;
            }
            workContainer.AddPersonToWaitingLine(this);
            if (!workContainer.IsFreeToUse(this))
            {
                var position = GetWaitingPosition(workContainer);
                TriggerWaitingInLine(position);
                yield break;
            }
            yield return StartCoroutine(MoveToWorkContainer(workContainer));
            workContainer.SetUsingPerson(this);
            yield return StartCoroutine(DoStep(stepPerformer.Step));
            workContainer.SetUsingPerson(null);
            taskPerformer.MoveToNextStep();
        }

        if (taskPerformer.IsFinished())
        {
            currentTaskIndex++;
            StartCoroutine(HandleAllAssignedTask());
        }
    }
    public void TriggerWaitingInLine(Vector3 waitingPos)
    {
        agent.SetDestination(waitingPos);
        StopAllCoroutines();
    }

    private IEnumerator MoveToWorkContainer(WorkContainer workContainer)
    {
        var targetPosition = workContainer.transform.position;
        while (!Utils.HasSamePosition(agent.transform.position, targetPosition))
        {
            agent.SetDestination(targetPosition);
            yield return null;
        }
    }

    private IEnumerator DoStep(Step step)
    {
        yield return new WaitForSeconds(step.Data.Duration);
    }

    private WorkContainer GetTheShortestFreeWorkContainer(Step step)
    {
        var places = step.WorkContainers.FindAll(wc => wc.IsFreeToUse());
        if (places.Count == 0)
        {
            var lowPersons = int.MaxValue;
            foreach (var wc in step.WorkContainers)
            {
                var totalPerson = wc.CountPersonInWaitingLine();
                if (totalPerson < lowPersons)
                {
                    lowPersons = totalPerson;
                    places.Clear();
                    places.Add(wc);
                }
            }
        }

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

    //! Pls override this function for your own waiting in line shape you want
    private Vector3 GetWaitingPosition(WorkContainer workContainer)
    {

        var transformWk = workContainer.transform;
        var distance = 2;
        return transformWk.position + distance * workContainer.GetIndexInWaitingLine(this) * transformWk.forward;
    }




}