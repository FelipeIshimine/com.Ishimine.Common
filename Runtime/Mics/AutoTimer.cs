using System;
using Sirenix.OdinInspector;
using UnityEngine;

[System.Serializable]
public class AutoTimer
{
    public event Action OnCompleted;
    public event Action OnRestart;
    public event Action<float> OnTick;
    
    public bool Completed { get; private set; }
    [ShowInInspector] public readonly bool Loop;
    [ShowInInspector] public readonly float Duration;
    [ShowInInspector] private float _currentValue = 0;
    
    public float Progress => _currentValue / Duration;
    public float CountdownValue => Duration - _currentValue;
    public float Current => _currentValue;
    public float Remaining => Duration - Current;

    public AutoTimer(float duration, bool loop)
    {
        Duration = duration;
        Completed = duration == 0;
        Loop = loop;
    }
    
    public AutoTimer(float duration, bool loop, Action callback)
    {
        Duration = duration;
        Completed = duration == 0;
        Loop = loop;
        OnCompleted += callback;
    }

    public AutoTimer(float duration, bool loop, Action<float> onTickProgressCallback, Action callback) : this(duration, loop, callback)
    {
        OnTick = onTickProgressCallback;
    }


    public bool Tick(float delta)
    {
        if(Completed) 
            return true;
        _currentValue += delta;

        if (_currentValue >= Duration)
        {
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
}