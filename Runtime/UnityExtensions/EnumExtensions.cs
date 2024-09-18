using System;
using System.Collections.Generic;

public static class EnumExtensions
{
    public static IEnumerable<T> GetIndividualFlags<T>(this T flags) where T : Enum
    {
        foreach (T value in Enum.GetValues(typeof(T)))
        {
            if (flags.HasFlag(value))
                yield return value;
        }
    }
    
    public static int AsIndexFlag<T>(this T flag) where T : Enum => Array.IndexOf(Enum.GetValues(typeof(T)), flag);
}