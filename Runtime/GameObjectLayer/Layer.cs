using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine;
 
[System.Serializable]
public class Layer
{
    [SerializeField]
    private int m_LayerIndex = 0;
    public int LayerIndex => m_LayerIndex;

    public void Set(int _layerIndex)
    {
        if (_layerIndex > 0 && _layerIndex < 32)
        {
            m_LayerIndex = _layerIndex;
        }
    }
 
    public int Mask =>  1 << m_LayerIndex;

    //public static implicit operator LayerMask(Layer layer) => layer.Mask;
    //public static implicit operator int(Layer layer) => layer.Mask;
}

