using System;
using UnityEngine;

[Serializable]
public class Optional<T>
{
    [SerializeField] private bool enabled;
    [SerializeField] private T value;

    public bool Enabled
    {
	    get => enabled;
	    set => enabled = value;
    }

    public T Value => value;

    public Optional(T initialValue)
    {
        enabled = true;
        value = initialValue;
    }
    public Optional(T initialValue, bool initialEnable)
    {
        enabled = initialEnable;
        value = initialValue;
    }
    
    public static implicit operator T(Optional<T> optional) => optional.value;
    public static implicit operator bool(Optional<T> optional) => optional.enabled;

    public void SetValue(T nValue) => value = nValue;
    private void SetValueThenEnable(T nValue)
    {
	    SetValue(nValue);
	    enabled = true;
    }

}