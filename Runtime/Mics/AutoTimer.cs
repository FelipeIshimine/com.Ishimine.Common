using System;
using UnityEngine;

[System.Serializable]
public class AutoTimer : ManualTimer, IDisposable
{
	public readonly DeltaTimeType DeltaTimeType;

	private Func<float> _getDelta;

	private bool _running = true;

	public AutoTimer(float duration, bool loop, DeltaTimeType deltaTimeType) : base(duration, loop)
	{
		DeltaTimeType = deltaTimeType;
		SetDelta(deltaTimeType);
	}

	public AutoTimer(float duration, bool loop, DeltaTimeType deltaTimeType, Action callback) : base(duration, loop, callback)
	{
		DeltaTimeType = deltaTimeType;
		SetDelta(deltaTimeType);
	}

	public AutoTimer(float duration, bool loop, DeltaTimeType deltaTimeType, Action<float> onTickProgressCallback, Action callback) : base(duration, loop, onTickProgressCallback, callback)
	{
		DeltaTimeType = deltaTimeType;
		SetDelta(deltaTimeType);
	}
	

	private void SetDelta(DeltaTimeType deltaTimeType)
	{
		switch (deltaTimeType)
		{
			case DeltaTimeType.deltaTime:
				_getDelta = () => Time.deltaTime;
				break;
			case DeltaTimeType.fixedDeltaTime:
				_getDelta = () => Time.fixedDeltaTime;
				break;
			case DeltaTimeType.unscaledDeltaTime:
				_getDelta = () => Time.unscaledDeltaTime;
				break;
			case DeltaTimeType.fixedUnscaledDeltaTime:
				_getDelta = () => Time.fixedUnscaledDeltaTime;
				break;
			default:
				throw new ArgumentOutOfRangeException(nameof(deltaTimeType), deltaTimeType, null);
		}
		Register();
	}
    
	private void Register()
	{
		switch (DeltaTimeType)
		{
			case DeltaTimeType.deltaTime:
			case DeltaTimeType.unscaledDeltaTime:
				GlobalUpdate.OnUpdateEvent += Tick;
				break;
			case DeltaTimeType.fixedDeltaTime:
			case DeltaTimeType.fixedUnscaledDeltaTime:
				GlobalUpdate.OnFixedUpdateEvent += Tick;
				break;
			default:
				throw new ArgumentOutOfRangeException();
		}
	}

	private void Unregister()
	{
		switch (DeltaTimeType)
		{
			case DeltaTimeType.deltaTime:
			case DeltaTimeType.unscaledDeltaTime:
				GlobalUpdate.OnUpdateEvent -= Tick;
				break;
			case DeltaTimeType.fixedDeltaTime:
			case DeltaTimeType.fixedUnscaledDeltaTime:
				GlobalUpdate.OnFixedUpdateEvent -= Tick;
				break;
			default:
				throw new ArgumentOutOfRangeException();
		}
	}

	public void Dispose()
	{
		Unregister();
		_getDelta = null;
	}

	private void Tick()
	{
		if(!_running || _getDelta == null) return;
		Tick(_getDelta.Invoke());
	}

	public void Pause() => _running = false;
	public void Resume() => _running = true;
}