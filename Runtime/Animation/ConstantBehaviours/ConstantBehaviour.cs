using UnityEngine;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using System;

public abstract class ConstantBehaviour : MonoBehaviour
{
    [OnValueChanged("OnDurationChange"), MinValue(.001f)] public float duration = 1;
    [OnValueChanged("OnSpeedChange"), MinValue(.001f)] public float speed = 1;

    public bool resetOnEnable = false;

    public float timeOffset = 0;

    private float _time;

    private void OnDurationChange() => speed = 1 / duration;

    private void OnSpeedChange() => duration = 1 / speed;

    protected virtual void Awake()
    {
        ResetTime();
    }

    protected void ResetTime()
    {
        _time = timeOffset;
        Process(timeOffset);
    }
    protected virtual void OnEnable()
    {
        if(resetOnEnable)
            ResetTime();
    }

    public void Update()
    {
        _time += Time.deltaTime / duration;
        Process(_time);
    }

    public void SetDuration(float nDuration)
    {
        duration = nDuration;
        OnDurationChange();
    }

    internal void SetTimeOffset(float nTimeOffset)
    {
        timeOffset = nTimeOffset;
        if (!Application.isPlaying)
            Process(timeOffset);
    }

    public void SetSpeed(float nSpeed)
    {
        speed= nSpeed;
        OnSpeedChange();
    }

    private void OnValidate()
    {
        Process(timeOffset);
    }
    protected abstract void Process(float nTime);

}
