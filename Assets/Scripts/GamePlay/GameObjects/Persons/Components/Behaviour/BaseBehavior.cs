using System.Collections.Generic;
using BaseEngine.Debuggers;
using UnityEngine;
using static ManagerSingleton;

public class BaseBehavior : IWorkable
{
    protected Person person;
    protected TaskHandler taskHandler;
    protected AgentController agent;
    protected PatrollingSystem patrollingSystem;

    // need items
    protected Pack needItemsPack;


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
        agent = person.GetComponent<AgentController>();
        patrollingSystem = EmpireInstance.PatrollingSystem;
        needItemsPack = new Pack(100);
    }
    private void RegisterEvents()
    {
        EventBus.Subscribe(GameEvents.TaskHandlerEvents.OnTaskAvailable, OnTaskAvailable);
    }
    private void UnregisterEvents()
    {
        EventBus.Unsubscribe(GameEvents.TaskHandlerEvents.OnTaskAvailable, OnTaskAvailable);
    }
    private void OnTaskAvailable(object taskPerformer)
    {
        //? cast type check
        if (taskPerformer is not TaskPerformer performer) return;
        var step = performer.CurStep;
        var wkType = step.WKType;
        var chosenWK = TaskCoordinator.GetSuitableWorkContainer(wkType, person.transform);
        if (chosenWK == null)
        {
            BugTracer.Trace($"No suitable work container found for type {wkType} for person {person.PersonData.Name}");
            return;
        }
        chosenWK.RequestToWork(this);
        agent.MoveTo(new Movable
        {
            destination = chosenWK.transform.position,

        });

    }
    public void DoWork()
    {
        throw new System.NotImplementedException();
    }

    public bool IsWorking()
    {
        throw new System.NotImplementedException();
    }


}