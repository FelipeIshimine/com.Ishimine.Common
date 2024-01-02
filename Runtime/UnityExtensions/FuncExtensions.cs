using System;
using System.Collections.Generic;

public static class FuncExtensions
{
	public static IEnumerable<T> EnumerableInvoke<T>(this Func<T> handler)
	{
		if (handler != null)
		{
			var delegates = handler.GetInvocationList();
			for (var index = 0; index < delegates.Length; index++)
			{
				yield return ((Func<T>)delegates[index]).Invoke();
			}
		}
	}
}