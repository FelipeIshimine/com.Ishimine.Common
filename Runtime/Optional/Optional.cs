using System;
using Optional;
using UnityEngine;

[Serializable]
public class Optional<T> : ISerializationCallbackReceiver
{
#if UNITY_EDITOR
	[SerializeField,HideInInspector] private string name;
#endif
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

    public void OnBeforeSerialize()
    {
	    #if UNITY_EDITOR
	    if (value is IName iName)
	    {
		    name = iName.GetName();
	    }
	    #endif
    }

    public void OnAfterDeserialize()
    {
    }

    public T TryGet(T valueOnFail = default) => Enabled ? Value : valueOnFail;
}