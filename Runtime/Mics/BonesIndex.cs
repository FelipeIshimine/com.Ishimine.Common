using System.Collections.Generic;
using UnityEngine;

public class BonesIndex : MonoBehaviour
{
    public Transform[] bones;
     private Dictionary<string, Transform> _index = null;

    private void Start()
    {
        InitializeBones();
    }

    private void InitializeBones()
    {
        _index = new Dictionary<string, Transform>();
        foreach (Transform bone in bones)
            _index.Add(bone.name, bone);
    }

    public void FillWithChildBones(Transform t)
    {
        if (_index == null)
            InitializeBones();
        bones = t.GetComponentsInChildren<Transform>();
    }

    public Transform FindBone(string boneName)
    {
        if (_index == null || !Application.isPlaying) InitializeBones();
        return _index.TryGetValue(boneName, out Transform value) ? value : null;
    }
}
