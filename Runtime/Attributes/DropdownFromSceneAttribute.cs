using System;
using UnityEngine;

[AttributeUsage(AttributeTargets.Field)]
public class DropdownFromSceneAttribute : PropertyAttribute
{
    public readonly bool SplitByHierarchy;

    public DropdownFromSceneAttribute(bool splitByHierarchy = true)
    {
        SplitByHierarchy = splitByHierarchy;
    }
}