using System.Collections.Generic;
using UnityEngine;



public class StepPerformer
{
    private Step step;
    private bool isFinished = false;

    private List<ItemRequirement> needItems;
    private float progress = 0f;

    public Step Step => step;
    public bool IsFinished => isFinished;
    public float Progress => progress;
    public List<ItemRequirement> NeedItems => needItems;
    public StepPerformer(Step step)
    {
        this.step = step;
        isFinished = false;
    }

    public void SetIsFinished(bool isFinished)
    {
        this.isFinished = isFinished;
    }
    public void SetProgress(float progress)
    {
        this.progress = progress;
        if (this.progress >= step.Data.Duration)
        {
            SetIsFinished(true);
        }
    }

    /// <summary>
    /// Adds a need object to the step performer.
    /// </summary>
    /// <param name="item"></param>
    /// <param name="neededAmount"></param>
    public void AddNeedObject(ItemRequirement item)
    {
        needItems ??= new List<ItemRequirement>();

        needItems.Add(item);
    }

    public ItemRequirement GetNeedObject(ItemKey key)
    {
        if (needItems == null) return null;

        foreach (var needObject in needItems)
        {
            if (needObject.itemKey == key)
            {
                return needObject;
            }
        }
        return null;
    }

}