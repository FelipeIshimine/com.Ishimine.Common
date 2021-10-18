using System;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class ReplaceWithNewPrefabClone
{
    [MenuItem("GameObject/Replace With/New Prefab Clone", false, 10)]
    public static void Execute()
    {
        Debug.Log($"{Selection.activeObject.name} is PrefabVariant");

        var selected = Selection.activeObject as GameObject;
        
        if(selected == null)
            return;
        
        GameObject originalPrefab = PrefabUtility.GetCorrespondingObjectFromSource(selected);

        Debug.Log(originalPrefab);
        
        string assetPath = AssetDatabase.GetAssetPath(originalPrefab);

        string nPath = assetPath.Replace(".prefab", " Clone.prefab");
        
        
        AssetDatabase.CopyAsset(assetPath, nPath);
        
        ReplaceWithPrefabAt(selected, nPath);

    }

    public static void Execute(GameObject sceneObject)
    {
        GameObject originalPrefab = PrefabUtility.GetCorrespondingObjectFromSource(sceneObject);
        string assetPath = AssetDatabase.GetAssetPath(originalPrefab);

        string extension = Path.GetExtension(assetPath);
        string fileName = Path.GetFileName(assetPath);
        string cleanFile = Path.GetFileNameWithoutExtension(assetPath);
        
        string newPath = assetPath.Replace(fileName, $"{cleanFile} Clone{extension}");
        AssetDatabase.CopyAsset(assetPath, newPath);

        ReplaceWithPrefabAt(sceneObject, newPath);
    }

    private static void ReplaceWithPrefabAt(GameObject sceneObject, string newPath)
    {
        var newObject = (GameObject)PrefabUtility.InstantiatePrefab(AssetDatabase.LoadAssetAtPath<GameObject>(newPath),
            SceneManager.GetActiveScene());

        if (!newObject)
        {
            throw new Exception($"Invalid path:{newPath}");
        }
        
        Undo.RegisterCreatedObjectUndo(newObject, "Replace With Prefabs");

        newObject.transform.parent = sceneObject.transform.parent;
        newObject.transform.localPosition = sceneObject.transform.localPosition;
        newObject.transform.localRotation = sceneObject.transform.localRotation;
        newObject.transform.localScale = sceneObject.transform.localScale;
        newObject.transform.SetSiblingIndex(sceneObject.transform.GetSiblingIndex());

        Undo.DestroyObjectImmediate(sceneObject);

        Selection.activeObject = newObject;
        
        Debug.Log("<color=green> Clone and Replaced: SUCCESS </color>");
    }

    public static void Execute(GameObject sceneObject, string newPath)
    {
        GameObject originalPrefab = PrefabUtility.GetCorrespondingObjectFromSource(sceneObject);

        string assetPath = AssetDatabase.GetAssetPath(originalPrefab);
        string fileName = Path.GetFileName(assetPath);
        newPath += fileName;
        AssetDatabase.CopyAsset(assetPath, newPath);

        ReplaceWithPrefabAt(sceneObject, newPath);
    }
    
    [MenuItem("GameObject/Replace With/New Prefab Clone", true)]
    static bool Validate()
    {
        var  value = PrefabUtility.GetOutermostPrefabInstanceRoot(Selection.activeObject);
        return value != null && PrefabUtility.IsPartOfAnyPrefab(value);
        
        //return PrefabUtility.IsPartOfVariantPrefab(Selection.activeObject);
    }
    
}
