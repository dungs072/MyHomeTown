
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "StepData", menuName = "Data/Task/StepData")]
public class StepData : ScriptableObject
{
    [SerializeField] private string stepName;
    [SerializeField]
    private string description = null;
    [SerializeField] private StepType stepType = StepType.SINGLE;
    [SerializeField] private WorkContainerType workContainerType;
    [SerializeField] private float duration = 0;
    [SerializeField] private bool isNeedItem = false;
    [SerializeField] private List<StepData> nextSteps;

    public string StepName => stepName;
    public string Description => description;
    public float Duration => duration;
    public StepType StepType => stepType;
    public WorkContainerType WorkContainerType => workContainerType;
    public bool IsNeedItem => isNeedItem;

    public List<StepData> NextSteps => nextSteps;

    public Vector2 Position { get; set; } = Vector2.zero;
}