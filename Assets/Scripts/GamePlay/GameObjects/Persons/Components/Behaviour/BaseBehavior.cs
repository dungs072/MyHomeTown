using System.Collections.Generic;
using UnityEngine;
using static ManagerSingleton;

public class BaseBehavior : IPersonBehavior
{
    protected Person person;
    protected TaskHandler taskHandler;
    protected AgentController agent;
    protected PatrollingSystem patrollingSystem;

    // need items
    protected Pack needItemsPack;
    protected int currentWaitPointIndex = 0;
    private TaskPerformer previousTaskPerformer;
    private PersonStateMachine stateMachine;


    public BaseBehavior(Person person)
    {
        this.person = person;
        taskHandler = person.GetComponent<TaskHandler>();
        agent = person.GetComponent<AgentController>();
        patrollingSystem = EmpireInstance.PatrollingSystem;
        stateMachine = new PersonStateMachine(person);
        //! temporary code to initialize need items pack
        needItemsPack = new Pack(100);
        //! temporary code to initialize patrolling path
        InitBehavior();
    }
    public virtual void InitBehavior()
    {
        var personStatus = person.PersonStatus;
        personStatus.CurrentPatrollingPath = patrollingSystem.PathDictionary[PatrollingPathKey.DefaultPath];
        personStatus.TargetPosition = personStatus.CurrentPatrollingPath.Waypoints[0].position;
        stateMachine.ChangeState<PatrollingState>();
    }
    public void ExecuteBehavior()
    {
        UpdatePersonState();
        stateMachine.Update();

    }
    protected virtual void UpdatePersonState()
    {
        var personStatus = person.PersonStatus;
        if (personStatus.CurrentPatrollingPath) return;

        if (personStatus.TargetPosition != null)
        {
            stateMachine.ChangeState<MoveState>();
            return;
        }

        if (personStatus.CurrentTaskPerformer == null)
        {
            stateMachine.ChangeState<IdleState>();
            return;
        }

        var wk = personStatus.CurrentWorkContainer;
        var currentStep = personStatus.CurrentTaskPerformer.GetCurrentStepPerformer();
        if (wk == null)
        {
            wk = TaskCoordinator.GetSuitableWorkContainer(currentStep.Step.Data.WorkContainerType, person);
            wk.AddPersonToWorkContainer(person);
            personStatus.CurrentWorkContainer = wk;
            personStatus.TargetPosition = wk.GetWaitingPosition(person);

            return;
        }

        var waitingPosition = wk.GetWaitingPosition(person);
        if (!agent.IsReachedDestination(waitingPosition))
        {
            personStatus.TargetPosition = waitingPosition;
            stateMachine.ChangeState<MoveState>();
            return;
        }

        if (wk.IsPersonUse(person) && TryToMeetConditionsToWork())
        {
            stateMachine.ChangeState<WorkState>();
        }
        else
        {
            stateMachine.ChangeState<WaitState>();
        }
    }
    protected virtual bool TryToMeetConditionsToWork()
    {
        return true;
    }

    #region Task Handling

    public virtual void HandleStartTask()
    {
        var needItems = GetNeedItemsFromCurrentToEndStep();
        if (needItems == null || needItems.Count == 0) return;
        needItemsPack.AddItems(needItems);
    }

    public virtual void UpdateDoingStep()
    {
        var currentStep = person.PersonStatus.CurrentTaskPerformer.GetCurrentStepPerformer();
        var currentProgress = currentStep.Progress;
        var newProgress = currentProgress + Time.deltaTime;
        currentStep.SetProgress(newProgress);
    }

    public virtual void HandleFinishedStep()
    {
        var personStatus = person.PersonStatus;
        var selectedWK = personStatus.CurrentWorkContainer;
        var taskPerformer = personStatus.CurrentTaskPerformer;

        taskPerformer.MoveToNextStep();
        selectedWK.RemovePersonFromWorkContainer(person);
        personStatus.CurrentWorkContainer = null;
    }
    public virtual void HandleFinishedTask()
    {
        taskHandler.MoveNextTask();
        needItemsPack.Clear();
        HandleEndTask();
    }
    protected virtual void HandleEndTask()
    {
        if (person.PersonStatus.CurrentTaskPerformer != null) return;
        // Base case: no task performer, so we can reset the task handler
        taskHandler.CreateNewTask();
    }

    #endregion

    #region Need Items

    public void TakeNeedItemsFrom(Dictionary<ItemKey, int> items)
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

}