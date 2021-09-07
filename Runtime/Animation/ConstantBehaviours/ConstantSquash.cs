using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using Random = UnityEngine.Random;

public class ConstantSquash : MonoBehaviour
{
    public Vector3 multiplier = new Vector3(1, 1, 1);
    public float cycleMultiplier;
    public float scale;
    public AnimationCurve squashCurve;
    
    private float _t=0;

    private bool _active = true;
    
    private void Awake()
    {
        _t += Random.Range(0, 1f);
    }

    public void Update()
    {
        if(!_active) return;
        _t += Time.deltaTime * cycleMultiplier;
        float deformation = squashCurve.Evaluate(_t) * scale;
        transform.localScale = new Vector3(1-deformation, 1+deformation);
    }

    [Button]
    public void Activate()
    {
        _active = true;
    }

    [Button]
    public void Deactivate()
    {
        _active = false;
        _t = 0;
        transform.localScale = new Vector3(1,1);
    }


}
