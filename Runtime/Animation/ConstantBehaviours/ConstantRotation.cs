using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConstantRotation : MonoBehaviour
{
    public Vector3 offset = new Vector3();
    public Vector3 speed = new Vector3(0,0,1);

    public float timeOffset = 0;
    
    [SerializeField] private bool resetOnDisable = true;
    private float _time = 0;

    private void OnDisable()
    {
        _time = 0;
    }

    private void Update()
    {
        _time += Time.deltaTime;
        Process(_time);
    }

    private void Process(float time)
    {
        transform.rotation = Quaternion.Euler(offset.x + time * speed.x, offset.y + time * speed.y, offset.z + time * speed.z);
    }

    private void OnValidate()
    {
        Process(timeOffset);
    }
}
