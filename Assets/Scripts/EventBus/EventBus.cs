using System;
using System.Collections.Generic;

public static class EventBus
{
    private static readonly Dictionary<string, Action<object>> events = new();

    public static void Subscribe(string eventName, Action<object> callback)
    {
        if (!events.ContainsKey(eventName))
            events[eventName] = delegate { };
        events[eventName] += callback;
        UnityEngine.Debug.Log($"[EventBus] Subscribed: {eventName}");
    }

    public static void Unsubscribe(string eventName, Action<object> callback)
    {
        if (!events.ContainsKey(eventName)) return;
        events[eventName] -= callback;
        UnityEngine.Debug.Log($"[EventBus] Unsubscribed: {eventName}");
    }

    public static void Publish(string eventName, object data = null)
    {
        if (!events.ContainsKey(eventName)) return;
        events[eventName]?.Invoke(data);
        UnityEngine.Debug.Log($"[EventBus] Emit: {eventName}");
    }
}
