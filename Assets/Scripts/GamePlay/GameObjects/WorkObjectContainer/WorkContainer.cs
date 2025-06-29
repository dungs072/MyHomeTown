using System;
using System.Collections.Generic;
using UnityEngine;

public class WorkContainer : MonoBehaviour
{

    [SerializeField] private WorkContainerType workContainerType;
    public WorkContainerType WorkContainerType => workContainerType;
    private List<Person> personsWantToWorkHere = new();
    public List<Person> PersonsWantToWorkHere => personsWantToWorkHere;

    public void AddPersonToWorkContainer(Person person)
    {
        if (personsWantToWorkHere.Contains(person)) return;
        personsWantToWorkHere.Add(person);
        SortPersonsWaitingLine();
    }
    public void RemovePersonFromWorkContainer(Person person)
    {
        if (!personsWantToWorkHere.Contains(person)) return;
        personsWantToWorkHere.Remove(person);
        //SortPersonsWaitingLine();
    }

    public bool IsPersonUse(Person person)
    {
        var firstPerson = personsWantToWorkHere[0];
        return firstPerson == person;
    }
    public Vector3 GetWaitingPosition(Person person)
    {
        if (!personsWantToWorkHere.Contains(person)) return Vector3.zero;
        int index = personsWantToWorkHere.IndexOf(person);
        float offset = 1.5f * index; // Adjust the offset as needed
        Vector3 position = transform.position + transform.forward * offset;
        return position;
    }

    private void SortPersonsWaitingLine()
    {
        personsWantToWorkHere.Sort((x, y) =>
        {

            float sqrDistX = (x.transform.position - transform.position).sqrMagnitude;
            float sqrDistY = (y.transform.position - transform.position).sqrMagnitude;
            return sqrDistX.CompareTo(sqrDistY);
        });
    }



    private void OnDrawGizmos()
    {
        for (int i = 0; i < personsWantToWorkHere.Count; i++)
        {
            var person = personsWantToWorkHere[i];
            var position = GetWaitingPosition(person);
            Gizmos.color = Color.yellow;

            Gizmos.DrawWireSphere(position, 1);
        }
    }




    //TODO add more functions to handle work container
}