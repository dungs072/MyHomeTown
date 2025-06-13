using System;
using System.Collections.Generic;
using UnityEngine;

public class WorkContainer : MonoBehaviour
{

    [SerializeField] private WorkContainerType workContainerType;
    public WorkContainerType WorkContainerType => workContainerType;


    private TaskHandler usingPerson;
    public TaskHandler UsingPerson => usingPerson;

    private List<TaskHandler> personsWaitingLine;
    public List<TaskHandler> PersonsWaitingLine => personsWaitingLine;

    void Awake()
    {
        personsWaitingLine = new List<TaskHandler>();
    }
    public void SetUsingPerson(TaskHandler person)
    {
        usingPerson = person;

    }

    public bool IsFreeToUse(TaskHandler person)
    {
        if (usingPerson == person)
        {
            return true;
        }
        return usingPerson == null;
    }
    public bool IsFreeToUse()
    {
        return usingPerson == null;
    }

    public void AddPersonToWaitingLine(TaskHandler person)
    {
        if (personsWaitingLine.Contains(person)) return;
        personsWaitingLine.Add(person);
    }
    public void TryRemovePersonFromWaitingLine(TaskHandler person)
    {
        if (!personsWaitingLine.Contains(person)) return;
        personsWaitingLine.Remove(person);
    }
    public int GetIndexInWaitingLine(TaskHandler person)
    {
        return personsWaitingLine.IndexOf(person) + 1;
    }
    public int CountPersonInWaitingLine()
    {
        return personsWaitingLine.Count;
    }

    public Vector3 GetWaitingPosition(TaskHandler person)
    {
        var distance = 2;
        var index = GetIndexInWaitingLine(person);
        return transform.position + distance * index * transform.forward;
    }


    private void OnDrawGizmos()
    {
        for (int i = 0; i < personsWaitingLine.Count; i++)
        {
            var person = personsWaitingLine[i];
            var position = GetWaitingPosition(person);
            Gizmos.color = Color.yellow;

            Gizmos.DrawWireSphere(position, 1);
        }
    }




    //TODO add more functions to handle work container
}