using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEditor.UIElements;
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


			AddMethodsByAttribute(UnityToolbarButtonAttribute.SideMode.Left,LeftToolbarGUI);
			LeftToolbarGUI.Sort(CompareElements);

			foreach (var visualElement in LeftToolbarGUI)
			{
				container.Add(visualElement);
			}
			
			
			
			return container;
		}

		private static void AddMethodsByAttribute(UnityToolbarButtonAttribute.SideMode side,
		                                          List<VisualElement> list)
		{
			foreach (MethodInfo methodInfo in TypeCache.GetMethodsWithAttribute<UnityToolbarButtonAttribute>())
			{
				if (!methodInfo.IsStatic)
				{
					continue;
				}
				var attribute = methodInfo.GetCustomAttribute<UnityToolbarButtonAttribute>();

				if (attribute.Side == side)
				{
					ToolbarButton button = new ToolbarButton()
					{
						text = attribute.Label
					};
					button.userData = attribute.Priority;
					button.clicked += ()=> methodInfo.Invoke(null,null);
					list.Add(button);
				}
			}
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
            
			AddMethodsByAttribute(UnityToolbarButtonAttribute.SideMode.Right,RightToolbarGUI);
			RightToolbarGUI.Sort(CompareElements);
			foreach (var visualElement in RightToolbarGUI)
			{
				container.Add(visualElement);
			}
			return container;
		}
	}


	[AttributeUsage(AttributeTargets.Method)]
	public class UnityToolbarButtonAttribute : Attribute
	{
		public readonly int Priority;
		public readonly SideMode Side;
		public readonly string Label;

		public UnityToolbarButtonAttribute(string label, SideMode side, int priority=0)
		{
			Priority = priority;
			Side = side;
			Label = label;
		}

		public enum SideMode
		{
			Left,
			Right
		}

	

	}
}
