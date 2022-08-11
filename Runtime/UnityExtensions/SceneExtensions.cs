using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

public static class SceneExtensions
{
    public static T FindObjectInScene<T>(this Scene scene, bool includeInactive = false) where T : Component
    {
        if (!scene.isLoaded || !scene.IsValid())
            throw new Exception("Scene isn't loaded or is Invalid");

        var allFound = Object.FindObjectsOfType<T>(includeInactive);
        foreach (var found in allFound)
        {
            if(found.gameObject.scene == scene)
                return found;
        }
        return null;
    }
    
    public static T[] FindObjectsInScene<T>(this Scene scene, bool includeInactive = false) where T : Component
    {
        if (!scene.isLoaded || !scene.IsValid())
            throw new Exception("Scene isn't loaded or is Invalid");

        List<T> results = new List<T>();
        var allFound = Object.FindObjectsOfType<T>(includeInactive);
        foreach (var found in allFound)
        {
            if (found.gameObject.scene == scene)
                results.Add(found);
        }
        
        return results.ToArray();
    }
}