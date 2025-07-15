public enum ItemKey
{
    // add need item keys here
    VEGETABLE = 0,
    MEAT = 1,
    FRUIT = 2
}
public static class ItemKeyNames
{
    public static string ToName(this ItemKey key)
    {
        return key switch
        {
            ItemKey.VEGETABLE => "vegetable",
            ItemKey.MEAT => "meat",
            ItemKey.FRUIT => "fruit",
            _ => "unknown"
        };
    }
}