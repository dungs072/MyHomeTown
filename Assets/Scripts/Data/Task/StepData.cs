
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "StepData", menuName = "Data/Task/StepData")]
public class StepData : ScriptableObject
{
    [SerializeField] private string stepName;
    [SerializeField]
    private string description = null;
    [SerializeField] private WorkContainerType workContainerType;
    [SerializeField] private float duration = 0;
    [SerializeField] private bool isNeedItem = false;

    public string StepName
    {
        get => stepName;
        set => stepName = value;
    }
    public string Description
    {
        get => description;
        set => description = value;
    }
    public float Duration
    {
        get => duration;
        set => duration = value;
    }
    public WorkContainerType WorkContainerType
    {
        get => workContainerType;
        set => workContainerType = value;
    }
    public bool IsNeedItem => isNeedItem;

}