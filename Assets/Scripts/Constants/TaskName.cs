public enum TaskName
{
    NONE = 0,
    DISH_1 = 1,
    DISH_2 = 2,
    DISH_3 = 3,
    DINNER = 4,
    RECEPTION = 5,
}

public static class TaskNameParser
{
    public static string ToName(this TaskName key)
    {
        return key switch
        {
            TaskName.DISH_1 => "Dish 1",
            TaskName.DISH_2 => "Dish 2",
            TaskName.DISH_3 => "Dish 3",
            TaskName.DINNER => "Dinner",
            TaskName.RECEPTION => "Reception",
            _ => "None"
        };
    }
}