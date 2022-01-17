using UnityEngine;

public static class Vector2Extensions
{
    
    public static Vector2 GetRotation(this Vector2 v, float degrees) {
        float sin = Mathf.Sin(degrees * Mathf.Deg2Rad);
        float cos = Mathf.Cos(degrees * Mathf.Deg2Rad);
         
        float tx = v.x;
        float ty = v.y;
        v.x = (cos * tx) - (sin * ty);
        v.y = (sin * tx) + (cos * ty);
        return v;
    }

    public static void Swap(this Vector2 value)=>  (value.x, value.y) = (value.y, value.x);

    public static void Swap(this Vector2Int value)=> (value.x, value.y) = (value.y, value.x);

    public static float Round(this float value, float roundValue) => Mathf.Round(value / roundValue) * roundValue; 

    public static int GetRandom(this Vector2Int value, bool includeBottom = true, bool includeTop = false) 
        => Random.Range(value.x + (includeBottom?0:1), value.y + (includeTop?1:0));
    
    
    public static Vector2 Rotate(this Vector2 v, float degrees)
    {
        float radians = degrees * Mathf.Deg2Rad;
        float sin = Mathf.Sin(radians);
        float cos = Mathf.Cos(radians);

        float tx = v.x;
        float ty = v.y;

        return new Vector2(cos * tx - sin * ty, sin * tx + cos * ty);
    }
    
    public static Vector2 GetDirection(this Vector2 startPos, Vector2 endPos)
    {
        return GetDifference(startPos, endPos).normalized;
    }
    public static float GetMagnitude(this Vector2 startPos, Vector2 endPos)
    {
        return GetDifference(startPos, endPos).magnitude;
    }
    
    
    
    public static Vector2 GetDifference(this Vector2 startPos, Vector2 endPos) => (endPos - startPos);

    public static float GetAngle(this Vector2 startPos, Vector2 endPos)
    {
        Vector2 dif = GetDirection(startPos, endPos);
        float angle = Mathf.Atan2(dif.y, dif.x) * Mathf.Rad2Deg;
        return angle;
    }

    public static float AsAngle(this Vector2 source)
    {
        Vector2 normalized = source.normalized;
        float angle = Mathf.Atan2(normalized.y, normalized.x) * Mathf.Rad2Deg;
        return angle;
    }

    public static float GetRandomBetweenXY(this Vector2 value)
    {
        return Mathf.Lerp(value.x, value.y, Random.Range(0, 1f));
    }
    
    public static Vector2 GetRandomBetween(this Vector2 startPos, Vector3 endpos)
    {
        return Vector2.Lerp(startPos, endpos, Random.Range(0, 1f));
    }

    public static bool IsValueBetween(this Vector2 source, float value) => value >= source.x && value <= source.y;

    public static Vector2 GetRandomBetweenAsRect(this Vector2 startPos, Vector3 endpos)
    {
        return new Vector2(Mathf.Lerp(startPos.x, endpos.x, Random.Range(0f, 1f)), Mathf.Lerp(startPos.y, endpos.y, Random.Range(0f, 1f)));
    }
    
    public static float GetSqrMagnitud(this Vector2 startPos, Vector2 endPos)
    {
        return GetDifference(startPos, endPos).sqrMagnitude;
    }

    public static float GetLerp(this Vector2 startPos, float t)
    {
        return Mathf.Lerp(startPos.x, startPos.y, t);
    }
    
    public static float GetLerpUnclamped(this Vector2 startPos, float t)
    {
        return Mathf.LerpUnclamped(startPos.x, startPos.y, t);
    }
    
    public static Vector2 Clamp(this Vector2 value, Vector2 min, Vector2 max)
    {
        return new Vector2(Mathf.Clamp(value.x, min.x, max.x), Mathf.Clamp(value.y, min.y, max.y));
    }
    
    public static Vector2 SnapTo(this Vector2 value, float snap)
    {
        Vector2 retValue = value;
        retValue.x = retValue.x.SnapTo(snap);
        retValue.y = retValue.y.SnapTo(snap);
        return retValue;
    }
    
    public static Vector2 GetCropping(this Vector2 fromResolution, float toAspect)
    {
        Vector2 dif = Vector2.zero;
        float aspect = Camera.main.aspect;
        if (toAspect > aspect)
        {
            //Debug.Log("Horizontal");
            float targetHeight = fromResolution.x / toAspect;
            dif.y = (fromResolution.y - targetHeight) / 2;
        }
        else
        {
            //Debug.Log("Vertical");
            float targetWidth = fromResolution.y * toAspect;
            dif.x = (fromResolution.x - targetWidth) / 2;
        }
        return dif;
    }
    
    public static Vector2 PerpendicularClockwise(this Vector2 vector2)
    {
        return new Vector2(vector2.y, -vector2.x);
    }

    public static Vector2 PerpendicularCounterClockwise(this Vector2 vector2)
    {
        return new Vector2(-vector2.y, vector2.x);
    }
}