using System;
using System.IO;
using UnityEditor;
using UnityEditor.Overlays;
using UnityEditor.SceneManagement;
using UnityEditor.Toolbars;
using UnityEngine;
using Scene = UnityEngine.SceneManagement.Scene;


namespace Legion.Editor
{
	[Overlay(typeof(SceneView), "Scene Selection")]
    public class SceneSelectionOverlay : ToolbarOverlay
    {
	    public const string k_icon = "Assets/Editor/Icons/UnityIcon.png";

	    SceneSelectionOverlay() : base(SceneDropdownToggle.k_id)
	    {
		    
	    }

	    [EditorToolbarElement(k_id, typeof(SceneView))]
	    class SceneDropdownToggle : EditorToolbarDropdownToggle, IAccessContainerWindow
	    {
		    public const string k_id = "SceneSelectionOverlay/SceneDropdownToggle";
		    
		    public EditorWindow containerWindow { get; set; }

		    SceneDropdownToggle()
		    {
			    text = "";
			    tooltip = "Select a scene to load";
			    icon = AssetDatabase.LoadAssetAtPath<Texture2D>(SceneSelectionOverlay.k_icon);
			    
			    dropdownClicked += ShowSceneMenu;
		    }

		    private void ShowSceneMenu()
		    {
			    GenericMenu menu = new GenericMenu();
			    string[] sceneGuids = AssetDatabase.FindAssets("t:scene", new []{"Assets/"});
			    var currentScene = EditorSceneManager.GetActiveScene();
			    for (int i = 0; i < sceneGuids.Length; i++)
			    {
				    var path = AssetDatabase.GUIDToAssetPath(sceneGuids[i]);
				    string name = Path.GetFileNameWithoutExtension(path);
				    menu.AddItem(new GUIContent(name),String.CompareOrdinal(currentScene.name, name)==0,()=>OpenScene(currentScene,path));
			    }
			    menu.ShowAsContext();
		    }

		    void OpenScene(Scene currentScene, string path)
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
	    }
    }
}
