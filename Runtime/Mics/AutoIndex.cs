using System;
using Sirenix.OdinInspector;
using UnityEngine;

public class AutoIndex
{
    private readonly int _maxValue;
    private readonly Func<int> _maxValueFunc;

    [ShowInInspector]public int MaxValue => _maxValueFunc?.Invoke() ?? _maxValue;
    [ShowInInspector]public int Value { get; private set; } = 0;

    public AutoIndex(int maxValue)
    {
        _maxValue = maxValue;
    }
    
    public AutoIndex(Func<int> maxValueFunc)
    {
        _maxValueFunc = maxValueFunc;
    }

       
    public int Next()
    {
        Value = (int)Mathf.Repeat(Value + 1, MaxValue);
        return Value;
    }
    public int Previous()
    {
        Value = (int)Mathf.Repeat(Value - 1, MaxValue);
        return Value;
    }
    public void Set(int index)
    {
        if (index < 0)
            throw new Exception("No negative numbers");
        Value = index;
    }
    
    
  
}