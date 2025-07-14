using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AgentManager))]
public class NeedItemGenerator : MonoBehaviour
{
    [SerializeField] private List<NeedItemData> needItems;

    void Awake()
    {
        AgentManager.OnAgentSpawned += HandleAddNeedItem;
    }
    void OnDestroy()
    {
        AgentManager.OnAgentSpawned -= HandleAddNeedItem;
    }

    private void HandleAddNeedItem(AgentController agentController)
    {
        if (agentController.TryGetComponent(out BaseCharacter baseCharacter))
        {
            if (!baseCharacter.ShouldCreateNeedObjectsWhenSpawned) return;
            var needItemData = GetNeedItemData();
            if (needItemData == null) return;
            var amount = GetAmountOfNeedItem();
            //! modify this code here
            //baseCharacter.AddNeedObject();
        }
    }
    private NeedItemData GetNeedItemData()
    {
        if (needItems.Count == 0) return null;
        var randomIndex = Random.Range(0, needItems.Count);
        return needItems[randomIndex];
    }
    private int GetAmountOfNeedItem()
    {
        return Random.Range(1, 5);
    }
}
