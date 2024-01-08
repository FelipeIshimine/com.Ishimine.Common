using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using Screen = UnityEngine.Device.Screen;

namespace Legion.Editor
{
	public static class SceneSelectionQuickAccess
	{
		private static Vector2 lastMousePosition;

		[MenuItem("QuickAccess/Scene ^#Q")]
		public static void ShowSceneMenu()
		{
			SceneView.duringSceneGui += SceneGUI;
		}

		private static void SceneGUI(SceneView obj)
		{
			SceneQuickAccessDropdown dropdown = new SceneQuickAccessDropdown(new AdvancedDropdownState());
			lastMousePosition = Event.current.mousePosition;
			//Debug.Log(lastMousePosition);
			dropdown.Show(new Rect(lastMousePosition, Vector2.zero));
			SceneView.duringSceneGui -= SceneGUI;
		}
	}
}

class SceneQuickAccessDropdown : AdvancedDropdown
{
	private bool useFullPath = false;
	private string[] sceneGuids;
	private string[] paths;
	private string[] names;
	private Dictionary<string, AdvancedDropdownItem> items = new Dictionary<string, AdvancedDropdownItem>();

	public SceneQuickAccessDropdown(AdvancedDropdownState state) : base(state)
	{

	}

	protected override AdvancedDropdownItem BuildRoot()
	{
		var root = new AdvancedDropdownItem("Scenes");
		sceneGuids = AssetDatabase.FindAssets("t:scene", new[] { "Assets/" });
		paths = new string[sceneGuids.Length];
		names = new string[sceneGuids.Length];
		for (int i = 0; i < sceneGuids.Length; i++)
		{
			var path = AssetDatabase.GUIDToAssetPath(sceneGuids[i]);
			paths[i] = path;
			names[i] = Path.GetFileNameWithoutExtension(path);
		}

		if (useFullPath)
		{
			for (var index = 0; index < paths.Length; index++)
			{
				var split = paths[index].Split('/');

				AdvancedDropdownItem parent = root;
				for (int i = 0; i < split.Length - 1; i++)
				{
					var key = split[i];
					if (!items.TryGetValue(key, out var item))
					{
						items[key] = item = new AdvancedDropdownItem(key);
						parent.AddChild(item);
					}
					parent = item;
				}

				var indexItem = new SceneItem(paths[index], names[index]);
				parent.AddChild(indexItem);
			}	
		}
		else
		{
			for (var index = 0; index < names.Length; index++)
			{
				var name = names[index];
				root.AddChild(new SceneItem(paths[index], names[index]));
			}
		}
		return root;
	}

	protected override void ItemSelected(AdvancedDropdownItem item)
	{
		if (item is SceneItem sceneItem)
		{
			OpenScene(EditorSceneManager.GetActiveScene(), sceneItem.path);
		}
	}

	static void OpenScene(Scene currentScene, string path)
	{
		if (currentScene.isDirty)
		{
			if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
			{
				EditorSceneManager.OpenScene(path);
			}
		}
		else
		{
			EditorSceneManager.OpenScene(path);
		}
	}

	public class SceneItem : AdvancedDropdownItem
	{
		public readonly string path;

		public SceneItem(string path, string name) : base(name)
		{
			this.path = path;
		}
	}
}
