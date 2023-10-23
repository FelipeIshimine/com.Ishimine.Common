using System;
using Cysharp.Threading.Tasks;

public static class UniTaskEventExtensions
{
	public static UniTask InvokeAsync<TSource, TEventArgs>(this Func<TSource, TEventArgs, UniTask> handlers,
	                                                       TSource source,
	                                                       TEventArgs args)
	{
		if (handlers != null)
		{
			var delegates = handlers.GetInvocationList();
			UniTask[] tasks = new UniTask[delegates.Length];
			for (var index = 0; index < delegates.Length; index++)
			{
				tasks[index] = ((Func<TSource, TEventArgs, UniTask>)delegates[index]).Invoke(source,args);
			}
			return UniTask.WhenAll(tasks);
		}
		return UniTask.CompletedTask;
	}
	
	public static UniTask InvokeAsync<TEventArgs>(this Func<TEventArgs, UniTask> handlers, TEventArgs args)
	{
		if (handlers != null)
		{
			var delegates = handlers.GetInvocationList();
			UniTask[] tasks = new UniTask[delegates.Length];
			for (var index = 0; index < delegates.Length; index++)
			{
				tasks[index] = ((Func<TEventArgs, UniTask>)delegates[index]).Invoke(args);
			}
			return UniTask.WhenAll(tasks);
		}
		return UniTask.CompletedTask;
	}
}