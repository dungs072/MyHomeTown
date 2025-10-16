using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorkContainer : MonoBehaviour
{
    [SerializeField] private WorkContainerType workContainerType;
    public WorkContainerType Type => workContainerType;
    private List<IWorkable> _workersQueue = new();
    public bool HasWorkersInQueue()
    {
        return _workersQueue.Count > 0;
    }
    public bool IsFreeToRequest()
    {
        for (int i = 0; i < _workersQueue.Count; i++)
        {
            var worker = _workersQueue[i];
            if (worker.IsWorking()) return false;
        }
        return true;
    }
    public void RequestToWork(IWorkable worker)
    {
        if (_workersQueue.Contains(worker)) return;
        _workersQueue.Add(worker);
    }
    public void NotifyWorkersLeftWait()
    {
        for (int i = 0; i < _workersQueue.Count; i++)
        {
            var worker = _workersQueue[i];
            if (worker.IsWorking()) continue;
            worker.Wait();
        }
    }
    public void FinishWork(IWorkable worker)
    {
        if (!_workersQueue.Contains(worker)) return;
        _workersQueue.Remove(worker);

    }
    public void NotifyWorkersLeftWork()
    {
        for (int i = 0; i < _workersQueue.Count; i++)
        {
            var nextWorker = _workersQueue[i];
            nextWorker.ContinueWork();
        }
    }

}