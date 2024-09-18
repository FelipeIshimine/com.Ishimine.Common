using System;
using UnityEngine;
using UnityEngine.Events;


public class 
    OnTrigger : MonoBehaviour
{
    public event Action<Collider> OnEnter;
    public event Action<Collider> OnStay;
    public event Action<Collider> OnExit;

    public bool useUnityEvents = false;

    public UnityEvent<Collider> OnEnterUE;
    public UnityEvent<Collider> OnStayUE;
    public UnityEvent<Collider> OnExitUE;

    public void OnTriggerEnter(Collider other)
    {
        if(!enabled) return;
        OnEnter?.Invoke(other);
        if(useUnityEvents) OnEnterUE?.Invoke(other);
    }

    public void OnTriggerStay(Collider other)
    {
        if(!enabled) return;
        OnStay?.Invoke(other);
        if(useUnityEvents) OnStayUE?.Invoke(other);
    }
   
    public void OnTriggerExit(Collider other)
    {
        if(!enabled) return;
        OnExit?.Invoke(other);
        if(useUnityEvents) OnExitUE?.Invoke(other);
    }
}