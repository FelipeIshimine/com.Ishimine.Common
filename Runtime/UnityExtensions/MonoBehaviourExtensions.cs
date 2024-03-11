using System.Collections;
using Coroutines;
using UnityEngine;

public static class MonoBehaviourExtensions
{
    public static T GetOrAddComponent<T>(this MonoBehaviour source) where T : Component => source.gameObject.GetOrAddComponent<T>();

    public static Coroutine<T> StartCoroutine<T>(this MonoBehaviour source, IEnumerator routine)
    {
	    Coroutine<T> coroutine = new Coroutine<T>();
	    coroutine.InnerCoroutine = source.StartCoroutine(coroutine.Routine(routine));
	    return coroutine;
    }
    
}