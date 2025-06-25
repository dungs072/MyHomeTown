
using UnityEngine;

[CreateAssetMenu(fileName = "NeedItemData", menuName = "Data/Task/NeedItemData")]
public class NeedItemData : ScriptableObject
{
    [SerializeField] private string itemName;
    [SerializeField] private GameObject itemPrefab;

    public string ItemName => itemName;
    public GameObject ItemPrefab => itemPrefab;
}