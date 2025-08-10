using System.Collections.Generic;
using UnityEngine;
public class CustomerBehaviour : BaseBehaviour
{
    private int currentEndWaitPoint = 0;
    public CustomerBehaviour(Person person) : base(person)
    {
        this.person = person;
    }

    protected override bool TryToMeetConditionsToDoStep()
    {
        var personStatus = person.PersonStatus;
        var currentTaskPerformer = personStatus.CurrentTaskPerformer;
        var step = currentTaskPerformer.GetCurrentStepPerformer();
        var selectedWK = personStatus.CurrentWorkContainer;
        var needItems = step.NeedItems;
        if (needItems == null || needItems.Count == 0) return true;
        foreach (var needItem in needItems)
        {
            var requiredAmount = needItem.amount;
            var itemKey = needItem.itemKey;
            if (!selectedWK.ItemsInContainer.ContainsKey(itemKey)) return false;
            var amountInWC = selectedWK.ItemsInContainer[itemKey];
            if (amountInWC < requiredAmount)
            {
                return false;
            }
        }
        return true;

    }

    protected override void HandleWithItems()
    {
        var personStatus = person.PersonStatus;
        var workContainer = personStatus.CurrentWorkContainer;
        if (workContainer.IsDiningTable())
        {
            AbsorbItems();
            needItemsPack.Clear();
        }
        else
        {
            TakeItemsFromWorkContainer();

        }
    }
    private void AbsorbItems()
    {
        foreach (var itemKey in needItemsPack.Items)
        {
            var amount = needItemsPack.GetAmount(itemKey);
            if (amount <= 0) continue;
            person.Pack.RemoveItem(itemKey, amount);
        }
    }
    protected virtual void TakeItemsFromWorkContainer()
    {

        var personStatus = person.PersonStatus;
        var currentTaskPerformer = personStatus.CurrentTaskPerformer;
        var step = currentTaskPerformer.GetCurrentStepPerformer();
        var selectedWK = personStatus.CurrentWorkContainer;
        var needItems = step.NeedItems;
        if (needItems == null || needItems.Count == 0) return;
        foreach (var needItem in needItems)
        {
            var itemKey = needItem.itemKey;
            if (!selectedWK.ItemsInContainer.TryGetValue(itemKey, out int amount)) return;
            var requiredAmount = needItem.amount;
            if (amount < requiredAmount) return;
            selectedWK.AddItemToContainer(itemKey, -requiredAmount);
            person.Pack.AddItem(itemKey, requiredAmount);
        }
    }
    protected override bool HandleEndTask()
    {
        if (person.PersonStatus.CurrentTaskPerformer != null) return false;
        if (!patrollingSystem) return true;
        var patrollingPath = patrollingSystem.PathDictionary[PatrollingPathKey.BackPath];
        if (patrollingPath == null || patrollingPath.Waypoints.Length == 0) return true;

        var maxIndex = patrollingPath.Waypoints.Length - 1;
        if (currentEndWaitPoint > maxIndex)
        {
            person.gameObject.SetActive(false);
            return true;
        }

        var targetPosition = patrollingPath.Waypoints[currentEndWaitPoint].position;
        //person.SwitchState(PersonState.MOVE);

        if (!agent.IsReachedDestination(targetPosition))
        {
            agent.SetDestination(targetPosition);
        }
        else
        {
            currentEndWaitPoint++;
        }

        return false;
    }
}
