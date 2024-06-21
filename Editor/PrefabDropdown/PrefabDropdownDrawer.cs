using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace PrefabDropdown
{
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

        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            if (prefabList == null)
                LoadPrefabs();

            var container = new VisualElement()
            {
	            style = { flexDirection = FlexDirection.Row}
            };

            if (prefabList.Count == 0)
            {
                container.Add(new PropertyField(property));
                return container;
            }

            
            var label = new Label(property.displayName);
            container.Add(label);

            container.Add(new VisualElement()
            {
	            style = { flexGrow = 1}
            });
            container.Add(new PropertyField(property, string.Empty));
            
            var dropdown = new PopupField<string>();
            bool useFullPath = ((PrefabDropdownAttribute)attribute).showFullPath;
            dropdown.choices = prefabList.Select(x => useFullPath ? RemoveCommonPath(x.Path) : x.Prefab.name).ToList();
            dropdown.index = prefabList.FindIndex(x => x.Path == AssetDatabase.GetAssetPath(property.objectReferenceValue));

            dropdown.RegisterValueChangedCallback(evt =>
            {
                selectedPrefabIndex = dropdown.index;
                property.objectReferenceValue = selectedPrefabIndex >= 0 ? prefabList[selectedPrefabIndex].Prefab : null;
                property.serializedObject.ApplyModifiedProperties();
            });

            container.Add(dropdown);

            var clearButton = new Button(() =>
            {
                property.objectReferenceValue = null;
                property.serializedObject.ApplyModifiedProperties();
            })
            {
                text = "X"
            };

            container.Add(clearButton);

            return container;
        }

        private void LoadPrefabs()
        {
            prefabList = new List<Result>();
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
}
