using System;
using UnityEngine;

[System.Serializable]
public class ManualTimer
{
    public event Action OnCompleted;
    public event Action OnRestart;
    public event Action<float> OnTick;
    
    public bool Completed { get; private set; }
     public readonly bool Loop;
    
     public float Duration => _duration;
    private float _duration;
     protected float _currentValue = 0;
    
    public float ProgressUnclamped => _currentValue / _duration;
    public float Progress => Mathf.Clamp01(_currentValue / _duration);
    public float CountdownValue => _duration - _currentValue;
    public float Current => _currentValue;
    public float Remaining => _duration - Current;

    public ManualTimer(float duration, bool loop)
    {
        _duration = duration;
        Completed = duration == 0;
        Loop = loop;
    }

    public ManualTimer(float duration, bool loop, Action callback) : this(duration,loop)
    {
        OnCompleted += callback;
    }

    public ManualTimer(float duration, bool loop, Action<float> onTickProgressCallback, Action callback) : this(duration, loop, callback)
    {
        OnTick = onTickProgressCallback;
    }

    private void Update() => Tick(Time.deltaTime);

    public bool Tick(float delta)
    {
        if(Completed) 
            return true;
        _currentValue += delta;

        if (_currentValue >= _duration)
        {
            OnTick?.Invoke(1);
            OnCompleted?.Invoke();
            if (Loop)
                _currentValue = 0;
            else
                Completed = true;
        }
        else
            OnTick?.Invoke(Progress);
        return Completed;
    }

    public virtual void Restart()
    {
        Completed = _duration == 0;
        _currentValue = 0;
        OnRestart?.Invoke();
    }

    public override string ToString() => $"Timer:{_currentValue:F2}/{_duration:F2}. Progress:{Progress:F2} Loop:{Loop} Completed:{Completed}";

    public static implicit operator bool(ManualTimer manualTimer) => manualTimer.Completed;

    public void SetDuration(float nDuration) => _duration = nDuration;

}