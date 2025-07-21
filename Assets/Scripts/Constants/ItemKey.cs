public enum ItemKey
{
    // add need item keys here
    VEGETABLE = 0,
    MEAT = 1,
    FRUIT = 2,
    DISH_1 = 3,
    DISH_2 = 4,
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
            ItemKey.DISH_1 => "dish_1",
            ItemKey.DISH_2 => "dish_2",
            _ => "unknown"
        };
    }
}