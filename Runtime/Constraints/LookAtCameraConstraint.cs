using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Animations;

[RequireComponent(typeof(LookAtConstraint))]
public class LookAtCameraConstraint : MonoBehaviour
{
    public LookAtConstraint constraint;

    public Vector3 offset;
    
    private void Reset() => constraint = GetComponent<LookAtConstraint>();

    private void OnEnable()
    {
        if (constraint.sourceCount > 0)
            for (int i = constraint.sourceCount - 1; i >= 0; i--)
                constraint.RemoveSource(i);

        if(!Camera.main) return;
        
        constraint.AddSource(new ConstraintSource()
            { sourceTransform = Camera.main.transform, weight = 1 });

        constraint.locked = false;
        constraint.rotationOffset = offset;
    }
    private void OnDisable()
    {
        if(constraint) constraint.RemoveSource(0);
    }
    
    
    
    
}
