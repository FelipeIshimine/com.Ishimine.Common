using System.Collections;

public class WaitSequence : IEnumerator
{
	private readonly IEnumerator enumerator;
	public WaitSequence(params IEnumerator[] enumerators)
	{
		enumerator = enumerators.GetEnumerator();
	}
	
	public bool MoveNext() => enumerator.MoveNext();

	public void Reset() => enumerator.Reset();

	object IEnumerator.Current => enumerator.Current;
}