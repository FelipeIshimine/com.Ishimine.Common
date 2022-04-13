using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(DropdownFromSceneAttribute))]
public class DropdownFromSceneDrawer : PropertyDrawer
{
    //public override float GetPropertyHeight(SerializedProperty property, GUIContent label) => 32;
    private int _index;
    private List<MonoBehaviour> _monoBehaviours;
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        bool drawLabel = !string.IsNullOrEmpty(label.text);
        
        float labelWidth = position.width * .25f;
        
        float objectFieldWidth = position.width * .375f;
        float dropdownButtonWidth = position.width * .375f;
        
        if(drawLabel)
        {
            Rect labelPosition = new Rect(position.x, position.y, position.width * .25f, 16f);
            EditorGUI.LabelField(labelPosition, label);
        }
        else
        {
            objectFieldWidth += labelWidth/2;
            dropdownButtonWidth += labelWidth/2;
            labelWidth = 0;
        }
        
        
        
        Rect objectPosition = new Rect(position.x + labelWidth, position.y, objectFieldWidth, 16f);

        var ogValue = property.objectReferenceValue;
        EditorGUI.ObjectField(objectPosition, property, GUIContent.none);

        if (property.objectReferenceValue != ogValue && property.objectReferenceValue != null)
            SceneView.lastActiveSceneView.Frame(new Bounds(((MonoBehaviour)property.objectReferenceValue).transform.position, Vector3.one*10));
        
        Rect buttonPosition = new Rect(position.x + labelWidth + objectFieldWidth, position.y, dropdownButtonWidth, 16f);
        bool splitByHierarchy = ((DropdownFromSceneAttribute)attribute).SplitByHierarchy;

        string buttonName = "NULL";
        if (property.objectReferenceValue != null)
            buttonName = ((MonoBehaviour)property.objectReferenceValue).transform.GetHierarchyAsString();
        
        if (EditorGUI.DropdownButton(buttonPosition, new GUIContent(buttonName), FocusType.Keyboard))
        {
            GenericMenu menu = new GenericMenu();
            Debug.Log(fieldInfo.FieldType);

            var targetType = fieldInfo.FieldType;

            Debug.Log(targetType);
            Debug.Log(typeof(IEnumerable).IsAssignableFrom(targetType));
            if (typeof(IEnumerable).IsAssignableFrom(targetType))
            {
                Debug.Log("Es una colleccion");
                targetType = targetType.GenericTypeArguments[0];
            }
            
            var foundObjects = Object.FindObjectsOfType(targetType, true);
            _monoBehaviours = new List<MonoBehaviour>();
            foreach (var foundObject in foundObjects)
            {
                if (foundObject is MonoBehaviour go)
                    _monoBehaviours.Add(go);
            }
            
            _monoBehaviours.Insert(0,null);
            for (var index = 0; index < _monoBehaviours.Count; index++)
            {
                int localIndex = index;
                MonoBehaviour behaviour = _monoBehaviours[localIndex];

                string path;

                if (behaviour != null)
                {
                    path = behaviour.transform.GetHierarchyAsString();
                    if (!splitByHierarchy)
                        path = path.Replace("/", ">");
                }
                else
                    path = "Null";
             
                menu.AddItem(
                    new GUIContent(path),
                    behaviour == property.objectReferenceValue, 
                    ()=> 
                    {
                        property.objectReferenceValue = _monoBehaviours[localIndex];
                        _index = localIndex;
                        if(_monoBehaviours[localIndex])
                        {
                            SceneView.lastActiveSceneView.Frame(
                                new Bounds(_monoBehaviours[localIndex].transform.position, Vector3.one * 10));
                        }
                    });
                menu.ShowAsContext();
            } 
        }

        if (_monoBehaviours != null && _monoBehaviours.Count > 0)
            property.objectReferenceValue = _monoBehaviours[_index];
        
    }

    
    public override bool CanCacheInspectorGUI(SerializedProperty property) => true;
}