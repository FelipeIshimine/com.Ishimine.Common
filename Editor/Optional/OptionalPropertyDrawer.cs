using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace Optional
{
	[CustomPropertyDrawer(typeof(Optional<>),true)]
	public class OptionalPropertyDrawer : PropertyDrawer
	{
		public override VisualElement CreatePropertyGUI(SerializedProperty property)
		{
			var valueProperty = property.FindPropertyRelative("value");
			var enabledProperty = property.FindPropertyRelative("enabled");

			VisualElement container = new VisualElement()
			{
				style = { flexDirection = FlexDirection.Row }
			};

			var valueProp = new PropertyField(valueProperty, property.displayName){style = { flexGrow = 1}};
			valueProp.SetEnabled(enabledProperty.boolValue);
			container.Add(valueProp);
	    
			var toggleField = new Toggle();
			container.Add(toggleField);
			toggleField.BindProperty(enabledProperty);

			void OnToggle(ChangeEvent<bool> evt) => valueProp.SetEnabled(evt.newValue);
			toggleField.RegisterValueChangedCallback(OnToggle);
			return container;
		}
	}
	
	[CustomPropertyDrawer(typeof(OptionalAsset<>),true)]
	public class OptionalAssetPropertyDrawer : PropertyDrawer
	{
		public override VisualElement CreatePropertyGUI(SerializedProperty property)
		{
			var valueProperty = property.FindPropertyRelative("value");
			var enabledProperty = property.FindPropertyRelative("enabled");

			VisualElement container = new VisualElement()
			{
				style = { flexDirection = FlexDirection.Row }
			};

			var valueProp = new PropertyField(valueProperty, property.displayName){style = { flexGrow = 1}};
			valueProp.SetEnabled(enabledProperty.boolValue);
			container.Add(valueProp);
	    
			var toggleField = new Toggle();
			container.Add(toggleField);
			toggleField.BindProperty(enabledProperty);

			void OnToggle(ChangeEvent<bool> evt) => valueProp.SetEnabled(evt.newValue);
			toggleField.RegisterValueChangedCallback(OnToggle);
			return container;
		}
	}
}
