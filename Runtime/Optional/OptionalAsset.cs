using System;
using UnityEngine;

[Serializable]
public class OptionalAsset<T> where T : UnityEngine.Object
{
	[SerializeField] private bool enabled;
	[SerializeField] private T value;

	public bool Enabled
	{
		get => enabled;
		set => enabled = value;
	}

	public T Value => value;

	public OptionalAsset(T initialValue)
	{
		enabled = true;
		value = initialValue;
	}
	public OptionalAsset(T initialValue, bool initialEnable)
	{
		enabled = initialEnable;
		value = initialValue;
	}
    
	public static implicit operator T(OptionalAsset<T> optional) => optional.value;
	public static implicit operator bool(OptionalAsset<T> optional) => optional.enabled;

	public void SetValue(T nValue) => value = nValue;
	private void SetValueThenEnable(T nValue)
	{
		SetValue(nValue);
		enabled = true;
	}

}