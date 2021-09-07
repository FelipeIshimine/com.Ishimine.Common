﻿using System;
using Sirenix.OdinInspector;

public class PriorityBool
{
    [ShowInInspector] private bool _value = false;
    [ShowInInspector] private object _source = null;
    [ShowInInspector] private int _priority = -1;

    public bool Set(bool value, object source, int priority)
    {
        if (priority < 0)
            throw new Exception("Priority is below 0");
        if (priority < _priority)
            return false;
        _value = value;
        _source = source;
        _priority = priority;
        return true;
    }

    public void Clear()
    {
        _priority = -1;
        _source = null;
        _value = false;
    }

}