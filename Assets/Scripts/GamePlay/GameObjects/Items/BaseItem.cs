using UnityEngine;

public class BaseItem : BaseProduct
{
    [SerializeField] private ItemKey itemKey;


    public ItemKey ItemKey => itemKey;
    public string ItemName => ItemKeyNames.ToName(itemKey);
}