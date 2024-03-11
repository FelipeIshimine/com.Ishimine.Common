using System.Collections;
using UnityEngine;

namespace Coroutines
{
	
	public class Coroutine<T> : CustomYieldInstruction
	{
		public T Result { get; private set; }
		public Coroutine InnerCoroutine { get; internal set; }

		public IEnumerator Routine(IEnumerator routine)
		{
			while (true)
			{
				if (!routine.MoveNext())
				{
					yield break;
				}

				var value = routine.Current;

				if (value is T res)
				{
					Result = res;
					yield break;
				}
				else
				{
					yield return value;
				}
			}
		}

		public override bool keepWaiting => InnerCoroutine != null;
	}
	
}