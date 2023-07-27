using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaitConcurrent : IEnumerator<Coroutine>
{
	private readonly IEnumerator<Coroutine> enumerator;
	public WaitConcurrent(params Coroutine[] coroutines)
	{
		enumerator = ((IEnumerable<Coroutine>)coroutines).GetEnumerator();
	}
	public bool MoveNext() { return enumerator.MoveNext(); }

	public void Reset() { enumerator.Reset(); }

	public Coroutine Current => enumerator.Current;

	object IEnumerator.Current => ((IEnumerator)enumerator).Current;

	public void Dispose() { enumerator.Dispose(); }
}