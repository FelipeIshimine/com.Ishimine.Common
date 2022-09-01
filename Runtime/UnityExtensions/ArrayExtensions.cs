using System;
using System.Collections.Generic;

public static class ArrayExtensions
{
    public static T Last<T>(this T[] source) => source[source.Length - 1];
    public static T First<T>(this T[] source) => source[0];

    public static object[] Collect<T>(this T[] @this, Unity.Plastic.Antlr3.Runtime.Misc.Func<T,object> func)
    {
        var value = new object[@this.Length];
        for (int i = 0; i < @this.Length; i++)
            value[i] = func.Invoke(@this[i]);
        return value;
    }
    
    
    public static T[] FindAll<T>(this T[] @this, Predicate<T> func)
    {
        var value = new List<T>();
        for (int i = 0; i < @this.Length; i++)
        {
            if(func.Invoke(@this[i]))
                value.Add(@this[i]);
        }
        return value.ToArray();
    }
    
    
    
}