using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConstantFloatingAdvanced : MonoBehaviour
{

    public Vector3 magnitude = new Vector3(1,1,1);
    public Vector3 speed = new Vector3(1, 1, 1);
    public Vector3 timeOffset = new Vector3(0, .33f, .66f);

    
    //[ContextMenuItem("Copy from LocalPosition",nameof(SetLocalPositionAsOffset))]
    
    
    public Vector3 positionOffset = new Vector3(0, 0, 0);


    private Vector3 _t = new Vector3(0,0,0);

    [Header("Noise")]
    public bool useRandomNoise = false;

    [Range(0,1)]public float noiseOffset = .33f;

    [ContextMenu(nameof(SetLocalPositionAsOffset))]
    public void SetLocalPositionAsOffset() => positionOffset = transform.localPosition;
        
    public void Update()
    {
        _t += speed * Time.deltaTime;

        if (useRandomNoise)
        {
            transform.localPosition = positionOffset + new Vector3(
                     Mathf.PerlinNoise(timeOffset.x + _t.x, 0)* magnitude.x,
                     Mathf.PerlinNoise(timeOffset.y + _t.y, noiseOffset)* magnitude.y,
                     Mathf.PerlinNoise(timeOffset.z + _t.z, noiseOffset * 2)* magnitude.z);
        }
        else
        {
            transform.localPosition = positionOffset + new Vector3(
                timeOffset.x + Mathf.Sin(_t.x) * magnitude.x,
                timeOffset.x + Mathf.Sin(_t.x) * magnitude.y,
                timeOffset.x + Mathf.Sin(_t.x) * magnitude.z);
        }
    }
}
