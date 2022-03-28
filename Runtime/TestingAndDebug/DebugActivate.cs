using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugActivate : MonoBehaviour
{
    public void Awake()=> Debug.Log($"||| DebugActivate: AWAKE {transform.GetHierarchyAsString()}");
    public void Start()=> Debug.Log($"||| DebugActivate: START {transform.GetHierarchyAsString()}");
    public void OnEnable()=> Debug.Log($"||| DebugActivate: ON ENABLE {transform.GetHierarchyAsString()}");
    public void OnDisable() => Debug.Log($"||| DebugActivate: ON DISABLE {transform.GetHierarchyAsString()}");
    public void OnDestroy() => Debug.Log($"||| DebugActivate: OnDestroy {transform.GetHierarchyAsString()}");
}
