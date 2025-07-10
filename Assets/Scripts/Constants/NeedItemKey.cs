public enum NeedItemKey
{
    // add need item keys here
    VEGETABLE = 0,
    MEAT = 1,
    FRUIT = 2
}
public static class NeedItemKeyNames
{
    public static string ToName(this NeedItemKey key)
    {
        return key switch
        {
            NeedItemKey.VEGETABLE => "vegetable",
            NeedItemKey.MEAT => "meat",
            NeedItemKey.FRUIT => "fruit",
            _ => "unknown"
        };
    }
}