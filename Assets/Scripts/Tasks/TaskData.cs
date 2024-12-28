using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TaskData", menuName = "Scriptable Objects/TaskData")]
public class TaskData : ScriptableObject
{
    [SerializeField] private List<ActionData> actions;

    public List<ActionData> Actions => actions;
}
