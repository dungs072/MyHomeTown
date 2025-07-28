public class GatheredItem
{
    public ItemRequirement itemData;
    public int gainedAmount = 0;
    private ItemKey itemKey;
    private int v;

    public GatheredItem(ItemKey itemKey, int v)
    {
        this.itemKey = itemKey;
        this.v = v;
    }
}