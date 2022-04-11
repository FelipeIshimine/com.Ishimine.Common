using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(FilterComponentAttribute))]
public class FilterComponentDrawer : PropertyDrawer
{
    private int _index;
    private List<Component> _components;
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        float labelWidth = position.width * .25f;
        float objectFieldWidth = position.width * .375f;
        float dropdownButtonWidth = position.width * .375f;
        
        Rect labelPosition = new Rect(position.x, position.y, position.width*.25f, 16f);
        EditorGUI.LabelField(labelPosition, label);

        Rect objectPosition = new Rect(position.x + labelWidth, position.y, objectFieldWidth, 16f);

        var ogValue = property.objectReferenceValue;
        EditorGUI.ObjectField(objectPosition, property, GUIContent.none);
        var targetType = ((FilterComponentAttribute)attribute).TargetType;

        if (property.objectReferenceValue != ogValue && property.objectReferenceValue != null)
        {
            Component component = (Component)property.objectReferenceValue;
            if (component.GetComponent(targetType) == null)
                property.objectReferenceValue = null;
            else
                SceneView.lastActiveSceneView.Frame(new Bounds(((Component)property.objectReferenceValue).transform.position, Vector3.one*10));
        }
        
        Rect buttonPosition = new Rect(position.x + labelWidth + objectFieldWidth, position.y, dropdownButtonWidth, 16f);
        
        string buttonName = "NULL";
        if (property.objectReferenceValue != null)
        {
            buttonName = ((Component)property.objectReferenceValue).transform.GetHierarchyAsString();
        }
        
        if (EditorGUI.DropdownButton(buttonPosition, new GUIContent(buttonName), FocusType.Keyboard))
        {
            GenericMenu menu = new GenericMenu();
            _components = new List<Component>();

            if (targetType.IsInterface)
            {
                foreach (var monoBehaviour in Object.FindObjectsOfType<Component>(true))
                {
                    var validComponent = monoBehaviour.GetComponent(targetType);
                    if (validComponent != null)
                        _components.Add(validComponent);
                }
            }
            else
            {
                var foundObjects = Object.FindObjectsOfType(targetType, true);
                foreach (var foundObject in foundObjects)
                {
                    if (foundObject is Component go)
                        _components.Add(go);
                }
            }
            
            _components.Insert(0,null);
            
            for (var index = 0; index < _components.Count; index++)
            {
                int localIndex = index;
                Component component = _components[localIndex];
                
                string path =component?component.transform.GetHierarchyAsString():"NULL";

                menu.AddItem(
                    new GUIContent(path),
                    component == property.objectReferenceValue, 
                    ()=> 
                    {
                        property.objectReferenceValue = _components[localIndex];
                        _index = localIndex;
                        if(_components[localIndex]) SceneView.lastActiveSceneView.Frame(new Bounds(_components[localIndex].transform.position, Vector3.one*10));
                    });
                menu.ShowAsContext();
            } 
        }

        if (_components != null && _components.Count > 0)
            property.objectReferenceValue = _components[_index];
    }
    

    public override bool CanCacheInspectorGUI(SerializedProperty property) => true;
}