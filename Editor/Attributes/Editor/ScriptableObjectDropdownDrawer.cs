using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;
/*
[CustomPropertyDrawer(typeof(ScriptableObjectDropdownAttribute))]*/
public class ScriptableObjectDropdownDrawer : PropertyDrawer
{
    //public override float GetPropertyHeight(SerializedProperty property, GUIContent label) => 32;
    private int _index;
    private List<Object> sObjects;
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        Rect labelPosition = new Rect(position.x, position.y, position.width*.25f, 16f);
        Rect buttonPosition = new Rect(position.x + labelPosition.width, position.y, position.width * .75f, 16f);

        EditorGUI.LabelField(labelPosition, label);

        ScriptableObject current = property.objectReferenceValue as ScriptableObject;
        
        if (EditorGUI.DropdownButton(buttonPosition, new GUIContent(current != null?current.name:"Null"), FocusType.Keyboard))
        {
            GenericMenu menu = new GenericMenu();

            Debug.Log(property.serializedObject.targetObject.GetType().ToString());
            sObjects = UnityEditorExtensions.FindAssetsByType(property.serializedObject.targetObject.GetType());
            Debug.Log(sObjects.Count);
            for (var index = 0; index < sObjects.Count; index++)
            {
                int localIndex = index;
                Object obj = sObjects[localIndex];
                menu.AddItem(
                    new GUIContent(obj.name),
                    obj.name == current.name, 
                    ()=> 
                    {
                        property.objectReferenceValue = sObjects[localIndex];
                        _index = localIndex;
                        Selection.activeObject = sObjects[localIndex];
                        Debug.Log($"Selection.activeObject:{Selection.activeObject}");
                    });
                menu.ShowAsContext();
            }
        }

        if (sObjects != null && _index >= 0 && _index<sObjects.Count)
            property.objectReferenceValue = sObjects[_index];
        else
        {
            sObjects = UnityEditorExtensions.FindAssetsByType(property.GetType());
            if(current != null) _index = sObjects.FindIndex(x => x.name == current.name);
            if (_index == -1)
                _index = 0;
        }
        
    }
    

    public override bool CanCacheInspectorGUI(SerializedProperty property) => true;
}