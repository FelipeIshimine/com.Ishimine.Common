using UnityEngine;

public static class GameObjectExtensions
{
    public static T GetOrAddComponent<T>(this GameObject source) where T : Component
    {
        var component = source.GetComponent<T>();
        if (component == null)
            component = source.AddComponent<T>();
        return component;
    }

}