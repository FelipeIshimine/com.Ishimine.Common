using System;
using System.Collections.Generic;

public static class ListExtensions
{
    public static T Last<T>(this List<T> source) => source[source.Count - 1];
    public static T First<T>(this List<T> source) => source[0];

    
    public static List<T> ExtractAll<T>(this List<T> source, Func<T, bool> validation)
    {
        List<T> values = new List<T>();
        for (int i = source.Count - 1; i >= 0; i--)
        {
            if(validation.Invoke(source[i]))
            {
                values.Add(source[i]);
                source.RemoveAt(i);
            }
        }
        return values;
    }
    
    public static T Take<T>(this List<T> source, int index, bool removePosition = true) 
    {
        var value = source[index];
        if (removePosition) 
            source.RemoveAt(index);
        else
            source[index] = default;
        return value;
    }

    public static void Shuffle<T>(this List<T> source)
    {
        for (int i = source.Count - 1; i >= 0; i--)
        {
            int j = UnityEngine.Random.Range(0, i);
            (source[i], source[j]) = (source[j], source[i]);
        }
    }

    public static int FindMinIndex<T>(this List<T> source, Func<T, float> function)
    {
        float min = float.MaxValue;
        int current = -1;
        for (int i = 0; i < source.Count; i++)
        {
            float value = function.Invoke(source[i]); 
            if (min > value)
            {
                min = value;
                current = i;
            }
        }
        return current;
    }
    
    public static T FindMin<T>(this List<T> source, Func<T, float> function) => source[FindMinIndex(source, function)];
}