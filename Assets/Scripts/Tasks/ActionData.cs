using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "ActionData", menuName = "Scriptable Objects/ActionData")]
public class ActionData : ScriptableObject
{
    [SerializeField] private string actionName = "Action";
    [SerializeField] private float finishTime = 2f;

    public float FinishTime => finishTime;
    public string ActionName => actionName;

    private void OnValidate()
    {
        actionName = this.name;
    }


    //? custom for animations to send the person can perform the action
}
