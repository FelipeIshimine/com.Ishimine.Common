using UnityEditor;
using UnityEngine;

namespace PropertyDrawers
{
	[CustomPropertyDrawer(typeof(JoystickAttribute))]
	public class JoystickDrawer : PropertyDrawer
	{
		private const float LABEL_WIDTH = 200f;
		private const float HEIGHT = 50f;

		public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
		{
			JoystickAttribute joystick = attribute as JoystickAttribute;
			return EditorGUIUtility.singleLineHeight * 3.5f + HEIGHT/2 ;
		}

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			JoystickAttribute joystick = attribute as JoystickAttribute;

			// Create a label for the vector property
			position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

			int dimensions = property.propertyType == SerializedPropertyType.Vector2 ? 2 : 3;

			//Debug.Log(dimensions);
			// Calculate the position of the joystick
			Rect joystickRect = new Rect(position.x + HEIGHT / 2, position.y + EditorGUIUtility.singleLineHeight, HEIGHT, HEIGHT);

			// Calculate the position of the vertical slider
			Rect sliderRectY = new Rect(position.x, position.y + EditorGUIUtility.singleLineHeight, EditorGUIUtility.singleLineHeight, HEIGHT);

			// Calculate the position of the horizontal slider
			Rect sliderRectX = new Rect(position.x + joystickRect.width / 2, position.y + HEIGHT + EditorGUIUtility.singleLineHeight * 1.5f, HEIGHT, EditorGUIUtility.singleLineHeight);

			Rect labelRect = new Rect(position.x + joystickRect.width + 60, position.y + EditorGUIUtility.singleLineHeight, LABEL_WIDTH * 2, position.height);

			// Get the current value of the vector property
			Vector3 value = property.propertyType == SerializedPropertyType.Vector3 ? property.vector3Value / joystick.MaxMagnitude : property.vector2Value / joystick.MaxMagnitude;

			// Draw the labels
			if (joystick.UseMultiplier)
			{
				DrawLabelsAndMultiplier(ref labelRect, ref value, ref joystick.MaxMagnitude, dimensions);
			}
			else
			{
				DrawSideLabels(ref labelRect, ref value, dimensions);
			}

			// Draw the vertical slider
			value.y = GUI.VerticalSlider(sliderRectY, value.y, 1f, -1f);

			value.x = GUI.HorizontalSlider(sliderRectX, value.x, -1f, 1f);

			if (dimensions == 3)
			{
				Rect sliderRectZ = new Rect(position.x + joystickRect.width + EditorGUIUtility.singleLineHeight*2, position.y + EditorGUIUtility.singleLineHeight, EditorGUIUtility.singleLineHeight, HEIGHT);
				value.z = GUI.VerticalSlider(sliderRectZ, value.z, 1f, -1f);
			}

			// Draw the joystick
			value = DrawJoystick(joystickRect, value, HEIGHT);

			// Set the vector property to the new value
			if (property.propertyType == SerializedPropertyType.Vector3)
			{
				property.vector3Value = value * joystick.MaxMagnitude;
			}
			else
			{
				property.vector2Value = value * joystick.MaxMagnitude;
			}

			position.x *= .5f;
		}

		private Vector3 DrawJoystick(Rect position, Vector3 value, float size)
		{
			value.y *= -1;
			// Calculate the center position of the joystick
			Vector2 center = new Vector2(position.x + size / 2f, position.y + position.height / 2f);

			// Calculate the maximum distance the joystick can be moved from the center
			float maxDistance = size / 2f;

			// Calculate the position of the joystick based on the value
			Vector2 joystickPosition = center + new Vector2(value.x, value.y) * maxDistance;

			// Draw the background circle of the joystick
			Handles.color = Color.gray;
			Handles.DrawSolidDisc(center, Vector3.back, maxDistance);

			// Draw the joystick handle
			Handles.color = Color.white;
			Handles.DrawSolidDisc(joystickPosition, Vector3.back, maxDistance / 2f);

			// Check if the joystick is being dragged
			int controlId = GUIUtility.GetControlID(FocusType.Passive);
			if (Event.current.type == EventType.MouseDown && position.Contains(Event.current.mousePosition))
			{
				GUIUtility.hotControl = controlId;
			}
			else if (Event.current.type == EventType.MouseUp && GUIUtility.hotControl == controlId)
			{
				GUIUtility.hotControl = 0;
			}
			else if (Event.current.type == EventType.MouseDrag && GUIUtility.hotControl == controlId)
			{
				// Calculate the new value of the joystick based on the mouse position
				Vector2 mousePosition = Event.current.mousePosition;
				Vector2 direction = (mousePosition - center).normalized;
				float distance = Vector2.Distance(mousePosition, center);
				float clampedDistance = Mathf.Clamp(distance, 0f, maxDistance);
				var z = value.z;
				value = new Vector3(direction.x, direction.y) * (clampedDistance / maxDistance);
				value.z = z;
			}

			value.y *= -1;
			return value;
		}

		private void DrawSideLabels(ref Rect position, ref Vector3 value, int dimensions)
		{
			value = value.Clamp(-Vector3.one,Vector3.one);
			for (int i = 0; i < dimensions; i++)
			{
				string label = i == 0 ? "X:" : i == 1 ? "Y:" : "Z:";
				Rect labelRect = new Rect(position.x, position.y, 100, EditorGUIUtility.singleLineHeight);
				EditorGUI.LabelField(labelRect, label);

				Rect fieldRect = new Rect(position.x + 20, position.y, 60, EditorGUIUtility.singleLineHeight);

				switch (i)
				{
					case 0:
						value.x = EditorGUI.FloatField(fieldRect, value.x);
						break;
					case 1:
						value.y = EditorGUI.FloatField(fieldRect, value.y);
						break;
					case 2:
						value.z = EditorGUI.FloatField(fieldRect, value.z);
						break;
				}

				position.y += EditorGUIUtility.singleLineHeight;
			}
		}

		private void DrawLabelsAndMultiplier(ref Rect position, ref Vector3 value, ref float multiplier, int dimensions)
		{
			Rect multLabelRect = new Rect(position.x, position.y, 100, EditorGUIUtility.singleLineHeight);
			EditorGUI.LabelField(multLabelRect, "M:");

			Rect multFieldRect = new Rect(position.x + 20, position.y, 60, EditorGUIUtility.singleLineHeight);

			if (multiplier == 0) multiplier = .01f;

			multiplier = EditorGUI.FloatField(multFieldRect, multiplier);

			position.y += EditorGUIUtility.singleLineHeight;

			DrawSideLabels(ref position, ref value, dimensions);
		}
	}
}
