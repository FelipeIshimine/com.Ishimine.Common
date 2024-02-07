using System;
using UnityEditor.IMGUI.Controls;

public class QuickAdvancedDropdown : AdvancedDropdown
{
	private readonly string[] labels;
	private readonly Action<int> callback;

	public QuickAdvancedDropdown(string[] labels, Action<int> callback) : base(new AdvancedDropdownState())
	{
		this.labels = labels;
		this.callback = callback;
	}

	protected override AdvancedDropdownItem BuildRoot()
	{
		AdvancedDropdownItem root = new AdvancedDropdownItem("Scenes");

		for (int i = 0; i < labels.Length; i++)
		{
			root.AddChild(new AdvancedDropdownItem(labels[i])
			{
				id = i
			});
		}
		return root;
	}

	protected override void ItemSelected(AdvancedDropdownItem item) => callback.Invoke(item.id);
}