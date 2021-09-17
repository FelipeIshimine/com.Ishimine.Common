using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConstantMaterialAlpha : ConstantBehaviour
{
    public Renderer _renderer;
    private Material _material;

    public Gradient gradient; 

    protected override void Awake()
    {
        base.Awake();
        _material = _renderer.material;
    }

    protected override void Process(float nTime)
    {
        if (_material != null)
            _material.color = gradient.Evaluate(Mathf.Repeat(nTime, 1));
    }
}
