using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using Label = UnityEngine.UIElements.Label;

namespace AutoGetComponent
{
	[CustomPropertyDrawer(typeof(AutoGetComponentAttribute))]
	public class AutoGetComponentDrawer : PropertyDrawer
	{
    
		public override VisualElement CreatePropertyGUI(SerializedProperty property)
		{
			var attribute = this.attribute as AutoGetComponentAttribute;
        
			// Create a container for the property
			var container = new VisualElement();

			// Get the component type
			var componentType = attribute.componentType;

			// Get the component from the GameObject
			var component = GetComponent(property, componentType);
			if (component == null)
			{
				// Create a warning label if the component is not found
				var label = new Label($"{componentType} component not found on {property.serializedObject.targetObject.name}.");
				label.AddToClassList("warning");
				container.Add(label);
			}
			else
			{
				// Set the value of the field to the component
				property.objectReferenceValue = component;

				// Create the property field
				var field = new ObjectField(property.displayName);
				field.BindProperty(property);
				container.Add(field);
			}

			return container;
		}
    
		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			// Get the attribute
			var attribute = this.attribute as AutoGetComponentAttribute;

			if (property.objectReferenceValue != null && property.objectReferenceValue.GetType() == attribute.componentType)
				EditorGUI.PropertyField(position, property, label, true);
        
			// Get the component from the GameObject
			var component = GetComponent(property, attribute.componentType);
			if (component == null)
			{
				// Display a warning if the component is not found
				EditorGUI.HelpBox(position, $"{attribute.componentType.Name} component not found on {property.serializedObject.targetObject.name}.", MessageType.Warning);
			}
			else
			{
				property.objectReferenceValue = component;

				// Display the component field as normal
				EditorGUI.PropertyField(position, property, label, true);
			}
		}

		private Component GetComponent(SerializedProperty property, System.Type componentType)
		{
			var component = property.serializedObject.targetObject as Component;

			if (!component) return null;
        
			// Get the GameObject
			var gameObject = component.gameObject  as GameObject;

			// Return the component from the GameObject
			return gameObject.GetComponent(componentType);
		}
	}
}