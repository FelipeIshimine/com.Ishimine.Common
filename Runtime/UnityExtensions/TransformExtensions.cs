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
    
    public static void LookAt2D(this Transform source, Vector2 target)
    {
        Vector2 direction = target - (Vector2)source.transform.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        source.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }
    
    public static void LookAt2D(this Transform source, Vector2 target, bool aimWithUp)
    {
        Vector2 direction = target - (Vector2)source.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        if (aimWithUp) angle -= 90f;
        source.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }
}

