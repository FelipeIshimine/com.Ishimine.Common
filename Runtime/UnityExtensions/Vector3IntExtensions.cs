using UnityEngine;

public static class Vector3IntExtensions
{
    public static Vector3Int GetIndexXYZ(this Vector3Int source, int index)
    {
        Vector3Int value = new Vector3Int
        {
            x = index % source.x,
            y = (index / source.x) % source.y,
            z = index / (source.x * source.y)
        };
        return value;
    }

    public static Vector3Int GetIndexXZY(this Vector3Int source, int index)
    {
        Vector3Int value = new Vector3Int
        {
            x = index % source.x,
            z = (index / source.x) % source.z,
            y = index / (source.x * source.z)
        };
        return value;
    }
}