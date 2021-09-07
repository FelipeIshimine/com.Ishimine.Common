using System;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider2D))]
public class OnTrigger2D : MonoBehaviour
{
    public event Action<Collider2D> OnEnter;
    public event Action<Collider2D> OnStay;
    public event Action<Collider2D> OnExit;

    public bool useUnityEvents = false;

    [ShowIf(nameof(useUnityEvents))] public UnityEvent<Collider2D> OnEnterUE;
    [ShowIf(nameof(useUnityEvents))] public UnityEvent<Collider2D> OnStayUE;
    [ShowIf(nameof(useUnityEvents))] public UnityEvent<Collider2D> OnExitUE;

    public void OnTriggerEnter2D(Collider2D other)
    {
        if(!enabled) return;
        OnEnter?.Invoke(other);
        if(useUnityEvents) OnEnterUE?.Invoke(other);
    }

    public void OnTriggerStay2D(Collider2D other)
    {
        if(!enabled) return;
        OnStay?.Invoke(other);
        if(useUnityEvents) OnStayUE?.Invoke(other);
    }
   
    public void OnTriggerExit2D(Collider2D other)
    {
        if(!enabled) return;
        OnExit?.Invoke(other);
        if(useUnityEvents) OnExitUE?.Invoke(other);
    }
}