using System.Collections.Generic;
using UnityEngine;

public class Receptionist : BaseBehaviour
{
    private Pack orderItems;

    public Receptionist(Person person) : base(person)
    {
        this.person = person;
        orderItems = new Pack(-1);
    }

    #region Order items

    public void AddOrderItems(List<ItemRequirement> itemsToAdd)
    {
        orderItems.AddItems(itemsToAdd);
    }

    public void AddOrderItem(ItemKey itemKey, int amount)
    {
        orderItems.AddItem(itemKey, amount);
    }

    public void ClearItemsOrder(List<ItemKey> itemsToClear)
    {
        orderItems.RemoveItems(itemsToClear);
    }

    #endregion

    #region Handle Task
    // receptionist character does not patrol
    protected override bool StartPatrollingOverTime(string key = PatrollingPathKey.DefaultPath)
    {
        return true;
    }

    protected override void HandleStartTask()
    {
        var personStatus = person.PersonStatus;
        var selectedWK = personStatus.CurrentWorkContainer;
        selectedWK.SetServerPerson(person);
    }

    protected override Vector3 GetTargetPosition()
    {
        var personStatus = person.PersonStatus;
        var selectedWK = personStatus.CurrentWorkContainer;
        return selectedWK.GetServerPosition();
    }
    protected override bool CanWork()
    {
        var selectedWK = person.PersonStatus.CurrentWorkContainer;
        return selectedWK.IsPersonUse(person);
    }

    protected override void DoWork()
    {
        var personStatus = person.PersonStatus;
        var currentStep = personStatus.CurrentTaskPerformer.GetCurrentStepPerformer();
        //? do infinite work
        currentStep.SetProgress(-2);
        //person.SwitchState(PersonState.WORK);
    }

    protected override bool HandleEndTask()
    {
        var personStatus = person.PersonStatus;
        var selectedWK = personStatus.CurrentWorkContainer;
        //? reset the server person
        selectedWK.SetServerPerson(null);
        return true;
    }
    #endregion
}
