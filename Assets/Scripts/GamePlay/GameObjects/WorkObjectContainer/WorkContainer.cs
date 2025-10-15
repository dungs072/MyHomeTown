using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorkContainer : MonoBehaviour
{
    private List<IWorkable> workersQueue = new();

    public bool IsReadyToUse()
    {
        if (workersQueue.Count == 0) return true;
        var currentWorker = workersQueue[0];
        return !currentWorker.IsWorking();
    }
    public bool HasWorkersInQueue()
    {
        return workersQueue.Count > 0;
    }
    public void RequestToWork(IWorkable worker)
    {
        if (workersQueue.Contains(worker)) return;
        workersQueue.Add(worker);
    }
    public void FinishWork(IWorkable worker)
    {
        if (!workersQueue.Contains(worker)) return;
        workersQueue.Remove(worker);
    }

}