using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

[RequireComponent(typeof(RotationConstraint))]
public class CameraRotationConstraint : MonoBehaviour
{
    public RotationConstraint constraint;
    public Vector3 offset;

    private void Reset()
    {
        constraint = GetComponent<RotationConstraint>();
        constraint.constraintActive = true;
    }

    private IEnumerator GetCamera()
    {
        yield return new WaitUntil(()=> Camera.main);
        Initialize(Camera.main);
    }

    private void OnEnable()
    {
        StartCoroutine(GetCamera());
    }

    public void Initialize(Camera cam)
    {
        if (constraint.sourceCount > 0)
            for (int i = constraint.sourceCount - 1; i >= 0; i--)
                constraint.RemoveSource(i);

        constraint.AddSource(new ConstraintSource()
            { sourceTransform = cam.transform, weight = 1 });

        constraint.locked = false;
        constraint.rotationOffset = offset;
    }

    private void OnDisable()
    {
        if(constraint && constraint.sourceCount > 0) constraint.RemoveSource(0);
    }
}
