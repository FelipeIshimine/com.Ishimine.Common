using System;
using System.Collections.Generic;
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

[CustomPropertyDrawer(typeof(ScriptableObjectDropdownAttribute))]
public class ScriptableObjectDropdownDrawer : PropertyDrawer
{
    //public override float GetPropertyHeight(SerializedProperty property, GUIContent label) => 32;
    private int _index;
    private List<Object> sObjects;
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        Rect buttonPosition;
        if (!string.IsNullOrEmpty(label.text))
        {
            Rect labelPosition = new Rect(position.x, position.y, position.width*.50f, 16f);
            buttonPosition= new Rect(position.x + labelPosition.width, position.y, position.width * .50f, 16f);
            EditorGUI.LabelField(labelPosition, label);
        }
        else
            buttonPosition = new Rect(position.x , position.y, position.width, 16f);

        string overrideAsBackslash = (attribute as ScriptableObjectDropdownAttribute).OverrideAsBackSlash;
        
        
            
        ScriptableObject current = property.objectReferenceValue as ScriptableObject;

        Type targetType = fieldInfo.FieldType;
            
        //Debug.Log($"Current:{current}");
        if (EditorGUI.DropdownButton(buttonPosition, new GUIContent(current != null?current.name:"Null"), FocusType.Keyboard))
        {
            GenericMenu menu = new GenericMenu();
            sObjects = UnityEditorExtensions.FindAssetsByType(targetType);
            sObjects.Insert(0, null);
            for (var index = 0; index < sObjects.Count; index++)
            {
                int localIndex = index;
                Object obj = sObjects[localIndex];

                string path = obj != null ? obj.name : "Null";

                if (!string.IsNullOrEmpty(overrideAsBackslash))
                    path = path.Replace(overrideAsBackslash, "/");
                
                menu.AddItem(
                    new GUIContent(path),
                    obj == current, 
                    ()=> 
                    {
                        property.objectReferenceValue = sObjects[localIndex];
                        _index = localIndex;
                    });
            }
            
            menu.ShowAsContext();
        }

        if (sObjects != null && _index >= 0 && _index < sObjects.Count)
            property.objectReferenceValue = sObjects[_index];
        else
        {
            sObjects = UnityEditorExtensions.FindAssetsByType(targetType);
            if(current != null) _index = sObjects.FindIndex(x => x.name == current.name);
            if (_index == -1)
                _index = 0;
        }
        
    }


    private string GetProperName(SerializedProperty property)
    {
        if (!property.type.StartsWith("PPtr<"))
            return property.type;
        return property.type.Substring(5).Trim('<', '>', '$');
    }

    public override bool CanCacheInspectorGUI(SerializedProperty property) => true;
}