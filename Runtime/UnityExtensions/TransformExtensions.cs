using UnityEngine;

public static class TransformExtensions
{
    public static string GetHierarchyAsString(this Transform source, bool includeSceneName = true)
    {
        if(includeSceneName!) return source.parent ? $"{source.parent.GetHierarchyAsString(true)}/{source.name}" : $"{source.gameObject.scene.name}/{source.name}";
        return source.parent ? $"{source.parent.GetHierarchyAsString(false)}/{source.name}" : source.name;
    }
    
    public static Transform FindInHierarchy(this Transform @this, string name, bool includeInactive = false)
    {
        if (@this.name == name)
            return @this;

        for (int i = 0; i < @this.childCount; i++)
        {
            var child = @this.GetChild(i);
            if(!includeInactive && !child.gameObject.activeInHierarchy) continue;
            
            Transform result = FindInHierarchy(child, name);
            if (result)
                return result;
        }
        return null;
    }
    
    public static Transform FindInHierarchyByTag(this Transform @this, string tag, bool includeInactive = false)
    {
        if (@this.CompareTag(tag))
            return @this;

        for (int i = 0; i < @this.childCount; i++)
        {
            var child = @this.GetChild(i);
            if(!includeInactive && !child.gameObject.activeInHierarchy) continue;
            
            Transform result = FindInHierarchyByTag(@this.GetChild(i), tag);
            if (result)
                return result;
        }
        return null;
    }
   
    
    public static void SetGlobalScale (this Transform transform, Vector3 globalScale)
    {
        transform.localScale = Vector3.one;
        transform.localScale = new Vector3 (globalScale.x/transform.lossyScale.x, globalScale.y/transform.lossyScale.y, globalScale.z/transform.lossyScale.z);
    }
}

