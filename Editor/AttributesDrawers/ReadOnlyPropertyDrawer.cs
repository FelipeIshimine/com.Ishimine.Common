using GE;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace AttributesDrawers
{
    [CustomPropertyDrawer(typeof(ReadOnlyAttribute))]
    public class ReadOnlyPropertyDrawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            var propertyField = new PropertyField(property);
            propertyField.SetEnabled(false);
            return propertyField;
        }
    }
}
