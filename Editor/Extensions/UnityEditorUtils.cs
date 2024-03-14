using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

namespace Extensions
{
	public static class UnityEditorUtils
	{
		public static List<T> FindAssets<T>()
		{
			List<T> assets = new List<T>();
			string[] guids = AssetDatabase.FindAssets($"t:{typeof(T)}");
			for (int i = 0; i < guids.Length; i++)
			{
				string assetPath = AssetDatabase.GUIDToAssetPath(guids[i]);
				Object[] found = AssetDatabase.LoadAllAssetsAtPath(assetPath);

				for (int index = 0; index < found.Length; index++)
					if (found[index] is T item && !assets.Contains(item))
						assets.Add(item);
			}
			return assets;
		}
    
		public static Scene[] GetAllLoadedScenes()
		{
			List<Scene> scenes = new List<Scene>();
			for (int i = 0; i < SceneManager.sceneCount; i++)
				scenes.Add(SceneManager.GetSceneAt(i));
			return scenes.ToArray();
		}
    
		/// <summary>
		/// Empieza automaticamente desde Assets
		/// </summary>
		/// <param name="folders"></param>
		public static string CreateFoldersRecursive(params string[] folders)
		{
			string currentPath = "Assets/";
			for (int i = 0; i < folders.Length; i++)
			{
				Debug.Log(currentPath);
				string parentFolder = currentPath.Remove(currentPath.Length - 1, 1);
				string subfolder = folders[i];
				if(!AssetDatabase.IsValidFolder(currentPath + subfolder))
					AssetDatabase.CreateFolder(parentFolder, subfolder);
				currentPath += $"{folders[i]}/";
			}
			AssetDatabase.Refresh();
			return currentPath;
		}
        
		public static T AssetFromGuid<T>(string assetGuid) where T : Object
		{
			string path = AssetDatabase.GUIDToAssetPath(assetGuid);
			return  AssetDatabase.LoadAssetAtPath<T>(path);
		}
    
		public static T FindAssetByType<T>() where T : UnityEngine.Object => FindAssetsByType<T>()[0];
    
		public static List<T> FindAssetsByType<T>()
		{
			var objectsFound =  FindAssetsByType(typeof(T));
			List<T> values = new List<T>();
    
			foreach (Object obj in objectsFound)
				values.Add((T) Convert.ChangeType(obj, typeof(T)));
    
			return values;
		}
        
		public static List<Object> FindAssetsByType(Type type)
		{
			List<Object> assets = new List<Object>();
			string[] guids = AssetDatabase.FindAssets($"t:{type}");
			for (int i = 0; i < guids.Length; i++)
			{
				string assetPath = AssetDatabase.GUIDToAssetPath(guids[i]);
				UnityEngine.Object[] found = AssetDatabase.LoadAllAssetsAtPath(assetPath);
    
				for (int index = 0; index < found.Length; index++)
					if (found[index].GetType() == type && !assets.Contains(found[index]))
						assets.Add(found[index]);
			}
			return assets;
		}
    
		public static T[] LoadFilesInFolder<T>(string folderPath, string pattern, SearchOption searchOption) where T : Object
		{
			string[] files = Directory.GetFiles(folderPath, pattern, searchOption);
			T[] results = new T[files.Length];
			for (var index = 0; index < files.Length; index++)
			{
				string file = files[index];
				string assetPath = "Assets" + file.Replace(Application.dataPath, "").Replace('\\', '/');
				results[index] = AssetDatabase.LoadAssetAtPath<T>(assetPath);
			}
			return results;
		}
        
        
		public static void DrawWireCapsule(Vector3 _pos, Quaternion _rot, float _radius, float _height, Color _color = default(Color))
		{
			if (_color != default(Color))
				Handles.color = _color;
			Matrix4x4 angleMatrix = Matrix4x4.TRS(_pos, _rot, Handles.matrix.lossyScale);
			using (new Handles.DrawingScope(angleMatrix))
			{
				var pointOffset = (_height - (_radius * 2)) / 2;
     
				//draw sideways
				Handles.DrawWireArc(Vector3.up * pointOffset, Vector3.left, Vector3.back, -180, _radius);
				Handles.DrawLine(new Vector3(0, pointOffset, -_radius), new Vector3(0, -pointOffset, -_radius));
				Handles.DrawLine(new Vector3(0, pointOffset, _radius), new Vector3(0, -pointOffset, _radius));
				Handles.DrawWireArc(Vector3.down * pointOffset, Vector3.left, Vector3.back, 180, _radius);
				//draw frontways
				Handles.DrawWireArc(Vector3.up * pointOffset, Vector3.back, Vector3.left, 180, _radius);
				Handles.DrawLine(new Vector3(-_radius, pointOffset, 0), new Vector3(-_radius, -pointOffset, 0));
				Handles.DrawLine(new Vector3(_radius, pointOffset, 0), new Vector3(_radius, -pointOffset, 0));
				Handles.DrawWireArc(Vector3.down * pointOffset, Vector3.back, Vector3.left, -180, _radius);
				//draw center
				Handles.DrawWireDisc(Vector3.up * pointOffset, Vector3.up, _radius);
				Handles.DrawWireDisc(Vector3.down * pointOffset, Vector3.up, _radius);
     
			}
		}
	}
}