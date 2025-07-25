
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using System;


[Serializable]
public class StepData
{
    [SerializeField] private string uniqueId;
    [SerializeField] private string stepName;

    [SerializeField]
    private string description = null;
    [SerializeField] private WorkContainerType workContainerType;
    [SerializeField] private float duration = 0;
    [SerializeField] private List<ItemRequirement> needItems = new();
    [SerializeField] private bool needPermissionToGiveItems = false;
    [SerializeField] private List<ItemRequirement> possibleCreateItems = new();
    [SerializeField] private List<string> children = new List<string>();
    [SerializeField] private TaskName taskName = TaskName.NONE;
    public StepData()
    {
        uniqueId = Guid.NewGuid().ToString();
    }
    public string UniqueID => uniqueId;
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
    public List<ItemRequirement> NeedItems => needItems;
    public bool NeedPermissionToGiveItems
    {
        get => needPermissionToGiveItems;
        set => needPermissionToGiveItems = value;
    }
    public List<ItemRequirement> PossibleCreateItems => possibleCreateItems;
    public List<string> Children => children;
    public TaskName TaskName
    {
        get => taskName;
        set => taskName = value;
    }

    //? for editor purposes only
    public Vector2 position = Vector2.zero;
    public bool isMinimized = false;

    public bool TryToAddChild(string childId)
    {
        if (children.Contains(childId)) return false;
        children.Add(childId);
        return true;
    }
    public bool TryToRemoveChild(string childId)
    {
        if (!children.Contains(childId)) return false;
        children.Remove(childId);
        return true;
    }

    public bool IsChildExist(string childId)
    {
        return children.Contains(childId);
    }

    //! must override Equals and GetHashCode for dictionary usage
    public override bool Equals(object obj)
    {
        return obj is StepData other && UniqueID == other.UniqueID;
    }

    public override int GetHashCode()
    {
        return UniqueID?.GetHashCode() ?? 0;
    }

}