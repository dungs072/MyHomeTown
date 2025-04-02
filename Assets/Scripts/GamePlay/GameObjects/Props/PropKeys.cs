using System.Collections.Generic;

public enum PropKeys
{
    CookingBase,
    WallBase,
    Wall,
    Cooking,
    Table,
    Chair,
    Bed,
    Toilet,
    Sink,
    Shower,
    BathTub,
    Fridge,
    Oven,
    Microwave,
    WashingMachine,
    Dryer,
    Dishwasher,
}

public static class PropKeyDescriptions
{
    public static readonly Dictionary<PropKeys, string> Descriptions = new Dictionary<PropKeys, string>
    {
        { PropKeys.CookingBase, "Cooking Base" },
        { PropKeys.WallBase, "Wall Base" },
        { PropKeys.Wall, "Wall" },
        { PropKeys.Cooking, "Cooking" },
        { PropKeys.Table, "Table" },
        { PropKeys.Chair, "Chair" },
        { PropKeys.Bed, "Bed" },
        { PropKeys.Toilet, "Toilet" },
        { PropKeys.Sink, "Sink" },
        { PropKeys.Shower, "Shower" },
        { PropKeys.BathTub, "Bath Tub" },
        { PropKeys.Fridge, "Fridge" },
        { PropKeys.Oven, "Oven" },
        { PropKeys.Microwave, "Microwave" },
        { PropKeys.WashingMachine, "Washing Machine" },
        { PropKeys.Dryer, "Dryer" },
        { PropKeys.Dishwasher, "Dishwasher" },
    };
}