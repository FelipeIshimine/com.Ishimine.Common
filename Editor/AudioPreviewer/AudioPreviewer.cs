using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;

public static class AudioPreviewer
{
    [OnOpenAsset]
    public static bool OnOpenAsset(int instanceId, int line)
    {
        UnityEngine.Object obj = EditorUtility.InstanceIDToObject(instanceId);

        if (obj is AudioClip clip)
        {
            PlayPreviewClip(clip);
            return true;
        }
        return false;
    }


    public static void PlayPreviewClip(AudioClip audioClip)
    {
        Assembly unityAssembly = typeof(AudioImporter).Assembly;

        Type audioUtil = unityAssembly.GetType("UnityEditor.AudioUtil");

        MethodInfo methodInfo = audioUtil.GetMethod(
            "PlayPreviewClip",
            BindingFlags.Static | BindingFlags.Public,
            null,
            new System.Type[]
            {
                typeof(AudioClip),
                typeof(Int32),
                typeof(Boolean)
            },
            null);
        methodInfo.Invoke(null, new object[]
        {
            audioClip, 0, false
        });
    }
    
}
