using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace Manipulators
{
	public class DoubleClickManipulator : MouseManipulator
	{
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
			if (evt.clickCount == 2)
			{
				evt.StopImmediatePropagation();
				DoubleClickAction();
			}
		}

		private void DoubleClickAction() => Callback?.Invoke();
	}
}