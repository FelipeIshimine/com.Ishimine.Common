using System;
using System.Reflection;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;

namespace AudioPreviewer
{
	public static class AudioPreviewer
	{
		private static int? lastPlayedAudioClip = null;
    
		[OnOpenAsset]
		public static bool OnOpenAsset(int instanceId, int line)
		{
			UnityEngine.Object obj = EditorUtility.InstanceIDToObject(instanceId);

			if (obj is AudioClip clip)
			{
				if (IsPreviewClipPlaying())
				{
					StopAllPreviewClips();
					if(lastPlayedAudioClip.HasValue && lastPlayedAudioClip.Value != clip.GetInstanceID())
						PlayPreviewClip(clip);
				}
				else PlayPreviewClip(clip);
            
				lastPlayedAudioClip = clip.GetInstanceID();
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

		public static bool IsPreviewClipPlaying()
		{
			Assembly unityAssembly = typeof(AudioImporter).Assembly;
			Type audioUtil = unityAssembly.GetType("UnityEditor.AudioUtil");
			MethodInfo methodInfo = audioUtil.GetMethod(
				"IsPreviewClipPlaying",
				BindingFlags.Static | BindingFlags.Public);
			return (bool)methodInfo.Invoke(null, null);
		}
		public static void StopAllPreviewClips()
		{
			Assembly unityAssembly = typeof(AudioImporter).Assembly;
			Type audioUtil = unityAssembly.GetType("UnityEditor.AudioUtil");
			MethodInfo methodInfo = audioUtil.GetMethod(
				"StopAllPreviewClips",
				BindingFlags.Static | BindingFlags.Public);
			methodInfo.Invoke(null, null);
		}

	}
}
