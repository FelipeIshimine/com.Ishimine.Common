using System;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace ComponentSelector
{
	[CustomPropertyDrawer(typeof(ComponentSelectorAttribute))]
	public class ComponentSelectorDrawer : PropertyDrawer
	{
		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			if(property.serializedObject.isEditingMultipleObjects)
				return;
	    
			EditorGUI.BeginProperty(position, label, property);

			if (property.propertyType == SerializedPropertyType.ObjectReference && fieldInfo.FieldType.IsSubclassOf(typeof(Component)))
			{
				var compatibleTypes = TypeCache.GetTypesDerivedFrom(fieldInfo.FieldType);

				var types = compatibleTypes.ToArray();

				Type activeType = null;
				if (property.objectReferenceValue)
					activeType = property.objectReferenceValue.GetType();

				// Calculate the position for the dropdown
				var dropdownRect = new Rect(position.x, position.y, position.width - 70f, position.height);

				var oldIndex = compatibleTypes.IndexOf(activeType);
				int selectedIndex = EditorGUI.Popup(dropdownRect, label.text, oldIndex, types.Select(x => x.Name).ToArray());

				Component currentComponent = property.objectReferenceValue as Component;

				if (oldIndex != selectedIndex)
				{
					if (property.objectReferenceValue != null)
					{
						Undo.DestroyObjectImmediate(currentComponent);
					}

					if (selectedIndex >= 0)
					{
						Type selectedType = types[selectedIndex];
						var component = (Component)property.serializedObject.targetObject;
						property.objectReferenceValue = Undo.AddComponent(component.gameObject, selectedType);
					}
				}

				// Calculate the position for the "Clear" button
				var buttonWidth = 60f;
				var buttonRect = new Rect(position.x + position.width - buttonWidth, position.y, buttonWidth, position.height);
				if (GUI.Button(buttonRect, "Clear"))
				{
					if (property.objectReferenceValue != null)
					{
						Undo.DestroyObjectImmediate(currentComponent);
						property.objectReferenceValue = null; // Set the field to null
					}
				}
			}
			else
			{
				EditorGUI.LabelField(position, label, new GUIContent("Property is not a Component."));
			}

			EditorGUI.EndProperty();
		}

		public override float GetPropertyHeight(SerializedProperty property, GUIContent label) =>
			EditorGUIUtility.singleLineHeight;
	}
}