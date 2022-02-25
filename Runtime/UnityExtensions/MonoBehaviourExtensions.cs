using UnityEngine;

public static class MonoBehaviourExtensions
{
    public static T GetOrAddComponent<T>(this MonoBehaviour source) where T : Component => source.gameObject.GetOrAddComponent<T>();
}