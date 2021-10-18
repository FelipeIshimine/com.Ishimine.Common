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

        var newObject = (GameObject)PrefabUtility.InstantiatePrefab(AssetDatabase.LoadAssetAtPath<GameObject>(nPath), SceneManager.GetActiveScene());
        
        Undo.RegisterCreatedObjectUndo(newObject, "Replace With Prefabs");
        
        newObject.transform.parent = selected.transform.parent;
        newObject.transform.localPosition = selected.transform.localPosition;
        newObject.transform.localRotation = selected.transform.localRotation;
        newObject.transform.localScale = selected.transform.localScale;
        newObject.transform.SetSiblingIndex(selected.transform.GetSiblingIndex());

        Undo.DestroyObjectImmediate(selected);
    }
    
    [MenuItem("GameObject/Replace With/New Prefab Clone", true)]
    static bool Validate()
    {
        var  value = PrefabUtility.GetOutermostPrefabInstanceRoot(Selection.activeObject);
        return value != null && PrefabUtility.IsPartOfAnyPrefab(value);
        
        //return PrefabUtility.IsPartOfVariantPrefab(Selection.activeObject);
    }
    
}
