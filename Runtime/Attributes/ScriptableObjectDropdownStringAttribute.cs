using System;
using UnityEngine;

[AttributeUsage(AttributeTargets.Field)]
public class ScriptableObjectDropdownStringAttribute : PropertyAttribute
{
    public readonly Type TargetType;

    public ScriptableObjectDropdownStringAttribute(Type targetType)
    {
        this.TargetType = targetType;
    }
    
    
}