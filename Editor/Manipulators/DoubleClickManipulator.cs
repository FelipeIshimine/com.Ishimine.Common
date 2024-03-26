using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace Manipulators
{
	public class DoubleClickManipulator : MouseManipulator
	{
		public float doubleClickTimeThreshold = 0.3f; // Adjust this threshold as needed
		private float lastClickTime = 0f;
		public Action Callback { get; set; }

		public DoubleClickManipulator()
		{
			activators.Add(new ManipulatorActivationFilter { button = MouseButton.LeftMouse });
		}

		protected override void RegisterCallbacksOnTarget()
		{
			target.RegisterCallback<MouseDownEvent>(OnMouseDown);
		}

		protected override void UnregisterCallbacksFromTarget()
		{
			target.UnregisterCallback<MouseDownEvent>(OnMouseDown);
		}

		private void OnMouseDown(MouseDownEvent evt)
		{
			if (evt.clickCount == 2 && Time.time - lastClickTime < doubleClickTimeThreshold)
			{
				lastClickTime = 0f;
				evt.StopImmediatePropagation();
				DoubleClickAction();
			}
			else
			{
				lastClickTime = Time.time;
			}
		}

		private void DoubleClickAction() => Callback?.Invoke();
	}
}