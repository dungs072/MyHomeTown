using System.Collections.Generic;
using UnityEngine;

public class NeedObject
{
    public NeedItemData itemData;
    public int neededAmount = 0;
    public int gainedAmount = 0;
}

public class StepPerformer
{
    private Step step;
    private bool isFinished = false;

    private List<NeedObject> needObjects;
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
    public void AddNeedObject(NeedItemData item, int neededAmount)
    {
        needObjects ??= new List<NeedObject>();

        var needObject = new NeedObject
        {
            itemData = item,
            neededAmount = neededAmount,
            gainedAmount = 0
        };

        needObjects.Add(needObject);
    }

}