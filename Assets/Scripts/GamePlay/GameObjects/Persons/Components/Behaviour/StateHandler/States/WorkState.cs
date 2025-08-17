using UnityEngine;

public class WorkState : BasePersonState
{
    private TaskPerformer previousTaskPerformer;
    public WorkState(Person person) : base(person)
    {
    }

    public override void EnterState()
    {
        base.EnterState();
        var personStatus = person.PersonStatus;
        if (previousTaskPerformer != personStatus.CurrentTaskPerformer)
        {
            previousTaskPerformer = personStatus.CurrentTaskPerformer;
            behavior.HandleStartTask();
        }
    }

    public override void UpdateState()
    {
        var personStatus = person.PersonStatus;
        var taskPerformer = personStatus.CurrentTaskPerformer;
        var currentStep = taskPerformer.GetCurrentStepPerformer();
        behavior.UpdateDoingStep();

        if (currentStep.IsFinished)
        {
            behavior.HandleFinishedStep();
        }

        if (taskPerformer.IsFinished())
        {
            behavior.HandleFinishedTask();
        }
    }

    public override void ExitState()
    {
        base.ExitState();
    }
}