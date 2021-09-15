using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConstantRotation : ConstantBehaviour
{
    public Vector3 offset = new Vector3();
    public Vector3 axisSpeed = new Vector3(0,0,1);


    protected override void Process(float nTime)
    {
        transform.rotation = Quaternion.Euler(offset.x + nTime * axisSpeed.x, offset.y + nTime * axisSpeed.y, offset.z + nTime * axisSpeed.z);
    }
}
