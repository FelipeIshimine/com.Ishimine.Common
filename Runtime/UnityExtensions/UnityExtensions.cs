using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

public static class UnityExtensions
{
    public const float GoldenRatio = 1.61803398875f;

    public static T Last<T>(this List<T> source) => source[source.Count - 1];
    public static T First<T>(this List<T> source) => source[0];
    
    public static T Last<T>(this T[] source) => source[source.Length - 1];
    public static T First<T>(this T[] source) => source[0];
    
    public static List<Transform> GetAllChildren(this Transform source)
    {
        List<Transform> children = new List<Transform>();
        foreach (Transform child in source)
            children.Add(child);
        return children;
    }
   
    public static void SetLayerRecursively(this GameObject source, int layer)
    {
        foreach (var item in source.transform.GetComponentsInChildren<Transform>(true))
            item.gameObject.layer = layer;
    }
    public static void PlayCoroutine(this MonoBehaviour source, ref IEnumerator rutine, Func<IEnumerator> rutineMethod)
    {
        if (!source.isActiveAndEnabled)
            throw new Exception("El objeto está desactivado o el componente disabled.");

        if (rutine != null)
            source.StopCoroutine(rutine);

        rutine = rutineMethod();
        source.StartCoroutine(rutine);
    }

    public static IEnumerator CreateQuickSteppedRutine(this MonoBehaviour source, float duration, DeltaTimeType deltaTime, Action<float> step, Action callback = null)
    {
        Func<float> deltaStep;
        switch (deltaTime)
        {
            default:
            case DeltaTimeType.deltaTime:
                deltaStep = ()=> Time.deltaTime;
                break;
            case DeltaTimeType.fixedDeltaTime:
                deltaStep = ()=> Time.fixedDeltaTime;
                break;
            case DeltaTimeType.unscaledDeltaTime:
                deltaStep = ()=> Time.unscaledDeltaTime;
                break;
            case DeltaTimeType.fixedUnscaledDeltaTime:
                deltaStep = ()=> Time.fixedUnscaledDeltaTime;
                break;
        }

        float t = 0;
        do
        {
            step.Invoke(t);
            yield return null;
            t += deltaStep.Invoke() / duration;
        } while (t<1);
        step.Invoke(1);
        callback?.Invoke();
    }

    public static IEnumerator NonStopAnimation(this MonoBehaviour source, DeltaTimeType deltaTime, Action<float> stepAnimation)
    {
        Func<float> deltaStep;
        switch (deltaTime)
        {
            default:
            case DeltaTimeType.deltaTime:
                deltaStep = ()=> Time.deltaTime;
                break;
            case DeltaTimeType.fixedDeltaTime:
                deltaStep = ()=> Time.fixedDeltaTime;
                break;
            case DeltaTimeType.unscaledDeltaTime:
                deltaStep = ()=> Time.unscaledDeltaTime;
                break;
            case DeltaTimeType.fixedUnscaledDeltaTime:
                deltaStep = ()=> Time.fixedUnscaledDeltaTime;
                break;
        }

        do
        {
            yield return null;
            stepAnimation.Invoke(deltaStep.Invoke());
        } while (true);
    }

    public static void WaitAndExecute(this MonoBehaviour source, ref IEnumerator rutine, float waitTime, bool isRealTime, Action callback)
    {
        if (callback == null)
            Debug.LogError("Callback is null");
        source.PlayCoroutine(ref rutine, () => WaitAndExecuteRutine(waitTime, isRealTime, callback));
    }

    public static IEnumerator WaitAndExecuteRutine(float waitTime, bool isRealTime, Action callback)
    {
        if (isRealTime)
            yield return new WaitForSecondsRealtime(waitTime);
        else
            yield return new WaitForSeconds(waitTime);

        callback.Invoke();
    }
    
    public static void WaitForNextFrameAndExecute(this MonoBehaviour source, ref IEnumerator rutine, Action callback)
    {
        if (callback == null)
            Debug.LogError("Callback is null");
        source.PlayCoroutine(ref rutine, () => WaitForNextFrameRoutine(callback));
    }

    public static IEnumerator WaitForNextFrameRoutine(Action callback)
    {
        yield return null;
        callback.Invoke();
    }

    public static void WaitForFixedUpdateAndExecute(this MonoBehaviour source, ref IEnumerator rutine, Action callback)
    {
        if (callback == null)
            Debug.LogError("Callback is null");
        source.PlayCoroutine(ref rutine, () => WaitForFixedUpdateRoutine(callback));
    }
    

    public static IEnumerator WaitForFixedUpdateRoutine(Action callback)
    {
        yield return new WaitForFixedUpdate();
        callback.Invoke();
    }
    
