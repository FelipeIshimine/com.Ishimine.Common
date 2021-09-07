using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

public class QuickComponentSingleton : MonoBehaviour
{
    private static Dictionary<Type, Component> singletons = new Dictionary<Type, Component>();
    public Component targetMonoBehaviour;

    private void Awake()
    {
        if(singletons.ContainsKey(targetMonoBehaviour.GetType()))
            Destroy(gameObject);
        else
            singletons.Add(targetMonoBehaviour.GetType(), targetMonoBehaviour);
    }

    private void OnDestroy()
    {
        if(singletons[targetMonoBehaviour.GetType()] == targetMonoBehaviour)
            singletons.Remove(targetMonoBehaviour.GetType());
    }

    public static T Get<T>() where T : Component => Get<T>(typeof(T));

    private static T Get<T>(Type type) where T : Component
    {
        if (singletons.TryGetValue(type, out Component value))
            return value as T;
        return null;
    }
}
