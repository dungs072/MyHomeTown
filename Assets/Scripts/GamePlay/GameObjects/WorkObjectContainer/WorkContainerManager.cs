using System;
using System.Collections.Generic;
using UnityEngine;

public class WorkContainerManager : MonoBehaviour
{

    public static event Action<WorkContainer> OnWorkContainerAdded;
    public static event Action<WorkContainer> OnWorkContainerRemoved;
    [SerializeField] private List<WorkContainer> workContainers;

    public List<WorkContainer> WorkContainers => workContainers;


    public void AddWorkContainer(WorkContainer workContainer)
    {
        if (workContainers == null)
        {
            workContainers = new List<WorkContainer>();
        }
        OnWorkContainerAdded?.Invoke(workContainer);
        workContainers.Add(workContainer);
    }
    public void RemoveWorkContainer(WorkContainer workContainer)
    {
        if (workContainers == null) return;
        OnWorkContainerRemoved?.Invoke(workContainer);
        workContainers.Remove(workContainer);
    }
}
