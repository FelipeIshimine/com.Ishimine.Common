// JoystickDrawerUI.cs
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

[CustomPropertyDrawer(typeof(JoystickAttribute))]
public class JoystickDrawerUI : PropertyDrawer
{
    public override VisualElement CreatePropertyGUI(SerializedProperty property)
    {
        var attr    = (JoystickAttribute)attribute;
        bool isVec3 = property.propertyType == SerializedPropertyType.Vector3;

        // Create the appropriate field
        Vector2Field v2f = null;
        Vector3Field v3f = null;
        if (isVec3)
        {
            v3f = new Vector3Field(string.Empty)
            {
                style =
                {
                    width = Length.Percent(100),
                    alignSelf = Align.Center,
                    alignContent = Align.Center
                }
            };
        }
        else
        {
            v2f = new Vector2Field(string.Empty)
            {
                style =
                {
                    width = Length.Percent(100),
                    alignSelf = Align.Center,
                    alignContent = Align.Center

                }
            };
        }

        // JoystickKnob
        var knob = new JoystickKnob(attr.MaxMagnitude, attr.Step)
        {
            style = { alignSelf = Align.Center}
        };


        knob.schedule.Execute(() => knob.SetValue(isVec3 ? property.vector3Value : property.vector2Value));
        
        // Knob → Property → Field
        knob.OnValueChanged += val =>
        {
            if (isVec3) property.vector3Value = val;
            else        property.vector2Value = (Vector2)val;
            property.serializedObject.ApplyModifiedProperties();

            if (isVec3) v3f.SetValueWithoutNotify(property.vector3Value);
            else        v2f.SetValueWithoutNotify(property.vector2Value);
        };

        // Field → Knob → Property
        if (isVec3)
        {
            v3f.RegisterValueChangedCallback(evt =>
            {
                var v = evt.newValue;
                knob.SetValue(v);
                property.vector3Value = v;
                property.serializedObject.ApplyModifiedProperties();
            });
            v3f.SetValueWithoutNotify(property.vector3Value);
        }
        else
        {
            v2f.RegisterValueChangedCallback(evt =>
            {
                var v = evt.newValue;
                knob.SetValue((Vector3)v);
                property.vector2Value = v;
                property.serializedObject.ApplyModifiedProperties();
            });
            v2f.SetValueWithoutNotify(property.vector2Value);
        }

        // Build container
        var container = new VisualElement();
container.Add(new Label(property.displayName)
{
    style = { alignSelf = Align.Center}
});
        container.style.borderBottomLeftRadius =
            container.style.borderBottomRightRadius =
                container.style.borderTopLeftRadius =
                    container.style.borderTopRightRadius = 12;
        
        container.style.borderBottomWidth =
            container.style.borderLeftWidth =
                container.style.borderRightWidth =
                    container.style.borderTopWidth = 2;
        
        container.style.borderBottomColor =
            container.style.borderLeftColor =
                container.style.borderRightColor =
                    container.style.borderTopColor = new Color(0,0,0,.5f);
        
        container.Add(knob);
        if (isVec3) container.Add(v3f);
        else        container.Add(v2f);
        return container;
    }
}
