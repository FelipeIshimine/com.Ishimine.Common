﻿using System;
using UnityEngine;

public static class Vector3Extensions
{
    public static Vector3 RotateAround(this Vector3 point, Vector3 pivot, Vector3 angles)
    {
        Vector3 dir = point - pivot; // get point direction relative to pivot
        dir = Quaternion.Euler(angles) * dir; // rotate it
        point = dir + pivot; // calculate rotated point
        return point; // return it
    }

    public static float GetDistanceXZ(this Vector3 a, Vector3 b)
    {
        float num1 = a.x - b.x;
        float num2 = a.z - b.z;
        return (float)Math.Sqrt(num1 * num1 + num2 * num2);
    }

    public static float GetSqrDistanceXZ(this Vector3 a, Vector3 b)
    {
        float num1 = a.x - b.x;
        float num2 = a.z - b.z;
        return num1 * num1 + num2 * num2;
    }

    public static Vector3 GetDirection(this Vector3 startPos, Vector3 endPos) =>
        GetDifference(startPos, endPos).normalized;

    public static Vector3 GetDirectionXZ(this Vector3 startPos, Vector3 endPos) =>
        GetDifferenceXZ(startPos, endPos).normalized;

    public static Vector3 GetDifferenceXZ(this Vector3 startPos, Vector3 endPos) =>
        GetDifference(new Vector3(startPos.x, 0, startPos.z), new Vector3(endPos.x, 0, endPos.z));

    public static float GetMagnitude(this Vector3 startPos, Vector3 endPos) =>
        GetDifference(startPos, endPos).magnitude;


    public static float GetSqrMagnitude(this Vector3 startPos, Vector3 endPos) =>
        GetDifference((Vector2)startPos, (Vector2)endPos).sqrMagnitude;

    public static Vector3 GetDifference(this Vector3 startPos, Vector3 endPos) => (endPos - startPos);

    public static float AsAngleXY(this Vector3 source) => Vector2Extensions.AsAngle(source);

    public static float GetAngleXY(this Vector3 startPos, Vector3 endPos) => ((Vector2)startPos).GetAngle(endPos);

    public static float GetAngleXZ(this Vector3 startPos, Vector3 endPos) =>
        (new Vector2(startPos.x, startPos.z)).GetAngle(new Vector2(endPos.x, endPos.z));

    public static Vector3 SwapYZ(this Vector3 value)
    {
        (value.y, value.z) = (value.z, value.y);
        return value;
    }

    public static Vector2 AsVector2XZ(this Vector3 value) => new Vector3(value.x, value.z);

    public static Vector3 SnapTo(this Vector3 value, float snap)
    {
        Vector3 retValue = value;
        retValue.x = retValue.x.SnapTo(snap);
        retValue.y = retValue.y.SnapTo(snap);
        retValue.z = retValue.z.SnapTo(snap);
        return retValue;
    }

    public static Vector3 RotateByAxis(this Vector3 dir, Vector3 axis, float f) => Quaternion.AngleAxis(f, axis) * dir;

    public static Vector3 Clamp(this Vector3 value, Vector3 min, Vector3 max) => new Vector3(
        Mathf.Clamp(value.x, min.x, max.x), Mathf.Clamp(value.y, min.y, max.y), Mathf.Clamp(value.z, min.z, max.z));

    public static Vector3Int HexagonalRotationLeft(this Vector3Int @this) =>
        HexagonalRotationLeft(@this, Vector3Int.zero);

    public static Vector3Int HexagonalRotationLeft(this Vector3Int @this, Vector3Int pivot)
    {
        var coord = @this - pivot;

        var old = coord;

        old.x -= (coord.y - (coord.y & 1)) / 2;

        coord.x = -old.y;
        coord.y = -(-old.x - old.y);

        coord.x += (coord.y - (coord.y & 1)) / 2;

        return coord + pivot;
    }

    public static Vector3Int HexagonalRotationRight(this Vector3Int @this) =>
        HexagonalRotationRight(@this, Vector3Int.zero);

    public static Vector3Int HexagonalRotationRight(this Vector3Int @this, Vector3Int pivot)
    {
        var coord = @this - pivot;

        var old = coord;

        old.x -= (coord.y - (coord.y & 1)) / 2;

        coord.x = -(-old.x - old.y);
        coord.y = -old.x;

        coord.x += (coord.y - (coord.y & 1)) / 2;

        return coord + pivot;
    }

    public static Vector3 ClosestPointOnLine(this Vector3 point, Vector3 lineDirection, Vector3 targetPoint) => targetPoint + lineDirection * Vector3.Dot(point - targetPoint, lineDirection);
    
    public static float InverseLerp(this Vector3 value, Vector3 a, Vector3 b)
    {
	    Vector3 AB = b - a;
	    Vector3 AV = value - a;
	    var result = Vector3.Dot(AV, AB) / Vector3.Dot(AB, AB);
	    return result;
    }
}