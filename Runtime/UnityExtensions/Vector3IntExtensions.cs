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

    public static Vector3Int GetIndexYZX(this Vector3Int source, int index)
    {
        Vector3Int value = new Vector3Int
        {
            y = index % source.y,
            z = (index / source.y) % source.z,
            x = index / (source.y * source.z)
        };
        return value;
    }

    public static Vector2Int AsVector2Int(this Vector3Int @this) => new(@this.x, @this.y);
    public static Vector2Int AsVector2IntXZ(this Vector3Int @this) => new(@this.x, @this.z);


    public static int TotalCount(this Vector3Int source) => source.x * source.y * source.z;

    public static Vector3Int SwapYZ(this Vector3Int value)
    {
        (value.y, value.z) = (value.z, value.y);
        return value;
    }
}