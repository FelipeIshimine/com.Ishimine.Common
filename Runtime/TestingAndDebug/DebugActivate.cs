using UnityEngine;

public class DebugActivate : MonoBehaviour
{
    public void Awake()=> Debug.Log($"||| DebugActivate: AWAKE {transform.GetHierarchyAsString(true)}");
    public void Start()=> Debug.Log($"||| DebugActivate: START {transform.GetHierarchyAsString(true)}");
    public void OnEnable()=> Debug.Log($"||| DebugActivate: ON ENABLE {transform.GetHierarchyAsString(true)}");
    public void OnDisable() => Debug.Log($"||| DebugActivate: ON DISABLE {transform.GetHierarchyAsString(true)}");
    public void OnDestroy() => Debug.Log($"||| DebugActivate: OnDestroy {transform.GetHierarchyAsString(true)}");
}
