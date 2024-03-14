using System;
using System.Collections.Generic;
using UnityEditor.IMGUI.Controls;

public class QuickAdvancedDropdown : AdvancedDropdown
{
	private readonly string title;
	private readonly string[] labels;
	private readonly Action<int> callback;
	private readonly char splitCharacter;

	private Dictionary<string, AdvancedDropdownItem> pathToDropdownItems =
		new Dictionary<string, AdvancedDropdownItem>();
	
	public QuickAdvancedDropdown(string title, string[] labels, Action<int> callback, char splitCharacter = '/') : base(new AdvancedDropdownState())
	{
		this.title = title;
		this.labels = labels;
		this.callback = callback;
		this.splitCharacter = splitCharacter;
	}

	protected override AdvancedDropdownItem BuildRoot()
	{
		AdvancedDropdownItem root = new AdvancedDropdownItem(title);
		
		for (int i = 0; i < labels.Length; i++)
		{
			var split = labels[i].Split(splitCharacter);

			var previous = root;
			for (var j = 0; j < split.Length; j++)
			{
				var s = split[j];
				if (!pathToDropdownItems.TryGetValue(s, out var item) || j == split.Length-1)
				{
					item = pathToDropdownItems[s] = new AdvancedDropdownItem(s)
					{
						id = i
					};
					previous.AddChild(item);
				}
				previous = item;
				//item = root.AddChild(new AdvancedDropdownItem(s));
			}

			/*root.AddChild(new AdvancedDropdownItem(labels[i])
			{
				id = i
			});*/
		}
		return root;
	}

	protected override void ItemSelected(AdvancedDropdownItem item) => callback.Invoke(item.id);
}