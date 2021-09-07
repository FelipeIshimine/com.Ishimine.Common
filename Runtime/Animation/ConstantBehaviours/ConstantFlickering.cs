using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConstantFlickering : MonoBehaviour
{
    public float flickerSpeed = 0.5f;

    public CanvasGroup canvasGroup;

    public AnimationCurve alphaCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    public float startAlpha = 0;
    public float endAlpha = 1;
    private float time = 0;

    public bool resetOnDisable = true;

    private void Update()
    {
        time += Time.deltaTime * flickerSpeed;
        time = time % 1;
        canvasGroup.alpha = Mathf.Lerp(startAlpha, endAlpha, alphaCurve.Evaluate(time));

    }

    public void Set(bool value)
    {
        enabled = value;
        if (resetOnDisable && !value)
        {
            time = 0;
            canvasGroup.alpha = Mathf.Lerp(startAlpha, endAlpha, alphaCurve.Evaluate(time));
        }
    }
}