using System.Collections.Generic;
using UnityEngine;

public class RequiredItem
{
    public ItemData itemData;
    public int gainedAmount = 0;
}

public class StepPerformer
{
    private Step step;
    private bool isFinished = false;

    private List<RequiredItem> needObjects;
    private float progress = 0f;

    public Step Step => step;
    public bool IsFinished => isFinished;
    public float Progress => progress;
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
    public void AddNeedObject(ItemData item)
    {
        needObjects ??= new List<RequiredItem>();

        var needObject = new RequiredItem
        {
            itemData = item,
            gainedAmount = 0
        };

        needObjects.Add(needObject);
    }
    public void SetGainedAmount(NeedItemKey key, int gainedAmount)
    {
        if (needObjects == null) return;

        foreach (var needObject in needObjects)
        {
            if (needObject.itemData.itemKey == key)
            {
                needObject.gainedAmount = gainedAmount;
                return;
            }
        }
    }
    public RequiredItem GetNeedObject(NeedItemKey key)
    {
        if (needObjects == null) return null;

        foreach (var needObject in needObjects)
        {
            if (needObject.itemData.itemKey == key)
            {
                return needObject;
            }
        }
        return null;
    }


}