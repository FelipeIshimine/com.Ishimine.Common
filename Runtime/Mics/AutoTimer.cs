using System;
using Sirenix.OdinInspector;

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
    public float CurrentValue => _currentValue;

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

    public void Tick(float delta)
    {
        if(Completed) return;
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
    }

    public void Restart()
    {
        Completed = Duration == 0;
        _currentValue = 0;
        OnRestart?.Invoke();
    }

    public override string ToString() => $"Timer:{_currentValue:F2}/{Duration:F2}. Progress:{Progress:F2}";
}