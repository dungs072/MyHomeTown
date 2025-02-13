using System.Collections.Generic;
using UnityEngine;

public class ActionManager : MonoBehaviour
{
    [SerializeField] private List<ActionHolder> actionHolders;
    private Dictionary<string, List<ActionHolder>> actionsDict;
    private void Awake()
    {
        // actionsDict = new Dictionary<string, List<ActionHolder>>();
        // foreach (ActionHolder actionHolder in actionHolders)
        // {
        //     AddActionHolder(actionHolder);
        // }
    }

    public void AddActionHolder(ActionHolder actionHolder)
    {
        string key = actionHolder.ActionData.ActionName;
        if (!actionsDict.ContainsKey(key))
        {
            actionsDict.Add(key, new List<ActionHolder>());
        }
        actionsDict[key].Add(actionHolder);
    }

    public List<ActionHolder> GetPossibleActions(string actionName)
    {
        if (actionsDict.ContainsKey(actionName))
        {
            return actionsDict[actionName];
        }
        return null;
    }
}
