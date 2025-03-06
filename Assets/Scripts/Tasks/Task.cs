using System.Collections.Generic;
using UnityEngine;


//! a task will have many steps to implement
//! it has a list of step to handle and you have to use it like a queue data structure
public class Task : MonoBehaviour
{
    private List<Step> steps;


    public void PushBack(Step step)
    {
        steps.Add(step);
    }

    public void RemoveFirstStep()
    {
        if (steps.Count == 0) return;
        steps.RemoveAt(0);
    }

    public Step GetFirstStep()
    {
        if (steps.Count == 0) return null;
        return steps[0];
    }

}