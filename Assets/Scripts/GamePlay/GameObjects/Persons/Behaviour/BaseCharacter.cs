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

    protected List<RequiredItem> needObjects;
    public bool ShouldCreateNeedObjectsWhenSpawned => shouldCreateNeedObjectsWhenSpawned;

    public List<RequiredItem> NeedObjects => needObjects;

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

    public void AddNeedObject(ItemData item)
    {
        needObjects ??= new List<RequiredItem>();

        var needObject = new RequiredItem
        {
            itemData = item,
            gainedAmount = 0
        };
        var itemName = NeedItemKeyNames.ToName(item.itemKey);
        person.InfoPersonUI.SetInfoText($"Need {itemName} x{item.amount}");
        needObjects.Add(needObject);
    }

}
