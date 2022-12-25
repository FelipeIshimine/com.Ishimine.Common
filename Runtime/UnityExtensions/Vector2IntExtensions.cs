using UnityEngine;

public static class Vector2IntExtensions
{
    public static int DifferenceXtoY(this Vector2Int value)
    {
        return value.y - value.x;
    }

    public static int LerpRound(this Vector2Int value, float t) => Mathf.RoundToInt(Mathf.Lerp(value.x, value.y, t));
    public static int LerpCeil(this Vector2Int value, float t) => Mathf.CeilToInt(Mathf.Lerp(value.x, value.y, t));
    public static int LerpFloor(this Vector2Int value, float t) => Mathf.FloorToInt(Mathf.Lerp(value.x, value.y, t));
    
    
    public static int LerpUnclampedRound(this Vector2Int value, float t) => Mathf.RoundToInt(Mathf.LerpUnclamped(value.x, value.y, t));
    public static int LerpUnclampedCeil(this Vector2Int value, float t) => Mathf.CeilToInt(Mathf.LerpUnclamped(value.x, value.y, t));
    public static int LerpUnclampedFloor(this Vector2Int value, float t) => Mathf.FloorToInt(Mathf.LerpUnclamped(value.x, value.y, t));
    
    public static Vector3Int AsVector3IntXY(this Vector2Int value)=> new Vector3Int(value.x,value.y);
    public static Vector3Int AsVector3IntXZ(this Vector2Int value)=> new Vector3Int(value.x,0,value.y);

    public static float GetPercentageFor(this Vector2Int source, float value, bool clamp) => value.GetAsPercentageBetween(source, clamp);

    public static Vector3Int AxialToCube(this Vector2Int @this) => new Vector3Int(@this.x, @this.y, -@this.x-@this.y);
    
    
    public static Vector2Int HexagonalRotationLeft(this Vector2Int @this) => HexagonalRotationLeft(@this, Vector2Int.zero);

    public static Vector2Int HexagonalRotationLeft(this Vector2Int @this, Vector2Int pivot)
    {
        var coord = @this - pivot;

        var old = coord;
            
        old.x -= (coord.y - (coord.y & 1)) / 2;

        coord.x = -old.y;
        coord.y = -(-old.x - old.y);

        coord.x += (coord.y - (coord.y & 1)) / 2;

        return coord + pivot;
    }
    
    public static Vector2Int HexagonalRotationRight(this Vector2Int @this) => HexagonalRotationRight(@this, Vector2Int.zero);

    public static Vector2Int HexagonalRotationRight(this Vector2Int @this, Vector2Int pivot)
    {
        var coord = @this - pivot;

        var old = coord;
            
        old.x -= (coord.y - (coord.y & 1)) / 2;

        coord.x = -(-old.x - old.y);
        coord.y = -old.x;

        coord.x += (coord.y - (coord.y & 1)) / 2;

        return coord + pivot;
    }

}