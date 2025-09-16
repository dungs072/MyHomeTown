using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
public class CustomerBehavior : BaseBehavior
{
    public CustomerBehavior(Person person) : base(person)
    {
        this.person = person;
    }
    public override void InitBehavior()
    {
        if (taskHandler.TaskNames.Count == 0)
        {
            stateMachine.ChangeState<IdleState>();
            return;
        }
        base.InitBehavior();
    }
    protected override void UpdatePersonState()
    {
        base.UpdatePersonState();
        var personStatus = person.PersonStatus;
        if (personStatus.CurrentTaskPerformer == null && personStatus.CurrentPatrollingPath == null)
        {
            // release person to the pool
            person.gameObject.SetActive(false);
        }
    }

    protected override bool TryToMeetConditionsToWork()
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
    public override void HandleFinishedStep()
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
        base.HandleFinishedStep();
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
    protected override void HandleEndTask()
    {
        var personStatus = person.PersonStatus;
        if (personStatus.CurrentTaskPerformer != null) return;
        if (!patrollingSystem) return;
        personStatus.CurrentPatrollingPath = patrollingSystem.PathDictionary[PatrollingPathKey.BackPath];
        stateMachine.ChangeState<PatrollingState>();
    }
}
