using System.Collections.Generic;
using UnityEngine;
using static ManagerSingleton;

public class Diner : CustomerCharacter
{
    private MenuSystem menuSystem;
    protected override void InitSystems()
    {
        base.InitSystems();
        menuSystem = EmpireInstance.MenuSystem;
    }

    protected override List<GatheredItem> GetNeedItemsFromCurrentToEndStep()
    {
        var needItems = menuSystem.GetMenuItems();
        return needItems;
    }
    protected override bool TryToMeetConditionsToDoStep()
    {
        var personStatus = person.PersonStatus;
        var currentTaskPerformer = personStatus.CurrentTaskPerformer;
        var step = currentTaskPerformer.GetCurrentStepPerformer();
        var selectedWK = personStatus.CurrentWorkContainer;
        var needItems = needItemsList;
        if (needItems == null || needItems.Count == 0) return true;
        foreach (var needKey in needItems)
        {
            var requiredAmount = needItemsDict[needKey];
            if (!selectedWK.ItemsInContainer.ContainsKey(needKey)) return false;
            var amountInWC = selectedWK.ItemsInContainer[needKey];
            if (amountInWC < requiredAmount)    
            {
                return false;
            }
        }
        return true;

    }

    protected override void TakeItemsFromWorkContainer()
    {

    }


}
