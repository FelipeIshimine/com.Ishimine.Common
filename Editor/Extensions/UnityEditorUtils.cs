using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

public static class UnityEditorUtils
{
    public static List<Object> FindAssetsByType(Type type)
    {
        List<Object> assets = new List<Object>();
        string[] guids = AssetDatabase.FindAssets($"t:{type}");
        for (int i = 0; i < guids.Length; i++)
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(guids[i]);
            Object[] found = AssetDatabase.LoadAllAssetsAtPath(assetPath);

            for (int index = 0; index < found.Length; index++)
                if (found[index] is { } item && !assets.Contains(item))
                    assets.Add(item);
        }
        return assets;
    }
    
    public  static List<T> FindAssetsByType<T>()
    {
        List<T> assets = new List<T>();
        string[] guids = AssetDatabase.FindAssets($"t:{typeof(T)}");
        for (int i = 0; i < guids.Length; i++)
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(guids[i]);
            Object[] found = AssetDatabase.LoadAllAssetsAtPath(assetPath);

            for (int index = 0; index < found.Length; index++)
                if (found[index] is T item && !assets.Contains(item))
                    assets.Add(item);
        }
        return assets;
    }
    
    public static Scene[] GetAllLoadedScenes()
    {
        List<Scene> scenes = new List<Scene>();
        for (int i = 0; i < SceneManager.sceneCount; i++)
            scenes.Add(SceneManager.GetSceneAt(i));
        return scenes.ToArray();
    }
}