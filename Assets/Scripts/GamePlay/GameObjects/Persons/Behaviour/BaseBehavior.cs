using System.Collections.Generic;
using BaseEngine.Debuggers;
using UnityEngine;
using static ManagerSingleton;

public class BaseBehavior : IWorkable
{
    protected BehaviorState state = BehaviorState.IDLE;
    protected Person person;
    protected TaskHandler taskHandler;
    protected Movement movement;
    protected Laborer laborer;
    protected PatrollingSystem patrollingSystem;

    // need items
    protected Pack needItemsPack;

    protected WorkContainer _selectedWK;

    public bool IsWorking()
    {
        return state == BehaviorState.WORK;
    }

    public BaseBehavior(Person person)
    {
        this.person = person;
        InitComponents();
        RegisterEvents();
    }
    ~BaseBehavior()
    {
        UnregisterEvents();
    }
    private void InitComponents()
    {
        taskHandler = person.GetComponent<TaskHandler>();
        movement = person.GetComponent<Movement>();
        laborer = person.GetComponent<Laborer>();
        patrollingSystem = EmpireInstance.PatrollingSystem;
        needItemsPack = new Pack(100);
    }
    private void RegisterEvents()
    {
        EventBus.Subscribe(GameEvents.TaskHandlerEvents.OnTaskAvailable, HandleTaskAvailable);
    }
    private void UnregisterEvents()
    {
        EventBus.Unsubscribe(GameEvents.TaskHandlerEvents.OnTaskAvailable, HandleTaskAvailable);
    }
    private void HandleTaskAvailable(object taskPerformer)
    {
        //? cast type check
        if (taskPerformer is not TaskPerformer performer) return;
        var step = performer.CurStep;
        var wkType = step.WKType;
        _selectedWK = TaskCoordinator.GetSuitableWorkContainer(wkType, person.transform);
        if (_selectedWK == null)
        {
            BugTracer.Trace($"No suitable work container found for type {wkType} for person {person.PersonData.Name}");
            return;
        }
        if (!_selectedWK.IsFreeToRequest())
        {
            Wait();
        }
        _selectedWK.RequestToWork(this);
        movement.MoveTo(new Movable
        {
            destination = _selectedWK.transform.position,
            finishedAction = DoWork
        });
        SwitchState(BehaviorState.MOVE);
    }
    public void DoWork()
    {
        SwitchState(BehaviorState.WORK);
        _selectedWK.NotifyWorkersLeftWait();
        var step = taskHandler.CurrentStepPerformer;
        if (step == null)
        {
            BugTracer.Trace("No current step performer found.");
            return;
        }
        laborer.DoWork(new Doable
        {
            duration = 2f,
            finishedAction = HandleDoWorkFinished
        });
    }
    private void HandleDoWorkFinished()
    {
        CleanAfterWork();
        TryToFindNewWork();
    }
    private void CleanAfterWork()
    {
        SwitchState(BehaviorState.IDLE);
        _selectedWK.FinishWork(this);
        _selectedWK.NotifyWorkersLeftWork();
        _selectedWK = null;
    }
    private void TryToFindNewWork()
    {
        var task = taskHandler.CurrentTaskPerformer;
        if (task == null)
        {
            BugTracer.Trace("No current task performer found.");
            return;
        }
        task.MoveNextStep();

        var step = taskHandler.CurrentStepPerformer;
        if (step == null)
        {
            // do next task if have
            taskHandler.MoveNextTask();
        }
        else
        {
            // do new step
            HandleTaskAvailable(task);
        }
    }

    public void Wait()
    {
        SwitchState(BehaviorState.WAIT);
        movement.Stop();
    }
    public void ContinueWork()
    {
        var task = taskHandler.CurrentTaskPerformer;
        HandleTaskAvailable(task);
    }
    private void SwitchState(BehaviorState newState)
    {
        state = newState;
    }
}