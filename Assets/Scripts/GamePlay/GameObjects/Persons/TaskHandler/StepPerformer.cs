using System.Collections.Generic;
using UnityEngine;



public class StepPerformer
{
    private Step step;
    private bool isFinished = false;

    private List<GatheredItem> needItems;
    private float progress = 0f;

    public Step Step => step;
    public bool IsFinished => isFinished;
    public float Progress => progress;
    public List<GatheredItem> NeedItems => needItems;
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
        needItems ??= new List<GatheredItem>();

        var needObject = new GatheredItem(item.itemKey, 0);

        needItems.Add(needObject);
    }
    public void SetGainedAmount(ItemKey key, int gainedAmount)
    {
        if (needItems == null) return;

        foreach (var needObject in needItems)
        {
            if (needObject.itemData.itemKey == key)
            {
                needObject.gainedAmount = gainedAmount;
                return;
            }
        }
    }
    public GatheredItem GetNeedObject(ItemKey key)
    {
        if (needItems == null) return null;

        foreach (var needObject in needItems)
        {
            if (needObject.itemData.itemKey == key)
            {
                return needObject;
            }
        }
        return null;
    }

    public bool HasEnoughItems()
    {
        if (needItems == null || needItems.Count == 0) return true;

        foreach (var needObject in needItems)
        {
            if (needObject.gainedAmount < needObject.itemData.amount)
            {
                return false;
            }
        }
        return true;
    }
    public bool IsWorkHereInfinite()
    {
        return step.Data.Duration == -1;
    }

}