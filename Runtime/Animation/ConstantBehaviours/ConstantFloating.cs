﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConstantFloating : MonoBehaviour
{
    public float speed = 1;
    public float height = 1;
    private float _time;
    private Vector3 _offset;
    
    private void Start()
    {
        _offset = transform.localPosition;
    }

    private void OnEnable()
    {
        _time = 0;
    }

    public void Update()
    {
        _time += Time.deltaTime * speed * Mathf.PI;
        //transform.localPosition = _offset + Mathf.Sin(_time) * height * Vector3.up;
        transform.localPosition = new Vector3(transform.localPosition.x, _offset.y + Mathf.Sin(_time) * height, transform.localPosition.z);
    }
}
