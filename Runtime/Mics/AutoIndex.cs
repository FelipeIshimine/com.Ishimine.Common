using System;
using UnityEngine;

[System.Serializable]
public class AutoIndex
{
    private readonly int _maxValue;
    private readonly Func<int> _maxValueFunc;

    public int Value { get; private set; } = 0;
    public int MaxValue => _maxValueFunc?.Invoke() ?? _maxValue;

    private readonly Func<int,int> _calculate;

    public enum Mode { Loop, PingPong }

    private readonly Mode _mode;
    
    public AutoIndex(int maxValue, Mode mode = Mode.Loop)
    {
        _mode = mode;
        _maxValue = maxValue;
        if (mode == Mode.Loop)
            _calculate = CalculateRepeat;
        else
            _calculate = CalculatePingPong;
    }
    
    public AutoIndex(Func<int> maxValueFunc, Mode mode = Mode.Loop)
    {
        _maxValueFunc = maxValueFunc;
        if (mode == Mode.Loop)
            _calculate = CalculateRepeat;
        else
            _calculate = CalculatePingPong;
    }
       
    public int Next()
    {
        Value = _calculate.Invoke(1);
        return Value;
    }
    public int Previous()
    {
        Value = _calculate.Invoke(-1);
        return Value;
    }
    public void Set(int index)
    {
        if (index < 0)
            throw new Exception("No negative numbers");
        Value = index;
    }

    public static implicit operator int(AutoIndex autoIndex) => autoIndex.Value;

    private int CalculateRepeat(int offset) =>  (int)Mathf.Repeat(Value + offset, MaxValue);
    
    private int CalculatePingPong(int offset)
    {
        float nValue = (offset + Value);
        nValue =(int)Mathf.PingPong(nValue, MaxValue-1);
        return Mathf.RoundToInt(nValue);
    }
    public override string ToString() => $"Value:{Value} MaxValue:{MaxValue} Mode:{_mode.ToString()}";
}