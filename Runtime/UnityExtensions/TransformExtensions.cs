using UnityEngine;

public static class TransformExtensions
{
    public static string GetHierarchyAsString(this Transform source, bool includeSceneName = false)
    {
        if(includeSceneName!) return source.parent ? $"{source.parent.GetHierarchyAsString(true)}/{source.name}" : $"{source.gameObject.scene.name}/{source.name}";
        return source.parent ? $"{source.parent.GetHierarchyAsString()}/{source.name}" : source.name;
    }
    
    
    public static void SetGlobalScale (this Transform transform, Vector3 globalScale)
    {
        transform.localScale = Vector3.one;
        transform.localScale = new Vector3 (globalScale.x/transform.lossyScale.x, globalScale.y/transform.lossyScale.y, globalScale.z/transform.lossyScale.z);
    }
}

