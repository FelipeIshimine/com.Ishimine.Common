using System.Collections.Generic;
using UnityEngine;

public static class ObjectExtensions
{
    public static bool IsNameUsed<T>(this T source) where T : Object
    {
        var list = new List<T>(Object.FindObjectsOfType<T>());
        list.Remove(source);
        return list.Find(x => x.name == source.name);
    }

    
}