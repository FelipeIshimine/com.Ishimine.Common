using System;
using UnityEngine;
[AttributeUsage(AttributeTargets.Field)]
public class ScriptableObjectDropdownAttribute : PropertyAttribute
{
    public readonly Type TargetType;
    public ScriptableObjectDropdownAttribute(Type type)
    {
        TargetType = type;
    }
}