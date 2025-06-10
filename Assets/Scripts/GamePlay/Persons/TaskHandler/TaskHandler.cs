using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static ManagerSingleton;
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
        yield return HandleCurrentTask();
    }
    public IEnumerator HandleCurrentTask()
    {
        var taskPerformer = taskPerformers[currentTaskIndex];
        while (!taskPerformer.IsFinished())
        {
            var stepPerformer = taskPerformer.GetCurrentStepPerformer();
            var step = stepPerformer.Step;
            var selectedWK = GetSuitableWorkContainer(step);
            if (!selectedWK)
            {
                //! Should trigger event here to notify that no suitable work container found
                yield break;
            }

            yield return MoveToWorkContainer(selectedWK);
            //! do the task
            if (selectedWK.IsFreeToUse(this))
            {
                selectedWK.TryRemovePersonFromWaitingLine(this);
                yield return HandleDoTask(selectedWK, step);
                taskPerformer.MoveToNextStep();
            }
            //! wait in line
            else
            {
                //! must check there if the wk is not free or disappeared
                yield return HandleWaitingLine(selectedWK);
            }
        }
        Debug.Log($"Task {taskPerformer.Task} is finished.");
    }
    private IEnumerator HandleWaitingLine(WorkContainer selectedWK)
    {
        selectedWK.AddPersonToWaitingLine(this);
        yield return MoveToWaitingLine(selectedWK);
        yield return new WaitUntil(() => selectedWK.IsFreeToUse());
    }
    private IEnumerator HandleDoTask(WorkContainer selectedWK, Step step)
    {
        yield return DoStep(step);
        selectedWK.SetUsingPerson(null);
    }
    private IEnumerator MoveToWaitingLine(WorkContainer wk)
    {
        var waitingPos = wk.GetWaitingPosition(this);
        agent.SetDestination(waitingPos);
        while (!agent.IsReachedDestination(waitingPos))
        {
            agent.SetDestination(waitingPos);
            waitingPos = wk.GetWaitingPosition(this);
            yield return null;
        }
    }
    private IEnumerator MoveToWorkContainer(WorkContainer wk)
    {
        Func<bool> shouldStopWhenMoving = () =>
        {
            return !wk.IsFreeToUse();
        };
        Action moveFinished = () =>
        {
            wk.SetUsingPerson(this);
        };
        yield return agent.MoveToPosition(wk.transform.position, shouldStopWhenMoving, moveFinished);

    }
    private WorkContainer GetSuitableWorkContainer(Step step)
    {
        var suitableWorkContainer = GetShortestWorkContainer(step.WorkContainerType, true);
        //! case when no suitable work container found
        if (suitableWorkContainer == null)
        {
            suitableWorkContainer = GetShortestWorkContainer(step.WorkContainerType, false);

        }
        return suitableWorkContainer;
    }
    private WorkContainer GetShortestWorkContainer(WorkContainerType type, bool shouldBeFree = true)
    {
        var workContainers = EmpireInstance.WorkContainerManager.WorkContainers;
        var sameTypePlaces = workContainers.FindAll(wc => wc.WorkContainerType == type);
        WorkContainer suitableWorkContainer = null;
        float minDistance = float.MaxValue;
        foreach (var workContainer in sameTypePlaces)
        {
            if (!workContainer.IsFreeToUse(this) && shouldBeFree) continue;
            var sqrtDistance = Vector3.SqrMagnitude(agent.transform.position - workContainer.transform.position);
            if (sqrtDistance >= minDistance) continue;
            minDistance = sqrtDistance;
            suitableWorkContainer = workContainer;
        }
        return suitableWorkContainer;
    }


    private IEnumerator DoStep(Step step)
    {
        yield return new WaitForSeconds(step.Data.Duration);
    }



}