using System;
using Unity.VisualScripting;

[Serializable]
public class ItemRequirement
{
    public ItemKey itemKey;
    public int amount = 0;

    public ItemRequirement Clone()
    {
        return new ItemRequirement
        {
            itemKey = this.itemKey,
            amount = this.amount
        };
    }
}