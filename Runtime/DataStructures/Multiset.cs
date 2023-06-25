using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class Multiset<T> : IEnumerable<KeyValuePair<T,int>>, IEnumerable<(T,int)>
{
    [ShowInInspector] private readonly Dictionary<T, int> _values = new Dictionary<T, int>();

    public Dictionary<T, int>.KeyCollection Keys => _values.Keys;
    public Dictionary<T, int>.ValueCollection Values => _values.Values;

    public int this[T key]
    {
        get => GetValue(key);
        set => SetValue(key, value);
    }

    public int Add(T value)
    {
        if (!_values.TryGetValue(value, out int count))
            _values[value] = 0;
        return ++_values[value];
    }
    
    public int Remove(T value)
    {
        if (!_values.TryGetValue(value, out int count) || count == 0) return 0;
        int result = --_values[value];
        if (result == 0) _values.Remove(value);
        return result;
    }

    public int GetValue(T value)
    {
        if (_values.TryGetValue(value, out var count)) return count;
        return 0;
    }

    private void SetValue(T key, int value) => _values[key] = value;

    
    public T GetMax()
    {
        T value = default;
        int maxValue = -1;

        var keys = new List<T>(Keys);
        for (int i = 0; i < keys.Count; i++)
        {
            if (_values[keys[i]] > maxValue)
            {
                value = keys[i];
                maxValue = _values[keys[i]];
            }
        }

        return value;
    }

    IEnumerator<(T, int)> IEnumerable<(T, int)>.GetEnumerator()
    {
        foreach (var pair in _values)
            yield return (pair.Key,pair.Value);
    }
    
    IEnumerator<KeyValuePair<T, int>> IEnumerable<KeyValuePair<T, int>>.GetEnumerator() =>_values.GetEnumerator();
    public IEnumerator GetEnumerator() =>_values.GetEnumerator();
}
