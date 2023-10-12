using UnityEngine;

public static class ParticleSystemExtensions
{
	public static void SetStopAction(this ParticleSystem particleSystem, ParticleSystemStopAction stopAction)
	{
		var particleSystemMain = particleSystem.main;
		particleSystemMain.stopAction = stopAction;
	}
}