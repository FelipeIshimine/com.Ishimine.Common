using System;
using System.Collections.Generic;
using Unity.Plastic.Newtonsoft.Json.Serialization;
using Action = Unity.Plastic.Newtonsoft.Json.Serialization.Action;

public class StackedBool
{
    public event Action<bool> OnChange;
    
    private readonly Dictionary<object, bool> _values = new Dictionary<object, bool>();
    private readonly List<object> _stack = new List<object>();

    public StackedBool(bool defaultValue)
    {
        Set(defaultValue,this);
    }
    
    public StackedBool(bool defaultValue, Action<bool> onChangeCallback)
    {
        Set(defaultValue,this);
        OnChange = onChangeCallback;
    }

    public bool Contains(object value) => _values.ContainsKey(value);
    
    public void Set(bool value, object source)
    {
        if (Contains(source))
            throw new Exception($"The object {source} already has a value in the stack");

        var old = Get(); 
        _values[source] = value;
        _stack.Insert(0, source);
        if(old != value) OnChange?.Invoke(value);
    }

    public bool Get() => _values[_stack[0]];

    public void Release(object source)
    {
        _values.Remove(source);
        _stack.RemoveAt(_stack.IndexOf(source));
    }
    
    public static implicit operator bool(StackedBool source) => source.Get();

}