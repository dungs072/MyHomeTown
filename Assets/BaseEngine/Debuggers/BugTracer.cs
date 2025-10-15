namespace BaseEngine.Debuggers
{
    public static class BugTracer
    {
        public static void Trace(string message)
        {
            UnityEngine.Debug.LogError($"[BugTracer] {message}");
        }
        public static void Log(string message)
        {
            UnityEngine.Debug.Log($"[BugTracer] {message}");
        }
    }
}