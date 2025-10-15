namespace GameEvents
{
    public static class WkEvents
    {
        public const string OnReadyToUse = "WorkContainer.OnReadyToUse";
        public const string OnBusy = "WorkContainer.OnBusy";
    }

    public static class TaskHandlerEvents
    {
        public const string OnTaskAvailable = "TaskHandler.OnTaskAvailable";
    }
    public static class MovementEvents
    {
        public const string OnMoveStarted = "Movement.OnMoveStarted";
        public const string OnMoveFinished = "Movement.OnMoveFinished";
    }

    public static class WorkerEvents
    {
        public const string OnReachDes = "Worker.OnReachDes";
        public const string OnFinishWork = "Worker.OnFinishWork";

    }
}
