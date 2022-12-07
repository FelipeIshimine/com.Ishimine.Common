using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using UnityEditor;
using UnityEngine;

public static class ScreenShot
{
    private static string FileName => $"Screenshots_{DateTime.Now.ToString(CultureInfo.InvariantCulture).Replace("/","_").Replace(":","-")}.png";
    
    [MenuItem("Tools/Take Screenshot")]
    public static void TakeScreenshot()
    {
        //DirectoryInfo screenshotDirectory = Directory.CreateDirectory(DirectoryName);
        string fullPath = Path.Combine( Application.dataPath, FileName);
        ScreenCapture.CaptureScreenshot(fullPath);
        Debug.Log(fullPath);
        AssetDatabase.SaveAssets();
    }
}
