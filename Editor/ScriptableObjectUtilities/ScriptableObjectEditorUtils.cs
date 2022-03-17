using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

public static class ScriptableObjectEditorUtils
{
    private static IEnumerable<Type> GetFilter(Type targetType)
    {
        return targetType.Assembly.GetTypes()
            .Where(x => !x.IsAbstract)                                          // Excludes BaseClass
            .Where(x => !x.IsGenericTypeDefinition)                             // Excludes C1<>
            .Where(targetType.IsAssignableFrom);                 
    }

    [Button]
    public static void CreateNew<T>(string nName, Action<T> callback) where T : ScriptableObject
    {
#if UNITY_EDITOR
        var filteredTypes = new List<Type>(GetFilter(typeof(T)));
        
        if(filteredTypes.Count > 1)
        {
            Sirenix.OdinInspector.Editor.TypeSelector selector =
                new Sirenix.OdinInspector.Editor.TypeSelector(filteredTypes, false);
            selector.SelectionConfirmed += selection => callback?.Invoke(CreateNew<T>(nName, selection.ToArray()[0]));
            selector.ShowInPopup();
        }
        else
            callback?.Invoke(CreateNew<T>(nName, filteredTypes[0]));
#endif
    }

    private static T CreateNew<T>(string nName, Type nType) where T : ScriptableObject
    {
        var nScriptableObject = ScriptableObject.CreateInstance(nType);
        nScriptableObject.name = nName;
        return nScriptableObject as T;
    }
   
    [Button]
    public static void Destroy(ScriptableObject scriptableObject)
    {
        UnityEngine.Object.DestroyImmediate(scriptableObject,true);
#if UNITY_EDITOR
        AssetDatabase.SaveAssets();
#endif
    }
}