using System;
using Sirenix.OdinInspector;
using UnityEngine;
using Random = UnityEngine.Random;

[System.Serializable]
public class ManualTimer
{
    public event Action OnCompleted;
    public event Action OnRestart;
    public event Action<float> OnTick;
    
    public bool Completed { get; private set; }
    [ShowInInspector] public readonly bool Loop;
    [ShowInInspector] public readonly float Duration;
    [ShowInInspector] private float _currentValue = 0;
    
    public float ProgressUnclamped => _currentValue / Duration;
    public float Progress => Mathf.Clamp01(_currentValue / Duration);

    public float CountdownValue => Duration - _currentValue;
    public float Current => _currentValue;
    public float Remaining => Duration - Current;

    public ManualTimer(float duration, bool loop)
    {
        Duration = duration;
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

        if (_currentValue >= Duration)
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

    public void Restart()
    {
        Completed = Duration == 0;
        _currentValue = 0;
        OnRestart?.Invoke();
    }

    public override string ToString() => $"Timer:{_currentValue:F2}/{Duration:F2}. Progress:{Progress:F2} Loop:{Loop} Completed:{Completed}";

    public static implicit operator bool(ManualTimer manualTimer) => manualTimer.Completed;


}