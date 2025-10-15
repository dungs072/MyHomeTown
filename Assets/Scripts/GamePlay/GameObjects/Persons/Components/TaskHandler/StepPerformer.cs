using System.Collections.Generic;
using Unity.Entities.UniversalDelegates;
using UnityEngine;



public class StepPerformer
{
    private Step step;
    private List<ItemRequirement> needItems;
    public StepPerformer(Step step)
    {
        this.step = step;
    }

    public WorkContainerType WKType => step.Data.WorkContainerType;


    public void AddNeedObject(ItemRequirement item)
    {
        needItems ??= new List<ItemRequirement>();
        needItems.Add(item.Clone());
    }

}