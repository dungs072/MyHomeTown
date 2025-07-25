using UnityEngine;


public class PersonStatus
{
    public PersonState CurrentState { get; set; }
    public TaskPerformer CurrentTaskPerformer { get; set; }
    public StepPerformer CurrentStepPerformer { get; set; }
    public WorkContainer CurrentWorkContainer { get; set; }
}
