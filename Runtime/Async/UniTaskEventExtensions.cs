using System;
using Cysharp.Threading.Tasks;

public static class UniTaskEventExtensions
{
	public static UniTask InvokeParallelAsync<TSource, TEventArgs>(this Func<TSource, TEventArgs, UniTask> handlers,
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
	
	public static UniTask InvokeParallelAsync<TEventArgs>(this Func<TEventArgs, UniTask> handlers, TEventArgs args)
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
	
	public static UniTask InvokeParallelAsync(this Func<UniTask> handlers)
	{
		if (handlers != null)
		{
			var delegates = handlers.GetInvocationList();
			UniTask[] tasks = new UniTask[delegates.Length];
			for (var index = 0; index < delegates.Length; index++)
			{
				tasks[index] = ((Func<UniTask>)delegates[index]).Invoke();
			}
			return UniTask.WhenAll(tasks);
		}
		return UniTask.CompletedTask;
	}
	
	
	public static async UniTask InvokeAsync<TSource, TEventArgs>(this Func<TSource, TEventArgs, UniTask> handlers,
	                                                             TSource source,
	                                                             TEventArgs args)
	{
		if (handlers != null)
		{
			var delegates = handlers.GetInvocationList();
			for (var index = 0; index < delegates.Length; index++)
			{
				await ((Func<TSource, TEventArgs, UniTask>)delegates[index]).Invoke(source,args);
			}
		}
	}
	
	public static async UniTask InvokeAsync<TEventArgs>(this Func<TEventArgs, UniTask> handlers, TEventArgs args)
	{
		if (handlers != null)
		{
			var delegates = handlers.GetInvocationList();
			for (var index = 0; index < delegates.Length; index++)
			{
				await ((Func<TEventArgs, UniTask>)delegates[index]).Invoke(args);
			}
		}
	}
	
	public static async UniTask InvokeAsync(this Func<UniTask> handlers)
	{
		if (handlers != null)
		{
			var delegates = handlers.GetInvocationList();
			for (var index = 0; index < delegates.Length; index++)
			{
				await ((Func<UniTask>)delegates[index]).Invoke();
			}
		}
	}
}