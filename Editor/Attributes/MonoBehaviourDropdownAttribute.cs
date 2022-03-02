using System;
using UnityEngine;

[AttributeUsage(AttributeTargets.Field)]
public class MonoBehaviourDropdownAttribute : PropertyAttribute
{
    public readonly bool SplitByHierarchy;

    public MonoBehaviourDropdownAttribute(bool splitByHierarchy = true)
    {
        SplitByHierarchy = splitByHierarchy;
    }
}