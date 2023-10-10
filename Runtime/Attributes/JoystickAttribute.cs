using UnityEngine;

public class JoystickAttribute : PropertyAttribute
{
	public readonly bool UseMultiplier;
	public float MaxMagnitude;

	public JoystickAttribute(bool useMultiplier, float maxMagnitude)
	{
		this.UseMultiplier = useMultiplier;
		this.MaxMagnitude = maxMagnitude;
	}
	
	public JoystickAttribute(bool useMultiplier)
	{
		this.UseMultiplier = useMultiplier;
		this.MaxMagnitude = 1;
	}
	
	public JoystickAttribute()
	{
		this.UseMultiplier = false;
		this.MaxMagnitude = 1;
	}
}