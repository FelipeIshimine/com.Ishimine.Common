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

    private void OnValidate()
    {
        if (!constraint) constraint = GetComponent<LookAtConstraint>();
    }

    private void OnEnable() => constraint.AddSource(new ConstraintSource()
        { sourceTransform = Camera.main.transform, weight = 1 });
    private void OnDisable() => constraint.RemoveSource(0);
}
