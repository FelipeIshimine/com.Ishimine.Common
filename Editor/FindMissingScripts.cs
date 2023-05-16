using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;
using UnityRandom = UnityEngine.Random;

public class FindMissingScripts
{

    [MenuItem("Tools/Find missing scripts in Project")]
    public static void FindMissingScriptsInProjectMenuItem()
    {
        var stringPaths = AssetDatabase.GetAllAssetPaths().Where(path => path.EndsWith(".prefab", StringComparison.Ordinal)).ToArray();

        UnityEditor.EditorUtility.DisplayCancelableProgressBar("Searching missing components in project", "Searching...", 0);
        
        for (var i = 0; i < stringPaths.Length; i++)
        {
            if (UnityEditor.EditorUtility.DisplayCancelableProgressBar("Searching missing components in project",
                    $"[{i}/{stringPaths.Length}] Searching...", (float)i/stringPaths.Length))
                break;
            
            var path = stringPaths[i];
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            if (prefab == null)
                continue;
            foreach (var component in prefab.GetComponentsInChildren<Component>())
            {
                if (component == null)
                    Debug.LogWarning("Prefab found with missing script", prefab);
            }
        }
        UnityEditor.EditorUtility.ClearProgressBar();
    }
    
    [MenuItem("Tools/Find missing scripts in Scene")]
    public static void FindMissingScriptsInSceneMenuItem()
    {
        foreach (var gameObject in Object.FindObjectsOfType<GameObject>(true))
        {
            foreach (var component in gameObject.GetComponentsInChildren<Component>())
            {
                if (component == null)
                    Debug.Log("GameObject found with missing script", gameObject);
            }
        }
    }
}
