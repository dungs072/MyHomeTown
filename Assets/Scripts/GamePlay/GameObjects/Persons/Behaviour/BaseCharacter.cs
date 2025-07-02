using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static ManagerSingleton;

[RequireComponent(typeof(Person))]
public class BaseCharacter : MonoBehaviour
{
    [SerializeField] private bool shouldCreateNeedObjectsWhenSpawned = false;

    protected Person person;
    protected TaskHandler taskHandler;

    protected List<NeedObject> needObjects;
    public bool ShouldCreateNeedObjectsWhenSpawned => shouldCreateNeedObjectsWhenSpawned;

    public List<NeedObject> NeedObjects => needObjects;

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

    public void AddNeedObject(NeedItemData item, int neededAmount)
    {
        needObjects ??= new List<NeedObject>();

        var needObject = new NeedObject
        {
            itemData = item,
            neededAmount = neededAmount,
            gainedAmount = 0
        };

        person.InfoPersonUI.SetInfoText($"Need {item.ItemName} x{neededAmount}");
        needObjects.Add(needObject);
    }

}
