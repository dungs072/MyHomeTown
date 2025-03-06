
using UnityEngine;

[CreateAssetMenu(fileName = "StepData", menuName = "Data/Task/StepData")]
public class StepData : ScriptableObject
{
    [SerializeField] private string stepName;
    [SerializeField]
    private string description = null;
    [SerializeField] private float duration = 0;

    public string StepName => stepName;
    public string Description => description;
    public float Duration => duration;
}