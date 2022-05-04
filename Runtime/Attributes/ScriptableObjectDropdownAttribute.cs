using System;
using UnityEngine;
[AttributeUsage(AttributeTargets.Field)]
public class ScriptableObjectDropdownAttribute : PropertyAttribute
{
    public readonly string OverrideAsBackSlash;

    public ScriptableObjectDropdownAttribute()
    {
        OverrideAsBackSlash = null;
    }
    
    
    public ScriptableObjectDropdownAttribute(string overrideAsBackSlash)
    {
        OverrideAsBackSlash = overrideAsBackSlash;
    }
}