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

    public static Vector3 GetDirection(this Vector3 startPos, Vector3 endPos) =>
        GetDifference(startPos, endPos).normalized;

    public static float GetMagnitude(this Vector3 startPos, Vector3 endPos) =>
        GetDifference(startPos, endPos).magnitude;


    public static float GetSqrMagnitud(this Vector3 startPos, Vector3 endPos) =>
        GetDifference((Vector2)startPos, (Vector2)endPos).sqrMagnitude;

    public static Vector3 GetDifference(this Vector3 startPos, Vector3 endPos) => (endPos - startPos);

    public static float AsAngle2D(this Vector3 source) => Vector2Extensions.AsAngle(source);

    public static float GetAngle(this Vector3 startPos, Vector3 endPos)
    {
        return GetAngle((Vector2)startPos, (Vector2)endPos);
    }

    public static Vector3 SwapYZ(this Vector3 value)
    {
        (value.y, value.z) = (value.z, value.y);
        return value;
    }

    public static Vector3 SnapTo(this Vector3 value, float snap)
    {
        Vector3 retValue = value;
        retValue.x = retValue.x.SnapTo(snap);
        retValue.y = retValue.y.SnapTo(snap);
        retValue.z = retValue.z.SnapTo(snap);
        return retValue;
    }

}


