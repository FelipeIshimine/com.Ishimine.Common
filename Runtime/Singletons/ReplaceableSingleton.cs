using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReplaceableSingleton : MonoBehaviour
{
    public Component target;
    private static readonly Dictionary<string, GameObject> Singletons = new Dictionary<string, GameObject>();

    private string Id() => target.GetType().ToString();

    private void Awake()
    {
        string id = Id();
        if (Singletons.ContainsKey(id)) Replace(id);
        else Singletons.Add(id, gameObject);
    }

    private void Replace(string id)
    {
        GameObject go = Singletons[id];
        Destroy(go);
        Singletons[id] = gameObject;
    }

    private void OnDestroy()
    {
        string id = Id();
        if (Singletons.ContainsKey(id) && Singletons[id] == gameObject)
            Singletons.Remove(id);
    }
}
