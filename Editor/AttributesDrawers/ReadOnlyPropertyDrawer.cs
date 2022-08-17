using UnityEditor;
using UnityEngine;

namespace GE.Editor
{
    [CustomPropertyDrawer(typeof(ReadOnlyAttribute))]
    public class ReadOnlyPropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            GUI.enabled = false;
            base.OnGUI(position, property, label);
            GUI.enabled = true;
        }
    }
}