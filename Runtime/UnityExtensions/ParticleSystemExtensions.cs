using UnityEngine;

public static class ParticleSystemExtensions
{
	public static void SetStopAction(this ParticleSystem particleSystem, ParticleSystemStopAction stopAction)
	{
		var particleSystemMain = particleSystem.main;
		particleSystemMain.stopAction = stopAction;
	}
	public static void Toggle(this ParticleSystem particleSystem, bool shouldPlay)
	{
		if (shouldPlay)
		{
			particleSystem.Play();
		}
		else
		{
			particleSystem.Stop(true, ParticleSystemStopBehavior.StopEmitting);
		}
	}
}