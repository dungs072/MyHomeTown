using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static ManagerSingleton;

[RequireComponent(typeof(Person))]
public class BaseCharacter : MonoBehaviour
{

    protected Person person;
    protected TaskHandler taskHandler;

   

    void Awake()
    {
        InitComponents();
    }

    protected virtual void InitComponents()
    {
        person = GetComponent<Person>();
        taskHandler = GetComponent<TaskHandler>();
    }

    void Start()
    {
        //StartCoroutine(HandlePreTasks());
    }
    protected virtual void OnAllTasksCompleted()
    {
        gameObject.SetActive(false);
    }



}
