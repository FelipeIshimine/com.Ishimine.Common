using System;
using UnityEngine;
using UnityEditor;

public class AdvancedReplaceWindow : EditorWindow
{
    [SerializeField] private GameObject prefab;

    private static ReplaceWithPrefabSO _so;

    private static readonly string ScriptableObjectPath = $"Assets/ScriptableObjects/{nameof(AdvancedReplaceWindow)}.asset";

    private static ReplaceWithPrefabSO Data
    {
        get
        {
            if (_so == null)
            {
                _so = AssetDatabase.LoadAssetAtPath<ReplaceWithPrefabSO>(ScriptableObjectPath);
                if (_so != null) return _so;
                _so = ScriptableObject.CreateInstance<ReplaceWithPrefabSO>();
                AssetDatabase.CreateAsset(_so, ScriptableObjectPath);
            }
            return _so;
        }
    }
    [System.Serializable]
    public enum  Mode 
    {
        OtherPrefab,
        CloneAndReplace
    }
    
    private static void OpenWindow()
    {
        var window = EditorWindow.GetWindow<AdvancedReplaceWindow>();
        window.name = "Advanced Replacement";
    }

    [MenuItem("Tools/Advanced Replacement")]
    static void CreateReplaceWithPrefab()=> OpenWindow();
 
    [MenuItem("GameObject/Advanced Replacement",false, 10)]
    static void CreateReplaceWithPrefabRickClick() => OpenWindow();

    private void OnGUI()
    {
        Data.mode = (Mode)EditorGUILayout.EnumPopup("Mode", Data.mode);
        switch (Data.mode)
        {
            case Mode.OtherPrefab:
                RenderReplaceWithPrefab();
                break;
            case Mode.CloneAndReplace:
                RenderCloneAndReplace();
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private void RenderCloneAndReplace()
    {

        Data.cloneOnNewPath = EditorGUILayout.Toggle("Clone on new path", Data.cloneOnNewPath);

        if (Data.cloneOnNewPath)
            Data.clonePath = EditorGUILayout.TextField("Assets/", Data.clonePath);

        if (GUILayout.Button("Clone and Replace"))
        {
            var selection = Selection.gameObjects;

            for (var i = selection.Length - 1; i >= 0; --i)
            {
                if (!PrefabUtility.IsAnyPrefabInstanceRoot(selection[i]))
                    return;

                if (Data.cloneOnNewPath)
                    ReplaceWithNewPrefabClone.Execute(selection[i], $"Assets/{Data.clonePath}/");
                else
                    ReplaceWithNewPrefabClone.Execute(selection[i]);
            }
        }

        GUI.enabled = false;
        EditorGUILayout.LabelField("Selection count: " + Selection.objects.Length);
    }

    private void RenderReplaceWithPrefab()
    {
        prefab = (GameObject)EditorGUILayout.ObjectField("Prefab", prefab, typeof(GameObject), false);

        if (GUILayout.Button("Replace"))
        {
            var selection = Selection.gameObjects;

            for (var i = selection.Length - 1; i >= 0; --i)
            {
                var selected = selection[i];
                var prefabType = PrefabUtility.GetPrefabInstanceStatus(prefab);
                GameObject newObject;

                if (prefabType == PrefabInstanceStatus.NotAPrefab)
                {
                    newObject = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
                }
                else
                {
                    newObject = Instantiate(prefab);
                    newObject.name = prefab.name;
                }

                if (newObject == null)
                {
                    Debug.LogError("Error instantiating prefab");
                    break;
                }

                Undo.RegisterCreatedObjectUndo(newObject, "Replace With Prefabs");
                newObject.transform.parent = selected.transform.parent;
                newObject.transform.localPosition = selected.transform.localPosition;
                newObject.transform.localRotation = selected.transform.localRotation;
                newObject.transform.localScale = selected.transform.localScale;
                newObject.transform.SetSiblingIndex(selected.transform.GetSiblingIndex());

                Undo.DestroyObjectImmediate(selected);
            }
        }
    }
}