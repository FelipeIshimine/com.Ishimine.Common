using System.Linq;
using UnityEditor;
using UnityEngine;


// Define a custom PropertyDrawer for the TagStringAttribute
namespace PropertyDrawers
{
	[CustomPropertyDrawer(typeof(TagStringAttribute))]
	public class TagStringPropertyDrawer : PropertyDrawer
	{
		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			// Get the current value of the string from the property
			string currentValue = property.stringValue;

			// Get a list of all available tags from the InternalEditorUtility
			string[] availableTags = UnityEditorInternal.InternalEditorUtility.tags;

			// Display a dropdown with the available tags as options
			int selectedIndex = EditorGUI.Popup(position, label.text, availableTags.ToList().IndexOf(currentValue), availableTags);

			// Set the selected value as the new value of the string property
			if(selectedIndex != -1) 
				property.stringValue = availableTags[selectedIndex];
		}
	}
}
