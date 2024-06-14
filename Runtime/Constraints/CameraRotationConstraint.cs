using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(RotationConstraint))]
public class CameraRotationConstraint : MonoBehaviour
{
    public RotationConstraint constraint;
    public Vector3 offset;

    private Camera cam;

    private void Reset()
    {
        constraint = GetComponent<RotationConstraint>();
        constraint.constraintActive = true;

    }

    private void Awake()
    {
	    SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDestroy()
    {
	    SceneManager.sceneLoaded -= OnSceneLoaded;
    }


    private void OnSceneLoaded(Scene arg0, LoadSceneMode arg1)
    {
	    if (!cam)
	    {
		    StopAllCoroutines();
		    if (gameObject.activeInHierarchy)
		    {
			    StartCoroutine(GetCamera());
		    }
	    }
    }

    private IEnumerator GetCamera()
    {
        yield return new WaitUntil(()=> Camera.main);
        Initialize(Camera.main);
    }

    private void OnEnable()
    {
	    if (gameObject.activeInHierarchy)
	    {
		    StartCoroutine(GetCamera());
	    }
    }

    public void Initialize(Camera cam)
    {
	    this.cam = cam;
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
