using System.Collections.Generic;
using UnityEngine;
using static ManagerSingleton;

public class BaseBehaviour : IPersonBehaviour
{
    protected Person person;
    protected TaskHandler taskHandler;
    protected AgentController agent;
    protected PatrollingSystem patrollingSystem;

    // need items
    protected Pack needItemsPack;
    protected int currentWaitPointIndex = 0;
    private TaskPerformer previousTaskPerformer;
    public BaseBehaviour(Person person)
    {
        this.person = person;
        taskHandler = person.GetComponent<TaskHandler>();
        agent = person.GetComponent<AgentController>();
        patrollingSystem = EmpireInstance.PatrollingSystem;
        //! temporary code to initialize need items pack
        needItemsPack = new Pack(100);
        //! temporary code to initialize patrolling path
        InitBehaviour();
    }
    public void InitBehaviour()
    {
        // default is patrolling
        var personStatus = person.PersonStatus;
        personStatus.StartPatrollingPath = patrollingSystem.PathDictionary[PatrollingPathKey.DefaultPath];
        personStatus.TargetPosition = personStatus.StartPatrollingPath.Waypoints[0].position;
        person.SwitchState(PersonState.MOVE);
    }
    public void ExecuteBehaviour()
    {
        var personState = person.PersonStatus.CurrentState;
        UpdatePersonState();
        if (personState == PersonState.IDLE)
        {
            HandleIdle();
        }
        else if (personState == PersonState.MOVE)
        {
            HandleMove();
        }
        else if (personState == PersonState.WAIT)
        {
            HandleWait();
        }
        else if (personState == PersonState.WORK)
        {
            HandleWork();
        }

        // other states
        // UpdateHandleTask();
        // HandleEndTask();

    }
    protected virtual void UpdatePersonState()
    {
        var personStatus = person.PersonStatus;

        if (personStatus.TargetPosition != null)
        {
            person.SwitchState(PersonState.MOVE);
            return;
        }

        if (personStatus.CurrentTaskPerformer == null)
        {
            person.SwitchState(PersonState.IDLE);
            return;
        }

        var wk = personStatus.CurrentWorkContainer;
        var currentStep = personStatus.CurrentTaskPerformer.GetCurrentStepPerformer();
        if (wk == null)
        {
            wk = TaskCoordinator.GetSuitableWorkContainer(currentStep.Step.Data.WorkContainerType, person);
            personStatus.CurrentWorkContainer = wk;
            wk.AddPersonToWorkContainer(person);
            personStatus.TargetPosition = wk.GetWaitingPosition(person);
            return;
        }

        if (wk.IsPersonUse(person))
        {
            if (personStatus.CurrentState != PersonState.WORK)
            {
                Debug.Log($"<color=#a635d7>PersonState.WORK: {PersonState.WORK}</color>");
            }
            person.SwitchState(PersonState.WORK);
        }
        else
        {
            if (personStatus.CurrentState != PersonState.WAIT)
            {
                Debug.Log($"<color=#a635d7>PersonState.WAIT: {PersonState.WAIT}</color>");
            }
            person.SwitchState(PersonState.WAIT);
        }
    }
    protected virtual void HandleIdle()
    {
        // just do nothing
        agent.StopMoving();
    }
    protected virtual void HandleMove()
    {

        HandleMovePatrolling();

        // move normally
        var personStatus = person.PersonStatus;
        var targetPosition = personStatus.TargetPosition;
        if (targetPosition == null) return;
        agent.SetDestination(targetPosition.Value);
        if (!agent.IsReachedDestination(targetPosition.Value)) return;
        personStatus.TargetPosition = null;
    }
    protected void HandleMovePatrolling()
    {
        var personStatus = person.PersonStatus;
        var currentPatrolling = personStatus.StartPatrollingPath;
        if (currentPatrolling == null) return;
        var maxIndex = currentPatrolling.Waypoints.Length - 1;
        if (currentWaitPointIndex > maxIndex) return;
        var newPos = currentPatrolling.Waypoints[currentWaitPointIndex].position;
        person.PersonStatus.TargetPosition = newPos;
        if (agent.IsReachedDestination(newPos))
        {
            currentWaitPointIndex++;
            if (currentWaitPointIndex > maxIndex) return;
            newPos = currentPatrolling.Waypoints[currentWaitPointIndex].position;
            person.PersonStatus.TargetPosition = newPos;
        }
    }
    protected virtual void HandleWait()
    {
        agent.StopMoving();
    }
    protected virtual void HandleWork()
    {
        var personStatus = person.PersonStatus;
        var taskPerformer = personStatus.CurrentTaskPerformer;
        var currentStep = taskPerformer.GetCurrentStepPerformer();
        DoWork();

        if (currentStep.IsFinished)
        {
            HandleFinishedStep();
        }

        if (taskPerformer.IsFinished())
        {
            taskHandler.MoveNextTask();
            previousTaskPerformer = null;
            needItemsPack.Clear();
        }
    }

