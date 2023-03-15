using System.Collections;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public static class AsyncUtils
{
	public static Task WaitAsync(float waitTime) => WaitAsync(waitTime, CancellationToken.None);
	
	public static async Task WaitAsync(float waitTime, CancellationToken cancellationToken)
	{
		float endTime = Time.time + waitTime;
		while (Time.time < endTime)
		{
			if(cancellationToken.IsCancellationRequested)
				cancellationToken.ThrowIfCancellationRequested();
			else
				await Task.Yield();
		}
	}
	
	public static async Task WaitRealtimeAsync(float duration) => await Task.Delay((int)(duration * 1000));

    public static async Task PlayCoroutine(IEnumerator routine)
    {
        var coroutine = GlobalUpdate.Instance.StartCoroutine(routine);
        while (coroutine != null)
            await Task.Yield();
    }
}