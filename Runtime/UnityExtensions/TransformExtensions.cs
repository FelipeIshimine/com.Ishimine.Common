using UnityEngine;

public static class TransformExtensions
{
    public static string GetHierarchyAsString(this Transform source, bool includeSceneName = false)
    {
        if(includeSceneName!) return source.parent ? $"{source.parent.GetHierarchyAsString(true)}/{source.name}" : $"{source.gameObject.scene.name}/{source.name}";
        return source.parent ? $"{source.parent.GetHierarchyAsString()}/{source.name}" : source.name;
    }
}