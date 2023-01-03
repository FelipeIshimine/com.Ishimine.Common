using UnityEngine;

[System.AttributeUsage(System.AttributeTargets.Field, AllowMultiple = false)]
public class AutoGetComponentAttribute : PropertyAttribute
{
    public readonly System.Type componentType;

    public AutoGetComponentAttribute(System.Type componentType)
    {
        this.componentType = componentType;
    }
}