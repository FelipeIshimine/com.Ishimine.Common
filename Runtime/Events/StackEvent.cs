using System;
using System.Collections.Generic;
using UnityEngine;

public class StackEvent
{
    private readonly Dictionary<object, Action> _callbackStack = new Dictionary<object, Action>();

    private readonly List<object> _sources = new List<object>();
    
    public void Register(object source, Action callback)
    {
        //Debug.Log($"{this} Register({source})");
        _sources.Insert(0, source);
        _callbackStack.Add(source, callback);
    }

    public void Unregister(object source)
    {
        //Debug.Log($"{this} Unregister({source})");
        
        _sources.RemoveAt(0);
        _callbackStack.Remove(source);
    }

    public void Invoke()
    {
        var source = _sources[0];
        //Debug.Log($"{source} taking the call");
        _callbackStack[_sources[0]].Invoke();
    }
}