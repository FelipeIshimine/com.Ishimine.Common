using System;
using System.Threading.Tasks;

public static class TaskEventExtensions
{
	public static Task InvokeAsync<TSource, TEventArgs>(this Func<TSource, TEventArgs, Task> handlers,
	                                                    TSource source,
	                                                    TEventArgs args)
	{
		if (handlers != null)
		{
			var delegates = handlers.GetInvocationList();
			Task[] tasks = new Task[delegates.Length];
			for (var index = 0; index < delegates.Length; index++)
			{
				tasks[index] = ((Func<TSource, TEventArgs, Task>)delegates[index]).Invoke(source,args);
			}
			return Task.WhenAll(tasks);
		}
		return Task.CompletedTask;
	}
	
	public static Task InvokeAsync<TEventArgs>(this Func<TEventArgs, Task> handlers, TEventArgs args)
	{
		if (handlers != null)
		{
			var delegates = handlers.GetInvocationList();
			Task[] tasks = new Task[delegates.Length];
			for (var index = 0; index < delegates.Length; index++)
			{
				tasks[index] = ((Func<TEventArgs, Task>)delegates[index]).Invoke(args);
			}
			return Task.WhenAll(tasks);
		}
		return Task.CompletedTask;
	}
}