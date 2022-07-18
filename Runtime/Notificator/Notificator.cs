using System;
using System.Collections.Generic;

public class Notificator<T, TB>
{
    private readonly Dictionary<T, Action<TB>> _events = new Dictionary<T, Action<TB>>();

    public void Register(T key, Action<TB> nCallback)
    {
        _events.TryGetValue(key, out var callback);
        callback -= nCallback;
        callback += nCallback;
        _events[key] = callback;
    }

    public void Unregister(T key, Action<TB> nCallback)
    {
        _events.TryGetValue(key, out var callback);
        callback -= nCallback;
        _events[key] = callback;
    }

    public void Send(T id, TB value)
    {
        if(_events.TryGetValue(id, out Action<TB> eventValue))
            eventValue?.Invoke(value);
    }

    public void SendAll(TB value)
    {
        foreach (Action<TB> eventsValue in _events.Values)
            eventsValue?.Invoke(value);
    }
}