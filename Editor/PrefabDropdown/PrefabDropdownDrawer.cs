using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;
using System.IO; // Added for path manipulation

[CustomPropertyDrawer(typeof(PrefabDropdownAttribute))]
public class PrefabDropdownDrawer : PropertyDrawer
{
    private struct Result
    {
        public GameObject Prefab;
        public string Path;
    }

    private List<Result> prefabList;
    private int selectedPrefabIndex = -1;

  public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
{
    if (prefabList == null)
        LoadPrefabs();

    if (prefabList.Count == 0)
    {
        EditorGUI.PropertyField(position, property, label);
        return;
    }

    EditorGUI.BeginProperty(position, label, property);

    // Calculate the label and dropdown widths
    float labelWidth = EditorGUIUtility.labelWidth;
    float dropdownWidth = 20; // Subtract a button's width

    // Calculate the width of the property field
    float propertyFieldWidth = position.width - labelWidth - dropdownWidth - EditorGUIUtility.singleLineHeight;

    // Split the position into label, dropdown, property field, and button portions
    Rect labelRect = new Rect(position.x, position.y, labelWidth, position.height);
    Rect dropdownRect = new Rect(position.x + labelWidth, position.y, dropdownWidth, position.height);
    Rect propertyFieldRect = new Rect(position.x + labelWidth + dropdownWidth, position.y, propertyFieldWidth, position.height);
    Rect buttonRect = new Rect(position.x + labelWidth + dropdownWidth + propertyFieldWidth, position.y, EditorGUIUtility.singleLineHeight, position.height);

    // Draw the label using EditorGUI.PrefixLabel
    EditorGUI.PrefixLabel(labelRect, label);

    // Get the index of the currently selected prefab
    string currentPrefabPath = property.objectReferenceValue != null
        ? AssetDatabase.GetAssetPath(property.objectReferenceValue)
        : null;

    selectedPrefabIndex = prefabList.FindIndex(x => x.Path == currentPrefabPath);

    // Show the dropdown for selecting a prefab
    EditorGUI.BeginChangeCheck();
    bool useFullPath = ((PrefabDropdownAttribute)attribute).showFullPath;
    selectedPrefabIndex = EditorGUI.Popup(
        dropdownRect,
        selectedPrefabIndex,
        prefabList.Select(x => useFullPath ? RemoveCommonPath(x.Path) : x.Prefab.name).ToArray()
    );

    // Draw the property field
    EditorGUI.PropertyField(propertyFieldRect, property, GUIContent.none);

    if (EditorGUI.EndChangeCheck())
    {
        property.objectReferenceValue = selectedPrefabIndex >= 0 ? prefabList[selectedPrefabIndex].Prefab : null;
    }

    // Add a button to clear the field
    if (GUI.Button(buttonRect, "X", EditorStyles.miniButton))
    {
        property.objectReferenceValue = null;
    }
    EditorGUI.EndProperty();
}


    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return EditorGUIUtility.singleLineHeight;
    }

    private void LoadPrefabs()
    {
        prefabList = new List<Result>();
        // Find all prefabs in the project that have any of the specified component types
        string[] allPrefabPaths = AssetDatabase.GetAllAssetPaths().Where(path => path.EndsWith(".prefab")).ToArray();

        foreach (string prefabPath in allPrefabPaths)
        {
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
            if (prefab != null && prefab.TryGetComponent(fieldInfo.FieldType, out var value))
            {
                prefabList.Add(new Result()
                {
                    Prefab = prefab,
                    Path = prefabPath
                });
            }
        }
    }

    private string RemoveCommonPath(string fullPath)
    {
        string commonPrefix = prefabList.Select(x => x.Path).GetCommonPrefix();
        if (fullPath.StartsWith(commonPrefix))
        {
            return fullPath.Substring(commonPrefix.Length);
        }
        return fullPath;
    }
}

public static class EnumerableExtensions
{
    public static string GetCommonPrefix(this IEnumerable<string> strings)
    {
        if (strings == null || !strings.Any())
        {
            return string.Empty;
        }

        string shortest = strings.OrderBy(s => s.Length).First();
        int length = shortest.Length;

        for (int i = 0; i < length; i++)
        {
            char c = shortest[i];
            if (!strings.All(s => s[i] == c))
            {
                return shortest.Substring(0, i);
            }
        }

        return shortest;
    }
}
