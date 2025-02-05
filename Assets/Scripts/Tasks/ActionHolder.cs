using System;
using System.Collections.Generic;
using UnityEditor.EditorTools;
using UnityEngine;
public class ActionHolder : MonoBehaviour
{
    [Tooltip("The directions that the person can go to reach this action holder. Please add only one direction")]
    [SerializeField] private DIRECTION[] directions;
    [SerializeField] private ActionData actionData;
    [SerializeField] private bool isBusy = false;

    private List<Person> persons = new List<Person>();
    public ActionData ActionData => actionData;
    private const float SPACE = 5f;
    public bool IsBusy => isBusy;


    public void SetBusy(bool isBusy)
    {
        this.isBusy = isBusy;
    }
    public void AddPerson(Person person)
    {
        persons.Add(person);
    }
    public void RemovePerson(Person person)
    {
        persons.Remove(person);
    }
    // temporary there is only one direction
    public Vector3 GetNextWaitingPoint()
    {
        float distance = persons.Count * SPACE;
        foreach (DIRECTION direction in directions)
        {

            switch (direction)
            {
                case DIRECTION.LEFT:
                    return -transform.right.normalized * distance + transform.position;
                case DIRECTION.RIGHT:
                    return transform.right.normalized * distance + transform.position;
                case DIRECTION.UP:
                    return transform.forward.normalized * distance + transform.position;
                case DIRECTION.DOWN:
                    return -transform.forward.normalized * distance + transform.position;
                default:
                    return transform.position;
            }
        }
        return transform.position;

    }

    private void OnDrawGizmos()
    {
        if (this.isBusy)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, 5f);
        }
        else
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position, 5f);
        }
    }


}