    #region Patrolling
    protected virtual bool StartPatrollingOverTime(string key = PatrollingPathKey.DefaultPath)
    {
        if (!patrollingSystem) return true;
        var patrollingPath = patrollingSystem.PathDictionary[key];
        if (patrollingPath == null || patrollingPath.Waypoints.Length == 0) return true;

        var maxIndex = patrollingPath.Waypoints.Length - 1;
        if (currentWaitPointIndex > maxIndex) return true;

        var targetPosition = patrollingPath.Waypoints[currentWaitPointIndex].position;
        person.SwitchState(PersonState.MOVE);

        if (!agent.IsReachedDestination(targetPosition))
        {
            agent.SetDestination(targetPosition);
        }
        else
        {
            currentWaitPointIndex++;
        }

        return false;
    }
    #endregion

    #region Task Handling
    protected virtual void UpdateHandleTask()
    {
        var personStatus = person.PersonStatus;
        var taskPerformer = personStatus.CurrentTaskPerformer;
        if (taskPerformer == null || taskPerformer.IsFinished()) return;
        var currentStep = taskPerformer.GetCurrentStepPerformer();

        if (!personStatus.CurrentWorkContainer)
        {
            personStatus.CurrentWorkContainer = TaskCoordinator.GetSuitableWorkContainer(currentStep.Step.Data.WorkContainerType, person);
        }

        if (previousTaskPerformer != taskPerformer)
        {
            HandleStartTask();
            previousTaskPerformer = taskPerformer;
        }

        if (!TryToMoveToTarget()) return;
        if (!TryToMeetConditionsToDoStep()) return;

        HandleStep();

        if (currentStep.IsFinished)
        {
            HandleFinishedStep();
        }

        if (taskPerformer.IsFinished())
        {
            taskHandler.MoveNextTask();
            previousTaskPerformer = null;
            needItemsPack.Clear();
        }
    }

    protected virtual void HandleStartTask()
    {
        var needItems = GetNeedItemsFromCurrentToEndStep();
        if (needItems == null || needItems.Count == 0) return;
        needItemsPack.AddItems(needItems);
    }

    protected bool TryToMoveToTarget()
    {
        var selectedWK = person.PersonStatus.CurrentWorkContainer;
        if (selectedWK == null) return false;

        var targetPosition = GetTargetPosition();

        if (!agent.IsReachedDestination(targetPosition))
        {
            agent.SetDestination(targetPosition);
            person.SwitchState(PersonState.MOVE);
            return false;
        }

        return true;
    }

    protected virtual Vector3 GetTargetPosition()
    {
        var selectedWK = person.PersonStatus.CurrentWorkContainer;
        selectedWK.AddPersonToWorkContainer(person);
        return selectedWK.GetWaitingPosition(person);
    }

    protected virtual bool TryToMeetConditionsToDoStep()
    {
        return true;
    }

    protected void HandleStep()
    {
        if (CanWork())
        {
            DoWork();
        }
        else
        {
            Wait();
        }
    }

    protected virtual bool CanWork()
    {
        var selectedWK = person.PersonStatus.CurrentWorkContainer;
        return selectedWK.IsPersonUse(person);
    }

    protected virtual void DoWork()
    {
        var currentStep = person.PersonStatus.CurrentTaskPerformer.GetCurrentStepPerformer();
        var currentProgress = currentStep.Progress;
        var newProgress = currentProgress + Time.deltaTime;
        currentStep.SetProgress(newProgress);
    }

    protected virtual void Wait()
    {
        person.SwitchState(PersonState.WAIT);
    }

    protected void HandleFinishedStep()
    {
        var personStatus = person.PersonStatus;
        var selectedWK = personStatus.CurrentWorkContainer;
        var taskPerformer = personStatus.CurrentTaskPerformer;

        HandleWithItems();
        taskPerformer.MoveToNextStep();
        selectedWK.RemovePersonFromWorkContainer(person);
        personStatus.CurrentWorkContainer = null;
    }

    protected virtual void HandleWithItems()
    {
        //todo
    }

    #endregion

    #region Need Items

    public void TakeNeedItems(Dictionary<ItemKey, int> items)
    {
        var needItems = GetNeedItemsFromCurrentToEndStep();
        if (needItems == null || needItems.Count == 0) return;
        foreach (var needItem in needItems)
        {
            var itemKey = needItem.itemKey;
            var requiredAmount = needItem.amount;

            if (items.TryGetValue(itemKey, out int amount))
            {
                var gainedAmount = Mathf.Min(amount, requiredAmount);
                person.Pack.AddItem(itemKey, gainedAmount);
                items[itemKey] -= gainedAmount;
            }
        }
    }
    protected virtual List<ItemRequirement> GetNeedItemsFromCurrentToEndStep()
    {
        List<ItemRequirement> items = new();
        var personStatus = person.PersonStatus;
        var currentTask = personStatus.CurrentTaskPerformer;
        var currentStep = currentTask.GetCurrentStepPerformer();
        if (currentStep == null) return null;

        for (int i = currentTask.CurrentStepIndex; i < currentTask.StepPerformers.Count; i++)
        {
            var step = currentTask.StepPerformers[i];
            if (step == null) continue;
            var stepNeedItems = step.NeedItems;
            if (stepNeedItems == null || stepNeedItems.Count == 0) continue;
            items.AddRange(stepNeedItems);
        }

        return items;
    }
    #endregion

    #region EndTask
    protected virtual bool HandleEndTask()
    {
        if (person.PersonStatus.CurrentTaskPerformer != null) return false;
        // Base case: no task performer, so we can reset the task handler
        taskHandler.CreateNewTask();
        return true;
    }
    #endregion
}