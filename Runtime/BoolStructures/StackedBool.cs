using System.Collections.Generic;

public class StackedBool
{
    private readonly Dictionary<object, bool> _values = new Dictionary<object, bool>();
    private readonly List<object> _stack = new List<object>();
    public void Set(bool value, object source)
    {
        _values[source] = value;
        _stack.Insert(0, source);
    }

    public bool Get() => _values[_stack[0]];

    public void Release(object source)
    {
        _values.Remove(source);
        _stack.RemoveAt(_stack.IndexOf(source));
    }

}