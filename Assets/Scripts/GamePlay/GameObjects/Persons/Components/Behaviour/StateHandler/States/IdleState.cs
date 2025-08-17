using ProjectDawn.Navigation;

public class IdleState : BasePersonState
{
    public IdleState(Person person) : base(person)
    {
    }

    public override void EnterState()
    {
        agent.StopMoving();
    }

    public override void UpdateState()
    {
        base.UpdateState();
    }

    public override void ExitState()
    {
        base.ExitState();
    }
}