using System.Collections.Generic;
using UnityEngine;

public class WorkContainer : MonoBehaviour
{
    [SerializeField] private List<TaskType> taskTypes;

    public List<TaskType> TaskTypes => taskTypes;

    //TODO add more functions to handle work container
}