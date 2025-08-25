using System.Collections.Generic;
using UnityEngine;
using static ManagerSingleton;

public class Diner : CustomerBehavior
{
    public Diner(Person person) : base(person)
    {
        this.person = person;
    }
    private MenuSystem menuSystem;
    // protected override void InitSystems()
    // {
    //     base.InitSystems();
    //     menuSystem = EmpireInstance.MenuSystem;
    // }

    protected override List<ItemRequirement> GetNeedItemsFromCurrentToEndStep()
    {
        var needItems = menuSystem.GetMenuItems();
        return needItems;
    }
    protected override bool TryToMeetConditionsToWork()
    {
        return true;

    }

    protected override void TakeItemsFromWorkContainer()
    {

    }


}
