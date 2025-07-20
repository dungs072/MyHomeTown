//! an step will hold some step data is created by system; 
//! be noticed that this is a step created by system and used for all persons class
//! the list WorkContainers must be updated every user add a new WorkContainer to game
using System.Collections.Generic;

public class Step
{
    public StepData Data { get; private set; }

    public List<WorkContainer> WorkContainers { get; private set; }

    public WorkContainerType WorkContainerType => Data.WorkContainerType;


    public Step(StepData data)
    {
        Data = data;
        WorkContainers = new List<WorkContainer>();
    }

    public void SetWorkContainers(List<WorkContainer> workContainers)
    {
        WorkContainers = workContainers;
    }

    public void AddWorkContainer(WorkContainer workContainer)
    {
        WorkContainers.Add(workContainer);
    }


}