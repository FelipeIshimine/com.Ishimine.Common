﻿using UnityEditor;
using UnityEngine;

namespace Extensions
{
	public static class SceneViewExtensions
	{
		// Looks at the pivot point from the specified *distance* and rotation.
		public static void LookAt(this SceneView sceneView, in Vector3 pivot, float distance, in Quaternion rotation ) =>
			sceneView.LookAt( pivot, rotation,
				GetSizeFromDistance( sceneView, distance ) );
 
		// From SceneView.cs / GetPerspectiveCameraDistance().
		static float GetSizeFromDistance( SceneView sceneView, float distance ) =>
			distance * Mathf.Sin( sceneView.camera.fieldOfView * 0.5f * Mathf.Deg2Rad );
	}
}