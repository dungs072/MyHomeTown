public class MoveState : BasePersonState
{
    public MoveState(Person person) : base(person)
    {
    }

    public override void EnterState()
    {
        base.EnterState();
    }

    public override void UpdateState()
    {
        var personStatus = person.PersonStatus;
        var targetPosition = personStatus.TargetPosition;
        if (targetPosition == null) return;
        agent.SetDestination(targetPosition.Value);
        if (!agent.IsReachedDestination(targetPosition.Value)) return;
        personStatus.TargetPosition = null;
    }

    public override void ExitState()
    {
        base.ExitState();
    }
}