    public static void WaitForEndOfFrameAndExecute(this MonoBehaviour source, ref IEnumerator rutine, Action callback)
    {
        if (callback == null)
            Debug.LogError("Callback is null");
        source.PlayCoroutine(ref rutine, () => WaitForEndOfFrameRoutine(callback));
    }
    
    public static IEnumerator WaitForEndOfFrameRoutine(Action callback)
    {
        yield return new WaitForEndOfFrame();
        callback.Invoke();
    }

    public static bool IsBetweenInclusive(this int value, Vector2Int minMax)
    {
        return value >= minMax.x && value <= minMax.y;
    }

    public static bool IsBetweenExclusive(this int value, Vector2Int minMax)
    {
        return value > minMax.x && value < minMax.y;
    }
    
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

    /// <summary>
    /// Extension method to check if a layer is in a layermask
    /// </summary>
    /// <param name="mask"></param>
    /// <param name="layer"></param>
    /// <returns></returns>
    public static bool Contains(this LayerMask mask, int layer)
    {
        return mask == (mask | (1 << layer));
    }

    public static Vector2 Rotate(this Vector2 v, float degrees)
    {
        float radians = degrees * Mathf.Deg2Rad;
        float sin = Mathf.Sin(radians);
        float cos = Mathf.Cos(radians);

        float tx = v.x;
        float ty = v.y;

        return new Vector2(cos * tx - sin * ty, sin * tx + cos * ty);
    }

    public static Vector3 RotateAround(this Vector3 point, Vector3 pivot, Vector3 angles) {
        Vector3 dir = point - pivot; // get point direction relative to pivot
        dir = Quaternion.Euler(angles) * dir; // rotate it
        point = dir + pivot; // calculate rotated point
        return point; // return it
    }

    public static Vector2 GetDirection(this Vector2 startPos, Vector2 endPos)
    {
        return GetDifference(startPos, endPos).normalized;
    }

    public static Vector3 GetDirection(this Vector3 startPos, Vector3 endPos)
    {
        return GetDifference(startPos, endPos).normalized;
    }

    public static float GetMagnitude(this Vector2 startPos, Vector2 endPos)
    {
        return GetDifference(startPos, endPos).magnitude;
    }

    public static float GetMagnitude(this Vector3 startPos, Vector3 endPos)
    {
        return GetDifference(startPos, endPos).magnitude;
    }

    public static float GetSqrMagnitud(this Vector2 startPos, Vector2 endPos)
    {
        return GetDifference(startPos, endPos).sqrMagnitude;
    }

    public static float GetSqrMagnitud(this Vector3 startPos, Vector3 endPos)
    {
        return GetDifference((Vector2)startPos, (Vector2)endPos).sqrMagnitude;
    }

    public static float GetLerp(this Vector2 startPos, float t)
    {
        return Mathf.Lerp(startPos.x, startPos.y, t);
    }
    
    public static float GetLerpUnclamped(this Vector2 startPos, float t)
    {
        return Mathf.LerpUnclamped(startPos.x, startPos.y, t);
    }
       
    public static Vector2 GetDifference(this Vector2 startPos, Vector2 endPos) => (endPos - startPos);

    public static Vector3 GetDifference(this Vector3 startPos, Vector3 endPos) =>(endPos - startPos);

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

    public static float AsAngle2D(this Vector3 source)
    {
        return AsAngle((Vector2)source);
    }

    public static Vector2 AsVector(this float degree)
    {
        return RadianToVector2(degree * Mathf.Deg2Rad);
    }

