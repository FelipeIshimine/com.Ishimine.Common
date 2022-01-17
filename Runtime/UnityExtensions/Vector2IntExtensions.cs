using UnityEngine;

public static class Vector2IntExtensions
{
    public static int DifferenceXtoY(this Vector2Int value)
    {
        return value.y - value.x;
    }
}