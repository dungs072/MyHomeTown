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
        if (person == null)
        {
            TriggerPersonLeft();
        }
        else
        {

            TryRemovePersonFromWaitingLine(person);
            TriggerWaitingInLine();
        }


    }
    private void TriggerPersonLeft()
    {
        foreach (var person in personsWaitingLine)
        {
            StartCoroutine(person.HandleCurrentTask());
        }
    }
    private void TriggerWaitingInLine()
    {
        foreach (var person in personsWaitingLine)
        {
            var index = GetIndexInWaitingLine(person);
            var distance = 2;
            var waitingPos = transform.position + distance * index * transform.forward;
            person.TriggerWaitingInLine(waitingPos);
        }
    }
    public bool IsFreeToUse(TaskHandler person)
    {
        if (usingPerson == person)
        {
            return true;
        }
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




    //TODO add more functions to handle work container
}