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
}