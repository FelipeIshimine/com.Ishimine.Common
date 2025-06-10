using UnityEngine;

public class JoystickAttribute : PropertyAttribute
{
	public readonly bool UseMultiplier;
	public float MaxMagnitude;
    public float Step;
    
	public JoystickAttribute(bool useMultiplier, float maxMagnitude, float step = 0.01f)
    {
        Step = step;
		this.UseMultiplier = useMultiplier;
		this.MaxMagnitude = maxMagnitude;
	}
	
	public JoystickAttribute(bool useMultiplier, float step = 0.01f)
	{
        Step = step;
		this.UseMultiplier = useMultiplier;
		this.MaxMagnitude = 1;
	}
	
	public JoystickAttribute()
	{
		this.UseMultiplier = false;
		this.MaxMagnitude = 1;
	}
}