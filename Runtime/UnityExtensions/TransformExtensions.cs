using UnityEngine;

public static class TransformExtensions
{
    public static string GetHierarchyAsString(this Transform source)
    {
        return source.parent ? $"{source.parent.GetHierarchyAsString()}/{source.name}" : source.name;
    }
}