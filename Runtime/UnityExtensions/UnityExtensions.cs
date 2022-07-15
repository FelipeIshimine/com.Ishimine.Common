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

    public static Vector2 AsVectorXY(this float degree) => RadianToVector2(degree * Mathf.Deg2Rad);
    
    public static Vector3 AsVectorXZ(this float degree) => RadianToVector3(degree * Mathf.Deg2Rad);

    
    public static Vector2 RadianToVector2(float radian)
    {
        return new Vector2(Mathf.Cos(radian), Mathf.Sin(radian));
    }

    public static Vector3 RadianToVector3(float radian)
    {
        return new Vector3(Mathf.Cos(radian), 0,Mathf.Sin(radian));
    }

    public static void Shuffle<T>(this IList<T> list)  
    {  
        int n = list.Count;  
        while (n > 1) 
        {  
            n--;
            int k = Random.Range(0, n + 1);  
            (list[k], list[n]) = (list[n], list[k]);
        }  
    }

    public static Vector2 ClampPositionToView(this Camera camera, Vector2 value)
    {
        float hSize = GetHorizontalSize(camera);
        return new Vector2(Mathf.Clamp(value.x, -hSize, hSize), Mathf.Clamp(value.y, -camera.orthographicSize, camera.orthographicSize)) + (Vector2)camera.transform.position;
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
  
    public static Color ColorFromString(this string value)
    {
        return new Color(
            (value.GetHashCode() / 256f * 3) % 1,
            (value.GetHashCode() / 256f * 6) % 1,
            (value.GetHashCode() / 256f * 3) % 1);
    }

    public static float GetAsPercentageBetween(this float value, float floor, float ceil, bool clamp)
    {
        if (clamp)
        {
            if (value > ceil) return 1;
            if (value < floor) return 0;
        }
        return (value - floor) / (ceil - floor);
    }

    public static float GetAsPercentageBetween(this float value, Vector2 range, bool clamp) => GetAsPercentageBetween(value, range.x, range.y, clamp);
    public static float GetAsPercentageBetween(this float value, Vector2Int range, bool clamp) => GetAsPercentageBetween(value, range.x, range.y, clamp);

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