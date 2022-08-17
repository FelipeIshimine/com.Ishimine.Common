using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

[CustomPropertyDrawer(typeof(ScriptableObjectDropdownStringAttribute))]
public class ScriptableObjectDropdownStringDrawer : PropertyDrawer
{
    //public override float GetPropertyHeight(SerializedProperty property, GUIContent label) => 32;
    private int _index;
    private List<Object> sObjects;
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        Rect labelPosition = new Rect(position.x, position.y, position.width*.25f, 16f);
        Rect buttonPosition = new Rect(position.x + labelPosition.width, position.y, position.width * .75f, 16f);

        EditorGUI.LabelField(labelPosition, label);

        if (EditorGUI.DropdownButton(buttonPosition, new GUIContent(property.stringValue), FocusType.Keyboard))
        {
            GenericMenu menu = new GenericMenu();
            sObjects = UnityEditorExtensions.FindAssetsByType(((ScriptableObjectDropdownStringAttribute)attribute).TargetType);
            for (var index = 0; index < sObjects.Count; index++)
            {
                int localIndex = index;
                Object obj = sObjects[localIndex];
                menu.AddItem(
                    new GUIContent(obj.name),
                    obj.name == property.stringValue, 
                    ()=> 
                    {
                        property.stringValue = sObjects[localIndex].name;
                        _index = localIndex;
                    });
                menu.ShowAsContext();
            }
        }

        if (sObjects != null)
            property.stringValue = sObjects[_index].name;
        else
        {
            sObjects = UnityEditorExtensions.FindAssetsByType(((ScriptableObjectDropdownStringAttribute)attribute).TargetType);
            _index = sObjects.FindIndex(x => x.name == property.stringValue);
            if (_index == -1)
                _index = 0;
        }
        
    }
    

    public override bool CanCacheInspectorGUI(SerializedProperty property) => true;
}