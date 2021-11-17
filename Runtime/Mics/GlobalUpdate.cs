using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GlobalUpdate : BaseMonoSingleton<GlobalUpdate>
{
    public static event Action<bool> OnApplicationPauseEvent;
    public static event Action OnUpdateEvent;
    public static event  Action OnFixedUpdateEvent;
    public static event  Action OnLateUpdateEvent;

    [RuntimeInitializeOnLoadMethod]
    private static void Initialize()
    {
        var go = new GameObject().AddComponent<GlobalUpdate>();
        go.name = "Global Update";
    }

    private void Update() =>  OnUpdateEvent?.Invoke();
    private void LateUpdate() =>OnLateUpdateEvent?.Invoke();
    private void FixedUpdate() => OnFixedUpdateEvent?.Invoke();
    private void OnApplicationPause(bool value) => OnApplicationPauseEvent?.Invoke(value);

}
