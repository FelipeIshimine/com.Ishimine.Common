using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GlobalUpdate : BaseMonoSingleton<GlobalUpdate>
{
    public static event Action<bool> OnApplicationPauseEvent;
    public static event Action<bool> OnApplicationFocusEvent;
    public static event Action OnUpdateEvent;
    public static event  Action OnFixedUpdateEvent;
    public static event  Action OnLateUpdateEvent;

    public static readonly Queue<Action> UpdateEventQueue = new Queue<Action>();
    public static readonly Queue<Action> LateUpdateEventQueue = new Queue<Action>();
    public static readonly Queue<Action> FixedUpdateEventQueue = new Queue<Action>();

    [RuntimeInitializeOnLoadMethod]
    private static void Initialize()
    {
        var go = new GameObject().AddComponent<GlobalUpdate>();
        go.name = "Global Update";
    }

    private void Update()
    {
        while (UpdateEventQueue.Count > 0)
            UpdateEventQueue.Dequeue().Invoke();
        OnUpdateEvent?.Invoke();
    }

    private void LateUpdate()
    {
        while (LateUpdateEventQueue.Count > 0)
            LateUpdateEventQueue.Dequeue().Invoke();
        OnLateUpdateEvent?.Invoke();
    }
    private void FixedUpdate()
    {
        while (FixedUpdateEventQueue.Count > 0)
            FixedUpdateEventQueue.Dequeue().Invoke();
        OnFixedUpdateEvent?.Invoke();
    }
    private void OnApplicationPause(bool value) => OnApplicationPauseEvent?.Invoke(value);
    private void OnApplicationFocus(bool value) => OnApplicationFocusEvent?.Invoke(value);
    
    
}
