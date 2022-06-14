using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugOnly : MonoBehaviour
{
    public void Awake()
    {
        if(!Debug.isDebugBuild)
            Destroy(gameObject);
    }
}