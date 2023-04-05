using UnityEngine;

public class JoystickAttribute : PropertyAttribute
{
    public float size = 50;

    public JoystickAttribute() { }
    public JoystickAttribute(float size)
    {
        this.size = size;
    }
}