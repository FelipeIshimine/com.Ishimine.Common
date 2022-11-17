using UnityEngine;

public class MathUtils
{
    public static float Apothem(float radius) => Mathf.Sqrt(Mathf.Pow(radius, 2f) - Mathf.Pow(radius * .5f, 2));

    public static float Remap(float iMin, float iMax, float oMin, float oMax, float value)
    {
        float t = Mathf.InverseLerp(iMin, iMax, value);
        return Mathf.Lerp(oMin, oMax, t);
    }
}