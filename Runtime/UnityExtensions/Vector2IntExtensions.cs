using UnityEngine;

public static class Vector2IntExtensions
{
    public static int DifferenceXtoY(this Vector2Int value)
    {
        return value.y - value.x;
    }

    public static Vector3Int AsVector3IntXY(this Vector2Int value)=> new Vector3Int(value.x,value.y);
    public static Vector3Int AsVector3IntXZ(this Vector2Int value)=> new Vector3Int(value.x,0,value.y);

    public static float GetPercentageFor(this Vector2Int source, float value, bool clamp) => value.GetAsPercentageBetween(source, clamp);

    public static Vector3Int AxialToCube(this Vector2Int @this) => new Vector3Int(@this.x, @this.y, -@this.x-@this.y);

}