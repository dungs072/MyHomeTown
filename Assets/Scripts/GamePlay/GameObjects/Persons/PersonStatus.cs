using System;
using UnityEngine;


public class PersonStatus
{
    public PersonState CurrentState { get; set; }
    public TaskPerformer CurrentTaskPerformer { get; set; }
    public WorkContainer CurrentWorkContainer { get; set; }

    public PatrollingPath CurrentPatrollingPath { get; set; }
    public Vector3? TargetPosition { get; set; }
}
