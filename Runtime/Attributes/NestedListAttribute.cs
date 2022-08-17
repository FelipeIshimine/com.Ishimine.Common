using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

#if UNITY_EDITOR
using Sirenix.OdinInspector.Editor;
using UnityEditor;

[ListDrawerSettings(CustomRemoveElementFunction = "@$property.GetAttribute<NestedListAttribute>().RemoveObject($removeElement, $property)", Expanded = true)]
[ValueDropdown("@$property.GetAttribute<NestedListAttribute>().GetAllObjectsOfType()", FlattenTreeView = true)]
[OnCollectionChanged("@$property.GetAttribute<NestedListAttribute>().OnCollectionChange($info)")]
#endif
[IncludeMyAttributes]
public class NestedListAttribute : Attribute
{
#if UNITY_EDITOR
    public List<UnityEngine.Object> objectsToRemove = new List<UnityEngine.Object>();
    public List<ScriptableObject> objectsToCreate = new List<ScriptableObject>();

    public Type Type;

    protected void RemoveObject(UnityEngine.Object objectToRemove, InspectorProperty property)
    {
        objectsToRemove.Add(objectToRemove);
    }

    protected IEnumerable GetAllObjectsOfType()
    {
        var items = UnityEditor.AssetDatabase.FindAssets("t:Monoscript", new[] { "Assets/Scripts" })
            .Select(x => UnityEditor.AssetDatabase.GUIDToAssetPath(x))
            .Where(x => IsCorrectType(UnityEditor.AssetDatabase.LoadAssetAtPath<MonoScript>(x)))
            .Select(x => new ValueDropdownItem(System.IO.Path.GetFileName(x), ScriptableObject.CreateInstance(UnityEditor.AssetDatabase.LoadAssetAtPath<MonoScript>(x).GetClass())));
        return items;
    }

    protected bool IsCorrectType(MonoScript script)
    {
        if (script != null)
        {
            Type scriptType = script.GetClass();
            if (scriptType != null && (scriptType.Equals(Type) || scriptType.IsSubclassOf(Type)) && !scriptType.IsAbstract)
            {
                return true;
            }
        }
        return false;
    }

    protected void OnCollectionChange(CollectionChangeInfo info)
    {
        if (info.ChangeType == CollectionChangeType.Add)
        {
            objectsToCreate.Add((ScriptableObject)info.Value);
        }
    }
#endif

}