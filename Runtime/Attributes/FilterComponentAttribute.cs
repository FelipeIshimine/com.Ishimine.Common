using System;
using UnityEngine;

[AttributeUsage(AttributeTargets.Field)]
public class FilterComponentAttribute : PropertyAttribute
{
    public Type TargetType { get; }

    public FilterComponentAttribute(Type targetType)
    {
        TargetType = targetType;
    }
}