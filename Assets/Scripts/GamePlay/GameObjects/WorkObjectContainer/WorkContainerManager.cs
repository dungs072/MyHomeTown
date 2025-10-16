using System;
using System.Collections.Generic;
using UnityEngine;

public class WorkContainerManager : MonoBehaviour
{

    public static event Action<WorkContainer> OnWorkContainerAdded;
    public static event Action<WorkContainer> OnWorkContainerRemoved;
    [SerializeField] private List<WorkContainer> workContainers;

    public List<WorkContainer> WorkContainers => workContainers;

    private Dictionary<WorkContainerType, List<WorkContainer>> workContainerDict = new();
    public Dictionary<WorkContainerType, List<WorkContainer>> WorkContainerDict => workContainerDict;
    void Start()
    {
        foreach (var wk in workContainers)
        {
            if (wk == null) continue;
            AddWorkContainerToDict(wk);
        }
    }

    public void AddWorkContainer(WorkContainer workContainer)
    {
        if (workContainers == null)
        {
            workContainers = new List<WorkContainer>();
        }
        OnWorkContainerAdded?.Invoke(workContainer);
        workContainers.Add(workContainer);
        AddWorkContainerToDict(workContainer);
    }
    public void RemoveWorkContainer(WorkContainer workContainer)
    {
        if (workContainers == null) return;
        OnWorkContainerRemoved?.Invoke(workContainer);
        workContainers.Remove(workContainer);
        RemoveWorkContainerFromDict(workContainer);
    }
    public void AddWorkContainerToDict(WorkContainer workContainer)
    {
        if (!workContainerDict.ContainsKey(workContainer.Type))
        {
            workContainerDict[workContainer.Type] = new List<WorkContainer>();
        }
        workContainerDict[workContainer.Type].Add(workContainer);
    }
    public void RemoveWorkContainerFromDict(WorkContainer workContainer)
    {
        if (!workContainerDict.ContainsKey(workContainer.Type)) return;
        var workContainerList = workContainerDict[workContainer.Type];
        workContainerList.Remove(workContainer);
        if (workContainerList.Count == 0)
        {
            workContainerDict.Remove(workContainer.Type);
        }
    }
}
