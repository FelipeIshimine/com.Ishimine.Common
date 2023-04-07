using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(JoystickAttribute))]
public class JoystickDrawer : PropertyDrawer
{
    private const float LABEL_WIDTH = 200f;
    
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        JoystickAttribute joystick = attribute as JoystickAttribute;
        return EditorGUIUtility.singleLineHeight*2.5f + joystick.size;
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        JoystickAttribute joystick = attribute as JoystickAttribute;
        float size = joystick.size;

        // Create a label for the vector property
        position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

        // Calculate the position of the joystick
        Rect joystickRect = new Rect(position.x + size/2, position.y + EditorGUIUtility.singleLineHeight, size, size);

        // Calculate the position of the vertical slider
        Rect sliderRectY = new Rect(position.x, position.y + EditorGUIUtility.singleLineHeight, EditorGUIUtility.singleLineHeight, size);

        // Calculate the position of the horizontal slider
        Rect sliderRectX = new Rect(position.x + joystickRect.width/2, position.y + size + EditorGUIUtility.singleLineHeight * 1.5f, size, EditorGUIUtility.singleLineHeight);

        Rect labelRect = new Rect(position.x + joystickRect.width + 60, position.y+EditorGUIUtility.singleLineHeight, LABEL_WIDTH * 2, position.height);

        // Get the current value of the vector property
        Vector2 value = property.vector2Value;

        // Draw the labels
        DrawLabels(labelRect, ref value);

        // Draw the vertical slider
        value.y = GUI.VerticalSlider(sliderRectY, value.y, 1f, -1f);

        value.x = GUI.HorizontalSlider(sliderRectX, value.x, -1f, 1f);

        // Draw the joystick
        value = DrawJoystick(joystickRect, value, size);

        // Set the vector property to the new value
        property.vector2Value = value;
    }

    private Vector2 DrawJoystick(Rect position, Vector2 value, float size)
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
            value = direction * (clampedDistance / maxDistance);
        }

        value.y *= -1;
        return value;
    }
    
    private void DrawLabels(Rect position, ref Vector2 value)
    {
        Rect yLabelRect = new Rect(position.x, position.y, 100, EditorGUIUtility.singleLineHeight);
        EditorGUI.LabelField(yLabelRect, "Y:");

        Rect yFieldRect = new Rect(position.x + 20, position.y, 60, EditorGUIUtility.singleLineHeight);
        value.y = EditorGUI.FloatField(yFieldRect, value.y);

        position.y += EditorGUIUtility.singleLineHeight;

        Rect xLabelRect = new Rect(position.x, position.y, 100, EditorGUIUtility.singleLineHeight);
        EditorGUI.LabelField(xLabelRect, "X:");

        Rect xFieldRect = new Rect(position.x + 20, position.y, 60, EditorGUIUtility.singleLineHeight);
        value.x = EditorGUI.FloatField(xFieldRect, value.x);
    }
    
}