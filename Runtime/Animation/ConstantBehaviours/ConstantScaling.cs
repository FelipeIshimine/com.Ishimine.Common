using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConstantScaling : MonoBehaviour
{
    public float speed = 1;
    public float timeOffset;

    public Vector3 offset = new Vector3(1, 1, 1);
    public Vector3 multiplier = new Vector3(1, 1, 1);

    public AnimationCurve curve = AnimationCurve.EaseInOut(0,0,1,1);
    public WrapMode wrapMode = WrapMode.Loop;
    private float _time;

    private bool resetOnEnable = true;

    private void OnEnable()
    {
        _time = timeOffset;
        if (resetOnEnable)
            Process(_time = 0);
    }

    private void OnValidate()
    {
        Process(timeOffset);
    }

    public void Update()
    {
        _time += Time.deltaTime * speed;
        Process(_time);
    }

    private void Process(float time)
    {
        transform.localScale = offset + multiplier * curve.Evaluate(time);
        curve.postWrapMode = wrapMode;
    }

}
