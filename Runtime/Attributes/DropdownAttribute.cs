using System;
using UnityEngine;

[AttributeUsage(AttributeTargets.Field)]
public class DropdownAttribute : PropertyAttribute
{
    public readonly bool SplitByHierarchy;

    public DropdownAttribute(bool splitByHierarchy = true)
    {
        SplitByHierarchy = splitByHierarchy;
    }
}