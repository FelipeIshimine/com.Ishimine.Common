using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Utils
{
	public static class EnumerableUtils
	{
		public static IEnumerator Create(Action<float> step, DeltaMode deltaMode, float duration, AnimationCurve curve = null)
		{
			Func<float> getTime;
			switch (deltaMode)
			{
				case DeltaMode.Scaled:
					getTime = () => Time.deltaTime;
					break;
				case DeltaMode.Unscaled:
					getTime = () => Time.unscaledDeltaTime;
					break;
				case DeltaMode.FixedScaled:
					getTime = () => Time.fixedDeltaTime;
					break;
				case DeltaMode.FixedUnscaled:
					getTime = () => Time.fixedUnscaledDeltaTime;
					break;
				default:
					throw new ArgumentOutOfRangeException(nameof(deltaMode), deltaMode, null);
			}
			curve ??= AnimationCurve.Linear(0,0,1,1);
			float t = 0;
			do
			{
				t += getTime.Invoke() / duration;
				step.Invoke(curve.Evaluate(t));
				yield return null;
			} while (t<1);
		}
		
	}
}