    public static Vector2 RadianToVector2(float radian)
    {
        return new Vector2(Mathf.Cos(radian), Mathf.Sin(radian));
    }
    public static float GetAngle(this Vector3 startPos, Vector3 endPos)
    {
        return GetAngle((Vector2)startPos, (Vector2)endPos);
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

    public static Vector2 ClampPositionToView(this Camera camera, Vector2 value)
    {
        float hSize = GetHorizontalSize(camera);
        return new Vector2(Mathf.Clamp(value.x, -hSize, hSize), Mathf.Clamp(value.y, -camera.orthographicSize, camera.orthographicSize)) + (Vector2)camera.transform.position;
    }

    public static Vector2 Clamp(this Vector2 value, Vector2 min, Vector2 max)
    {
        return new Vector2(Mathf.Clamp(value.x, min.x, max.x), Mathf.Clamp(value.y, min.y, max.y));
    }

    public static Vector3 SwapYZ(this Vector3 value)
    {
        (value.y, value.z) = (value.z, value.y);
        return value;
    }
    
    public static bool IsInsideCameraView(this Camera camera, Vector2 value)
    {
        value -= (Vector2)camera.transform.position;
        return Mathf.Abs(value.y) < camera.orthographicSize && Mathf.Abs(value.x) < camera.GetHorizontalSize();
    }

    public static float GetHorizontalSize(this Camera camera)
    {
        return camera.aspect * camera.orthographicSize * 2;
    }

    public static Vector2 GetOrtographicSize(this Camera camera)
    {
        return new Vector2(camera.GetHorizontalSize(), camera.orthographicSize * 2);
    }

    public static bool ContainsInLocalSpace(this BoxCollider2D boxCollider2D, Vector2 worldSpacePoint)
    {
        worldSpacePoint = boxCollider2D.transform.InverseTransformPoint(worldSpacePoint);
        return Mathf.Abs(worldSpacePoint.x) <= boxCollider2D.size.x / 2 && Mathf.Abs(worldSpacePoint.y) <= boxCollider2D.size.y / 2;
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

    public static Rect SetCenter(this ref Rect rect, Vector2 newPos)
    {
        rect.Set(newPos.x - rect.width / 2, newPos.y - rect.height / 2, rect.width, rect.height);
        return rect;
    }

    public static float GetPointDistanceFromLine(Vector2 lineStart, Vector2 lineEnd, Vector2 targetPoint)=>(targetPoint.x - lineStart.x) * (lineEnd.y-lineStart.y)-(targetPoint.y-lineStart.y)*(lineEnd.x-lineStart.x);
    

    public static bool LineIntersection(Vector2 p1, Vector2 p2, Vector2 p3, Vector2 p4, ref Vector2 intersection)
    {

        float Ax, Bx, Cx, Ay, By, Cy, d, e, f, num/*,offset*/;

        float x1lo, x1hi, y1lo, y1hi;

        Ax = p2.x - p1.x;

        Bx = p3.x - p4.x;

        // X bound box test/
        if (Ax < 0)
        {
            x1lo = p2.x;
            x1hi = p1.x;
        }
        else
        {
            x1hi = p2.x;
            x1lo = p1.x;
        }

        if (Bx > 0)
        {
            if (x1hi < p4.x || p3.x < x1lo) return false;
        }
        else
        {
            if (x1hi < p3.x || p4.x < x1lo) return false;
        }

        Ay = p2.y - p1.y;
        By = p3.y - p4.y;

        // Y bound box test//
        if (Ay < 0)
        {
            y1lo = p2.y;
            y1hi = p1.y;
        }
        else
        {
            y1hi = p2.y;
            y1lo = p1.y;
        }

        if (By > 0)
        {
            if (y1hi < p4.y || p3.y < y1lo) return false;
        }
        else
        {
            if (y1hi < p3.y || p4.y < y1lo) return false;
        }

        Cx = p1.x - p3.x;
        Cy = p1.y - p3.y;
        d = By * Cx - Bx * Cy;  // alpha numerator//
        f = Ay * Bx - Ax * By;  // both denominator//
        // alpha tests//
        if (f > 0)
        {
            if (d < 0 || d > f) return false;
        }
        else
        {
            if (d > 0 || d < f) return false;
        }
        e = Ax * Cy - Ay * Cx;  // beta numerator//
        // beta tests //
        if (f > 0)
        {
            if (e < 0 || e > f) return false;
        }
        else
        {
            if (e > 0 || e < f) return false;
        }
        // check if they are parallel
        if (f == 0) return false;
        // compute intersection coordinates //
        num = d * Ax; // numerator //

        //    offset = same_sign(num,f) ? f*0.5f : -f*0.5f;   // round direction //

        //    intersection.x = p1.x + (num+offset) / f;
        intersection.x = p1.x + num / f;

        num = d * Ay;

        //    offset = same_sign(num,f) ? f*0.5f : -f*0.5f;
        //    intersection.y = p1.y + (num+offset) / f;
        intersection.y = p1.y + num / f;
        return true;
    }

    public static Vector2 PerpendicularClockwise(this Vector2 vector2)
    {
        return new Vector2(vector2.y, -vector2.x);
    }

    public static Vector2 PerpendicularCounterClockwise(this Vector2 vector2)
    {
        return new Vector2(-vector2.y, vector2.x);
    }

    public static T GetRandom<T>(this T[] value)
    {
        if (value.Length == 0) throw new Exception("Array is empty");
        return value[Random.Range(0, value.Length)];
    }

    public static T GetRandom<T>(this IReadOnlyList<T> value)
    {
        if (value.Count == 0) throw new Exception("List is empty");

        return value[Random.Range(0, value.Count)];
    }

    public static List<T> GetRandom<T>(this IReadOnlyList<T> value, int count, bool canRepeat)
    {
        if (!canRepeat && count > value.Count)
            Debug.LogError("Count is Bigger than the list, but we cannot repeat values. Configuration is imposible to fulfil");

        var iterationList = new List<T>(value);
        var retValue = new List<T>();

        while (retValue.Count < count)
        {
            var element = iterationList.GetRandom();
            if (!canRepeat) iterationList.Remove(element);
            retValue.Add(element);
        }
        return retValue;
    }

    public static T GetLast<T>(this List<T> value)
    {
        return value[value.Count - 1];
    }

    public static T GetLast<T>(this T[] value)
    {
        return value[value.Length - 1];
    }

    public static T[] GetElements<T>(this T[] value, int[] indexes) where T : Object
    {
        T[] rValue = new T[indexes.Length];
        for (int i = 0; i < indexes.Length; i++)
        {
            rValue[i] = value[indexes[i]];
        }
        return rValue;
    }

    public static T[] GetElements<T>(this List<T> value, int[] indexes) where T : Object
    {
        return GetElements(value.ToArray(), indexes);
    }

    public static Vector2[] GetVertex(this BoxCollider2D box)
    {
        Vector2[] retValue = new Vector2[4];
        retValue[0] = new Vector2(box.bounds.min.x - box.edgeRadius, box.bounds.min.y - box.edgeRadius);
        retValue[1] = new Vector2(box.bounds.min.x - box.edgeRadius, box.bounds.max.y + box.edgeRadius);
        retValue[2] = new Vector2(box.bounds.max.x + box.edgeRadius, box.bounds.max.y + box.edgeRadius);
        retValue[3] = new Vector2(box.bounds.max.x + box.edgeRadius, box.bounds.min.y - box.edgeRadius);
        return retValue;
    }

    public static List<List<Vector2>> GetAllPaths(this PolygonCollider2D collider)
    {
        List<List<Vector2>> polygons = new List<List<Vector2>>();
        for (int i = 0; i < collider.pathCount; i++)
        {
            polygons.Add(new List<Vector2>(collider.GetPath(i)));
        }
        return polygons;
    }

    public static void SetPaths(this PolygonCollider2D collider2D, List<List<Vector2>> nPaths)
    {
        collider2D.pathCount = nPaths.Count;
        for (int i = 0; i < collider2D.pathCount; i++)
        {
            collider2D.SetPath(i, nPaths[i].ToArray());
        }
    }
    public static float SnapTo(this float value, float snap)
    {
        return Mathf.Round(value / snap) * snap;
    }
    public static Vector3 SnapTo(this Vector3 value, float snap)
    {
        Vector3 retValue = value;
        retValue.x = retValue.x.SnapTo(snap);
        retValue.y = retValue.y.SnapTo(snap);
        retValue.z = retValue.z.SnapTo(snap);
        return retValue;
    }

    public static Vector2 SnapTo(this Vector2 value, float snap)
    {
        Vector2 retValue = value;
        retValue.x = retValue.x.SnapTo(snap);
        retValue.y = retValue.y.SnapTo(snap);
        return retValue;
    }

    public static int DifferenceXtoY(this Vector2Int value)
    {
        return value.y - value.x;
    }

    public static Color ColorFromString(this string value)
    {
        return new Color(
            (value.GetHashCode() / 256f * 3) % 1,
            (value.GetHashCode() / 256f * 6) % 1,
            (value.GetHashCode() / 256f * 3) % 1);
    }

    public static float GetAsMinMaxPercentageFor(this float value, Vector2 minMaxValues, bool capAtOne)
    {
        if (value < minMaxValues.x) return 0;

        if (capAtOne)
        {
            return
                Mathf.Min(
            (value - minMaxValues.x) /
            (minMaxValues.y - minMaxValues.x), 1);
        }
        else
            return
            (value - minMaxValues.x) /
            (minMaxValues.y - minMaxValues.x);
    }


    public static IList<T> CollectComponents<T>(this IList<GameObject> value) where T : Component
    {
        List<T> l = new List<T>();
        foreach (GameObject gameObject in value)
        {
            T aux = gameObject.GetComponent<T>();
            if (aux != null) l.Add(aux);
        }
        return l;
    }

    public static int NextIndex(this int value, int baseModule)
    {
        return (value + baseModule + 1) % baseModule;
    }

    public static int PreviousIndex(this int value, int baseModule)
    {
        return (value + baseModule - 1) % baseModule;
    } 

    public static void DestroyContent<T>(this List<T> value) where T : Object
    {
        if (value.Count == 0) return;
        for (int i = value.Count-1; i >= 0; i--)
            Object.Destroy(value[i]);
    }
    public static int GetIndexOfClosest(this Transform source, List<Transform> targets)
    {
        float minDistance = float.MaxValue;
        int index = -1;
        for (int i = 0; i < targets.Count; i++)
        {
            float distance = (targets[i].position - source.position).magnitude;
            if (distance < minDistance)
            {
                minDistance = distance;
                index = i;
            }
        }
        return index;
    }
    
    public static IEnumerable<Type> GetAllSubclassTypes<T>() 
    {
        return from assembly in AppDomain.CurrentDomain.GetAssemblies()
            from type in assembly.GetTypes()
            where (type.IsSubclassOf(typeof(T)) && !type.IsAbstract)
            select type;
    }

    public static Color ColorFromString(this string value, bool useAlpha)
    {
        int aux = value.GetHashCode();
        Color c;
        c.b = ((aux) & 0xFF) / 255f;
        c.g = ((aux >> 8) & 0xFF) / 255f;
        c.r = ((aux >> 16) & 0xFF) / 255f;
        c.a = useAlpha ? ((aux >> 24) & 0xFF) / 255f : 1;
        return c;
    }
}

public static class MaterialExtensions
{
    public static void ToOpaqueMode(this Material material)
    {
        /*
        material.SetOverrideTag("RenderType", "");
        material.SetInt("_SrcBlend", (int) UnityEngine.Rendering.BlendMode.One);
        material.SetInt("_DstBlend", (int) UnityEngine.Rendering.BlendMode.Zero);
        material.SetInt("_ZWrite", 1);
        material.DisableKeyword("_ALPHATEST_ON");
        material.DisableKeyword("_ALPHABLEND_ON");
        material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
        material.renderQueue = -1;*/
        
        
        material.SetFloat("_Mode", 0);
     
        material.renderQueue = 2000;
        material.SetInt("_SrcBlend", 1);
        material.SetInt("_DstBlend", 0);
        material.SetInt("_ZWrite", 1);
        material.EnableKeyword("_ALPHATEST_ON");
        material.DisableKeyword("_ALPHABLEND_ON");
        material.EnableKeyword("_ALPHAPREMULTIPLY_ON");
    }
   
    public static void ToTransparentMode(this Material material)
    {
        /*material.SetOverrideTag("RenderType", "Transparent");
        material.SetInt("_SrcBlend", (int) UnityEngine.Rendering.BlendMode.SrcAlpha);
        material.SetInt("_DstBlend", (int) UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
        material.SetInt("_ZWrite", 0);
        material.DisableKeyword("_ALPHATEST_ON");
        material.EnableKeyword("_ALPHABLEND_ON");
        material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
        material.renderQueue = (int) UnityEngine.Rendering.RenderQueue.Transparent;*/
        
        material.SetFloat("_Mode", 3);
     
        material.renderQueue = 3000;
        material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
        material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
        material.SetInt("_ZWrite", 0);
        material.DisableKeyword("_ALPHATEST_ON");
        material.EnableKeyword("_ALPHABLEND_ON");
        material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
    }
}

public static class UnityStringExtensions
{
    public enum StringColor
    {
        Black,
        Blue,
        Brown,
        Cyan,
        Darkblue,
        Green,
        Grey,
        LightBlue,
        Lime,
        Magenta,
        Maroon,
        Navy,
        Olive,
        Orange,
        Purple,
        Red,
        Silver,
        Teal,
        White,
        Yellow
    }

    public static readonly string[] ColorCodes = new []
    {
        "#000000ff",
        "#0000ffff",
        "#a52a2aff",
        "#00ffffff",
        "#0000a0ff",
        "#008000ff",
        "#808080ff",
        "#add8e6ff",
        "#00ff00ff",
        "#ff00ffff",
        "#800000ff",
        "#000080ff",
        "#808000ff",
        "#ffa500ff",
        "#800080ff",
        "#ff0000ff",
        "#c0c0c0ff",
        "#008080ff",
        "#ffffffff",
        "#ffff00ff"
    };

    public static string ApplyColor(this string source, Color color)=> $"<color={ColorUtility.ToHtmlStringRGB(color)}>{source}</color>";

    public static string ApplyColor(this string source, StringColor stringColor)=> $"<color={ColorCodes[(int)stringColor]}>{source}</color>";
    public static string ApplyConditionalColor(this string source, StringColor trueStringColor, StringColor falseStringColor, bool value)=> ApplyColor(source, value ? trueStringColor : falseStringColor);
    
    
}