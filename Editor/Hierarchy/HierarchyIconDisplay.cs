using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

	[InitializeOnLoad]
	public static class HierarchyIconDisplay
	{
		private static bool _hierarchyHasFocus = false;

		private static EditorWindow _hierarchyEditorWindow;

		static HierarchyIconDisplay()
		{
			if(!EditorApplication.isPlayingOrWillChangePlaymode)
			{
				EditorApplication.hierarchyWindowItemOnGUI += OnHierarchyWindowItemOnGUI;
				EditorApplication.update += OnEditorUpdate;
			}
		}
		
		
		private static void OnEditorUpdate()
		{
			bool HasOpenInstances(Type t) 
			{
				UnityEngine.Object[] objectsOfTypeAll = Resources.FindObjectsOfTypeAll(t);
				return objectsOfTypeAll != null && objectsOfTypeAll.Length != 0;
			}
			
			if (_hierarchyEditorWindow == null)
			{
				var type = System.Type.GetType("UnityEditor.SceneHierarchyWindow,UnityEditor");
				if (HasOpenInstances(type))
				{
					_hierarchyEditorWindow = EditorWindow.GetWindow(System.Type.GetType("UnityEditor.SceneHierarchyWindow,UnityEditor"));
					_hierarchyHasFocus = EditorWindow.focusedWindow != null && EditorWindow.focusedWindow == _hierarchyEditorWindow;
				}
			}
			else
			{
				_hierarchyHasFocus = EditorWindow.focusedWindow != null &&
					EditorWindow.focusedWindow == _hierarchyEditorWindow;
			}
		}

		private static void OnHierarchyWindowItemOnGUI(int instanceID, Rect selectionRect)
		{
			GameObject obj = EditorUtility.InstanceIDToObject(instanceID) as GameObject;

			if (obj == null) return;

			if (PrefabUtility.GetCorrespondingObjectFromOriginalSource(obj) != null) return;

			Component[] components = obj.GetComponents<Component>();
			if (components == null || components.Length == 0) return;

			Component component = components.Length > 1 ? components[1] : components[0];

			if (component == null)
			{
				return;
			}

			Type type = component.GetType();

			GUIContent content = EditorGUIUtility.ObjectContent(component, type);
			content.text = null;
			content.tooltip = type.Name;

			if (content.image == null) return;

			bool isSelected = Selection.instanceIDs.Contains(instanceID);
			bool isHovering = selectionRect.Contains(Event.current.mousePosition);


			Color color = UnityEditorBackgroundColor.Get(isSelected, isHovering, _hierarchyHasFocus);
			Rect backgroundRect = selectionRect;
			backgroundRect.width = 18.5f;
			EditorGUI.DrawRect(backgroundRect, color);
			EditorGUI.LabelField(selectionRect, content);


		}
	}


	public static class UnityEditorBackgroundColor
	{
		private static readonly Color k_defaultColor = new Color(0.7843f, 0.7843f, 07843f);
		private static readonly Color k_defaultProColor = new Color(0.2196f, 0.2196f, 0.2196f);

		private static readonly Color k_selectedColor = new Color(0.22745f, .447f, .6902f);
		private static readonly Color k_selectedProColor = new Color(.1725f, .3647f, .5294f);

		private static readonly Color k_selectedUnFocusedColor = new Color(.68f, .68f, .68f);
		private static readonly Color k_selectedUnFocusedProColor = new Color(.3f, .3f, .3f);

		private static readonly Color k_hoveredColor = new Color(.698f, .698f, .698f);
		private static readonly Color k_hoveredProColor = new Color(.2706f, .2706f, .2706f);

		public static Color Get(bool isSelected, bool isHovered, bool isWindowFocused)
		{
			bool isProSkin = EditorGUIUtility.isProSkin;
			if (isSelected)
			{
				if (isWindowFocused)
				{
					return isProSkin ? k_selectedProColor : k_selectedColor;
				}
				else
				{
					return isProSkin ? k_selectedUnFocusedProColor : k_selectedUnFocusedColor;
				}
			}
			else if (isHovered)
			{
				return isProSkin ? k_hoveredProColor : k_hoveredColor;
			}
			else
				return isProSkin ? k_defaultProColor : k_defaultColor;
		}
	}

