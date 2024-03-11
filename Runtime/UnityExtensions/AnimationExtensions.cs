using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utils;

public static class AnimationExtensions
{
	public static IEnumerator ScaleToLocal(this Transform source,
	                                       Vector3 endScale,
	                                       DeltaMode deltaMode,
	                                       float duration,
	                                       AnimationCurve curve = null)
	{
		var startScale = source.localScale;
		yield return EnumerableUtils.Create(
			x => source.transform.localScale = Vector3.LerpUnclamped(startScale, endScale, x),
			deltaMode,
			duration,
			curve);
	}
	
	public static IEnumerator ScaleToWorld(this Transform source,
	                                       Vector3 endScale,
	                                       DeltaMode deltaMode,
	                                       float duration,
	                                       AnimationCurve curve = null)
	{
		var startScale = source.lossyScale;
		yield return EnumerableUtils.Create(
			x => source.transform.SetGlobalScale(Vector3.LerpUnclamped(startScale, endScale, x)),
			deltaMode,
			duration,
			curve);
	}

	public static IEnumerator RotateToLocal(this Transform source,
	                                        Quaternion endValue,
	                                        DeltaMode deltaMode,
	                                        float duration,
	                                        AnimationCurve curve = null)
	{
		var startValue = source.localRotation;
		yield return EnumerableUtils.Create(
			x => source.transform.localRotation = Quaternion.LerpUnclamped(startValue, endValue, x),
			deltaMode,
			duration,
			curve);
	}
	
	public static IEnumerator RotateToWorld(this Transform source,
	                                        Quaternion endValue,
	                                        DeltaMode deltaMode,
	                                        float duration,
	                                        AnimationCurve curve = null)
	{
		var startValue = source.rotation;
		yield return EnumerableUtils.Create(
			x => source.transform.rotation = Quaternion.LerpUnclamped(startValue, endValue, x),
			deltaMode,
			duration,
			curve);
	}

	public static IEnumerator AlphaTo(this CanvasGroup source, float endValue,
	                                  DeltaMode deltaMode,
										float duration, AnimationCurve curve = null)
	{
		var startValue = source.alpha;
		yield return EnumerableUtils.Create(
			x => source.alpha = Mathf.LerpUnclamped(startValue, endValue, x),
			deltaMode,
			duration,
			curve);
	}
    
	public static IEnumerator AnchorTo(this RectTransform source,
	                                   Vector3 endScale,
	                                   DeltaMode deltaMode,
	                                   float duration,
	                                   AnimationCurve curve = null)
	{
		var startScale = source.anchoredPosition;
		yield return EnumerableUtils.Create(
			x => source.anchoredPosition = Vector3.LerpUnclamped(startScale, endScale, x),
			deltaMode,
			duration,
			curve);
	}
	

	public static Coroutine WaitForAll(this MonoBehaviour source, IEnumerable<Coroutine> coroutines, Action callback = null)
	{
		IEnumerator Routine()
		{
			foreach (Coroutine coroutine in coroutines)
			{
				yield return coroutine;
			}
			callback?.Invoke();
		}
		return source.StartCoroutine(Routine());
	}
}
