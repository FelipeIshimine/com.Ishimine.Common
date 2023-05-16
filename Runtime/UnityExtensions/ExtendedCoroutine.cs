using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExtendedCoroutine : IEnumerator
{
    public object Result { get; private set; }
    private readonly IEnumerator _target;
    private readonly Action _callback;

    public ExtendedCoroutine(IEnumerator target, Action callback = null)
    {
        _target = target;
        _callback = callback;
    }
    
    public bool MoveNext()
    {
        if(_target.MoveNext()) 
        {
            Result = _target.Current;
            return true;
        }
        _callback?.Invoke();
        return false;
    }

    public void Reset() { }

    public object Current => _target.Current;
}

public class ExtendedCoroutine<T> : IEnumerator
{
    public T Result { get; private set; }
    private readonly IEnumerator<T> _target;
    private readonly Action _callback;

    public ExtendedCoroutine(IEnumerator<T> target, Action callback = null)
    {
        _target = target;
        _callback = callback;
    }

    public bool MoveNext()
    {
        if(_target.MoveNext()) 
        {
            Result = _target.Current;
            return true;
        }
        _callback?.Invoke();
        return false;
    }

    public void Reset() { }

    public object Current => _target.Current;
}