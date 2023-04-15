using UnityEngine;

public static class RayExtensions
{
    public static Vector3 ClosestPointOnLine(this Ray ray, Vector3 linePoint, Vector3 lineDirection) => linePoint + lineDirection * Vector3.Dot(ray.origin - linePoint, lineDirection);
}