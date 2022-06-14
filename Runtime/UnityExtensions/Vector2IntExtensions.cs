using UnityEngine;

public static class Vector2IntExtensions
{
    public static int DifferenceXtoY(this Vector2Int value)
    {
        return value.y - value.x;
    }

    public static Vector3Int AsVector3IntXY(this Vector2Int value)=> new Vector3Int(value.x,value.y);
    public static Vector3Int AsVector3IntXZ(this Vector2Int value)=> new Vector3Int(value.x,0,value.y);

    
}