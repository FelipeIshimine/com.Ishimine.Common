using System;
using UnityEngine;
using UnityEditor;
using System.Reflection;

#if UNITY_2019_1_OR_NEWER
using UnityEngine.UIElements;
#else
using UnityEngine.Experimental.UIElements;
#endif

namespace UnityToolbarExtender
{
	public static class ToolbarCallback
	{
		static Type m_toolbarType = typeof(Editor).Assembly.GetType("UnityEditor.Toolbar");
		static ScriptableObject m_currentToolbar;

		/// <summary>
		/// Callback for toolbar OnGUI method.
		/// </summary>
		public static Func<VisualElement> OnToolbarGUI;
		public static Func<VisualElement> OnToolbarGUILeft;
		public static Func<VisualElement> OnToolbarGUIRight;
		
		static ToolbarCallback()
		{
			EditorApplication.update -= OnUpdate;
			EditorApplication.update += OnUpdate;
		}

		static void OnUpdate()
		{
			// Relying on the fact that toolbar is ScriptableObject and gets deleted when layout changes
			if (m_currentToolbar == null)
			{
				// Find toolbar
				var toolbars = Resources.FindObjectsOfTypeAll(m_toolbarType);
				m_currentToolbar = toolbars.Length > 0 ? (ScriptableObject) toolbars[0] : null;
				if (m_currentToolbar != null)
				{ 
#if UNITY_2021_1_OR_NEWER
					var root = m_currentToolbar.GetType().GetField("m_Root", BindingFlags.NonPublic | BindingFlags.Instance);
					var rawRoot = root.GetValue(m_currentToolbar);
					var mRoot = rawRoot as VisualElement;

					mRoot.Q("ToolbarZoneLeftAlign").Add(OnToolbarGUILeft.Invoke());
					mRoot.Q("ToolbarZoneRightAlign").Add(OnToolbarGUIRight.Invoke());
#endif
				}
			}
		}
	}
}
