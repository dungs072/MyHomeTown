using System;
using System.Collections.Generic;
using UnityEngine;

public class WorkContainer : MonoBehaviour
{
    [SerializeField] private WorkContainerType workContainerType;
    public WorkContainerType WorkContainerType => workContainerType;

    private GameObject usingPerson;
    public GameObject UsingPerson => usingPerson;

    private List<GameObject> personsWaitingLine;
    public List<GameObject> PersonsWaitingLine => personsWaitingLine;

    void Awake()
    {
        personsWaitingLine = new List<GameObject>();
    }

    public void setUsingPerson(GameObject person)
    {
        usingPerson = person;
    }
    public bool IsFreeToUse(GameObject person)
    {
        if (usingPerson == person)
        {
            return true;
        }
        return usingPerson == null;
    }

    public void AddPersonToWaitingLine(GameObject person)
    {
        if (personsWaitingLine.Contains(person)) return;
        personsWaitingLine.Add(person);
    }
    public void RemovePersonFromWaitingLine(GameObject person)
    {
        if (!personsWaitingLine.Contains(person)) return;
        personsWaitingLine.Remove(person);
    }
    public int GetWaitingLinePersonsCount()
    {
        return personsWaitingLine.Count;
    }
    public int GetIndexInWaitingLine(GameObject person)
    {
        return personsWaitingLine.IndexOf(person) + 1;
    }




    //TODO add more functions to handle work container
}