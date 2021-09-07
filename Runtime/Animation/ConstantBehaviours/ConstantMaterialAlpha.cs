using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConstantMaterialAlpha : MonoBehaviour
{
    public float speed = 1;
    public float timeOffset;
    
    public Renderer _renderer;
    private Material _material;

    public Gradient gradient; 
    
    private float _time;
    
    public void Awake()
    {
        _material = _renderer.material;
        _time = timeOffset;
    }

    private void OnValidate()
    {
        if (!_renderer)
            _renderer = GetComponent<Renderer>();
    }

    public void Update()
    {
        _time += Time.deltaTime * speed;
        _material.color = gradient.Evaluate(Mathf.Repeat(_time,1));
    }
}
