
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
    [SerializeField] private bool isNeedItem = false;
    [SerializeField] private List<string> children = new List<string>();
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
    public bool IsNeedItem => isNeedItem;
    public List<string> Children => children;

    //? for editor use only
    public Vector2 Position { get; set; }

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