using System;
using System.Collections.Generic;

public class GenericStackEvent<T>
{
    private readonly Dictionary<object, Action<T>> _callbackStack = new Dictionary<object, Action<T>>();

    private readonly List<object> _sources = new List<object>();
    
    public void Register(object source, Action<T> callback)
    {
        _sources.Add(source);
        _callbackStack.Add(source, callback);
    }

    public void Unregister(object source)
    {
        _sources.RemoveAt(0);
        _callbackStack.Remove(source);
    }

    public void Invoke(T parameter)=>   _callbackStack[_sources[0]].Invoke(parameter);
}