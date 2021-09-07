using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugActivate : MonoBehaviour
{
    public void Awake()=> Debug.Log($"||| DebugActivate: AWAKE {this}");
    public void Start()=> Debug.Log($"||| DebugActivate: START {this}");
    public void OnEnable()=> Debug.Log($"||| DebugActivate: ON ENABLE {this}");
    public void OnDisable() => Debug.Log($"||| DebugActivate: ON DISABLE {this}");

    public void OnDestroy() => Debug.Log($"||| DebugActivate: OnDestroy {this}");
}
