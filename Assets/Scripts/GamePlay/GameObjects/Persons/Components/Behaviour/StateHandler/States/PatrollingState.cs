using UnityEngine;

public class PatrollingState : BasePersonState
{
    private int currentWaitPointIndex = 0;

    public PatrollingState(Person person) : base(person)
    {
    }

    public override void EnterState()
    {
        currentWaitPointIndex = 0;
    }

    public override void UpdateState()
    {
        var personStatus = person.PersonStatus;
        var currentPatrolling = personStatus.CurrentPatrollingPath;
        if (currentPatrolling == null) return;
        var maxIndex = currentPatrolling.Waypoints.Length - 1;
        if (currentWaitPointIndex > maxIndex) return;
        var newPos = currentPatrolling.Waypoints[currentWaitPointIndex].position;
        person.PersonStatus.TargetPosition = newPos;
        agent.SetDestination(newPos);
        if (agent.IsReachedDestination(newPos))
        {
            currentWaitPointIndex++;
            if (currentWaitPointIndex > maxIndex) return;
            newPos = currentPatrolling.Waypoints[currentWaitPointIndex].position;
            person.PersonStatus.TargetPosition = newPos;
            personStatus.CurrentPatrollingPath = null;
        }
    }

    public override void ExitState()
    {
        base.ExitState();
    }
}