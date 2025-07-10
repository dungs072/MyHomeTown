
using UnityEngine;

[CreateAssetMenu(fileName = "NeedItemData", menuName = "Data/Task/NeedItemData")]
public class NeedItemData : ScriptableObject
{
    [SerializeField] private NeedItemKey itemKey;
    [SerializeField] private GameObject itemPrefab;

    public NeedItemKey ItemKey => itemKey;
    public string ItemName => NeedItemKeyNames.ToName(itemKey);
    public GameObject ItemPrefab => itemPrefab;
}