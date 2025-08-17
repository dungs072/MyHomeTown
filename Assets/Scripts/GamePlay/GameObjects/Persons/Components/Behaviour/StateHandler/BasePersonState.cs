using static ManagerSingleton;
public class BasePersonState : IState
{
    protected PatrollingSystem patrollingSystem;
    protected Person person;
    protected PersonStatus personStatus;
    protected TaskHandler taskHandler;
    protected AgentController agent;
    protected BaseBehavior behavior;

    public BasePersonState(Person person)
    {
        patrollingSystem = EmpireInstance.PatrollingSystem;
        this.person = person;
        personStatus = person.PersonStatus;
        taskHandler = person.GetComponent<TaskHandler>();
        agent = person.GetComponent<AgentController>();
    }

    public virtual void EnterState()
    {
        // Enter state logic can be implemented in derived classes
        // get the behavior again 
        behavior = person.PersonBehavior;
    }
    public virtual void UpdateState()
    {
        // Update state logic can be implemented in derived classes
    }
    public virtual void ExitState()
    {
        // Exit state logic can be implemented in derived classes
    }
}