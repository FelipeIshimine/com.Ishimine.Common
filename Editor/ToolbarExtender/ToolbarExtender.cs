using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine.UIElements;
using UnityToolbarExtender;

namespace Common.Editor.ToolbarExtender
{
	[InitializeOnLoad]
	public static class ToolbarExtender
	{
		public static readonly List<VisualElement> LeftToolbarGUI = new();
		public static readonly List<VisualElement> RightToolbarGUI = new();

		static ToolbarExtender()
		{
			Type toolbarType = typeof(UnityEditor.Editor).Assembly.GetType("UnityEditor.Toolbar");
			
			string fieldName = "k_ToolCount";
			FieldInfo toolIcons = toolbarType.GetField(fieldName,
				BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
			
			ToolbarCallback.OnToolbarGUI = CreateGui;
			ToolbarCallback.OnToolbarGUILeft = BuildLeftVisualElement;
			ToolbarCallback.OnToolbarGUIRight = BuildRightVisualElement;
		}

		static VisualElement CreateGui()
		{
			VisualElement container = new VisualElement();
			return container;
		}
		
		public static VisualElement BuildLeftVisualElement()
		{
			VisualElement container = new VisualElement()
			{
				name = "Left Toolbar Extension",
				style =
				{
					flexDirection = FlexDirection.RowReverse,
					alignItems = Align.Center,
					alignContent = Align.Center,
					flexGrow = 1
				}
			};
            
			LeftToolbarGUI.Sort(CompareElements);
			foreach (var visualElement in LeftToolbarGUI)
			{
				container.Add(visualElement);
			}
			return container;
		}

		private static int CompareElements(VisualElement x, VisualElement y)
		{
			// Check if x.userData is an int, if not, treat its priority as 0
			int xPriority = x.userData is int xValue ? xValue : 0;
    
			// Check if y.userData is an int, if not, treat its priority as 0
			int yPriority = y.userData is int yValue ? yValue : 0;

			// Compare the priorities
			return xPriority.CompareTo(yPriority);
		}


		public static VisualElement BuildRightVisualElement()
		{
			VisualElement container = new VisualElement()
			{
				style =
				{
					flexDirection = FlexDirection.Row ,
					flexGrow = 1
				}
			};
			RightToolbarGUI.Sort(CompareElements);
			foreach (var visualElement in RightToolbarGUI)
			{
				container.Add(visualElement);
			}
			return container;
		}
	}
}
