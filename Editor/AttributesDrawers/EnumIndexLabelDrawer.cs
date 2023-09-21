using System;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(EnumIndexLabelAttribute))]
public class EnumIndexLabelDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        // Replace label with enum name if possible.
        try
        {
            var config = attribute as EnumIndexLabelAttribute;
            var enum_names = Enum.GetNames(config.EnumType);
            var match = Regex.Match(property.propertyPath, "\\[(\\d)\\]", RegexOptions.RightToLeft);
            int pos = int.Parse(match.Groups[1].Value);
 
            // Make names nicer to read (but won't exactly match enum definition).
            //var enum_label = ObjectNames.NicifyVariableName(enum_names[pos].ToLower());
            label = new GUIContent(enum_names[pos]);
        }
        catch
        {
            // keep default label
        }
        EditorGUI.PropertyField(position, property, label, property.isExpanded);
    }
}