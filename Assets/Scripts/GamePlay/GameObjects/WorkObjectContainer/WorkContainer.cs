using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GameController;

public class WorkContainer : MonoBehaviour
{
    // Events
    public static event Action<WorkContainer> OnAvailable;


    // handle task
    [SerializeField] private WorkContainerType workContainerType;
    [SerializeField] private List<ItemKey> possibleContainItems = new();
    [SerializeField] private Transform serverTransform;
    [SerializeField] private Transform puttingTransform;
    public WorkContainerType WorkContainerType => workContainerType;
    private List<Person> personsWantToWorkHere = new();
    public List<Person> PersonsWantToWorkHere => personsWantToWorkHere;
    private Person serverPerson;
    public Person ServerPerson => serverPerson;
    // handle items
    private Dictionary<ItemKey, int> itemsInContainer = new();
    public Dictionary<ItemKey, int> ItemsInContainer => itemsInContainer;
    public List<ItemKey> PossibleContainItems => possibleContainItems;
    void Start()
    {
        if (workContainerType != WorkContainerType.FOOD_STORAGE) return;
        StartCoroutine(AutoSpawnItems());
    }
    public void OnWorkContainerReady()
    {
        OnAvailable?.Invoke(this);
    }
    public void OnWorkContainerDemolished()
    {
        //TODO: Handle work container demolition
    }
    //! for test purpose only
    private IEnumerator AutoSpawnItems()
    {
        while (true)
        {
            yield return new WaitForSeconds(5f);
            DebugStorage();
        }
    }
    private void DebugStorage()
    {
        var screenName = ScreenName.GamePlayScreen.ToString();
        var gamePlayScreen = GameInstance.ScreenManager.GetScreen<GamePlayScreen>(screenName);
        var activityFeed = gamePlayScreen.Container.ActivityFeed;
        activityFeed.AddActivity($"Added 10 Vegetables to {gameObject.name}");

        activityFeed.AddActivity($"Added 5 Meat to {gameObject.name}");
        activityFeed.AddActivity($"Added 20 Fruits to {gameObject.name}");
        AddItemToContainer(ItemKey.VEGETABLE, 10);
        AddItemToContainer(ItemKey.MEAT, 5);
        AddItemToContainer(ItemKey.FRUIT, 20);
    }

    public void AddPersonToWorkContainer(Person person)
    {
        if (personsWantToWorkHere.Contains(person)) return;
        personsWantToWorkHere.Add(person);
        //SortPersonsWaitingLine();
    }
    public void RemovePersonFromWorkContainer(Person person)
    {
        if (!personsWantToWorkHere.Contains(person)) return;
        personsWantToWorkHere.Remove(person);
        if (personsWantToWorkHere.Count == 0)
        {
            OnAvailable?.Invoke(this);
        }
        //SortPersonsWaitingLine();
    }
    public bool HasPersonWaiting()
    {
        return personsWantToWorkHere.Count > 0;
    }

    public bool IsPersonUse(Person person)
    {
        // if (!HasPersonWaiting()) return true;
        var firstPerson = personsWantToWorkHere[0];
        return firstPerson == person || serverPerson == person;
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


    public Vector3 GetServerPosition()
    {
        if (serverTransform != null)
        {
            return serverTransform.position;
        }
        Debug.LogWarning("Server transform is not set for this work container.");
        return transform.position; // Fallback to the container's position
    }
    public void SetServerPerson(Person person)
    {
        serverPerson = person;
    }

    public Vector3 GetPuttingPosition()
    {
        if (puttingTransform != null)
        {
            return puttingTransform.position;
        }
        Debug.LogWarning("Putting transform is not set for this work container.");
        return transform.position; // Fallback to the container's position
    }

    public bool IsPuttingStation()
    {
        return workContainerType == WorkContainerType.PUTTING_STATION;
    }
    public bool IsDiningTable()
    {
        return workContainerType == WorkContainerType.DINING_TABLE;
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
    /// <summary>
    /// If the amount of item is less than 0, it will be removed from the container.
    /// </summary>
    /// <param name="itemKey"></param>
    /// <param name="amount"></param>
    public void AddItemToContainer(ItemKey itemKey, int amount)
    {
        if (itemsInContainer.ContainsKey(itemKey))
        {
            itemsInContainer[itemKey] += amount;
        }
        else
        {
            itemsInContainer[itemKey] = amount;
        }
    }
    public void AddPossibleItemToContainer(ItemKey itemKey, int amount)
    {
        if (!possibleContainItems.Contains(itemKey)) return;
        AddItemToContainer(itemKey, amount);
    }
    public int GetItemFromContainer(ItemKey itemKey)
    {
        if (itemsInContainer.TryGetValue(itemKey, out int amount))
        {
            return amount;
        }
        Debug.LogWarning($"Item {itemKey} not found in container.");
        return 0; // or throw an exception if item not found

    }

    //TODO add more functions to handle work container
}