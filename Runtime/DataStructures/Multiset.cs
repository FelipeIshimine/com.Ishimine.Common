using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Multiset<T>
{
    private Dictionary<T, int> _values = new Dictionary<T, int>();

    public Dictionary<T, int>.KeyCollection Keys => _values.Keys;
    public Dictionary<T, int>.ValueCollection Values => _values.Values;

    public int this[T value] => GetValue(value);

    public int Add(T value)
    {
        if (!_values.TryGetValue(value, out int count))
            _values[value] = 0;
        return ++_values[value];
    }
    
    public int Remove(T value)
    {
        if (!_values.TryGetValue(value, out int count) || count == 0) return 0;
        return --_values[value];
    }

    public int GetValue(T value) => _values[value];

}
