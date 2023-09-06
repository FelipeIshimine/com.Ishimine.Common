using UnityEngine;

public class PrefabDropdownAttribute : PropertyAttribute
{
	public readonly bool showFullPath;

	public PrefabDropdownAttribute(bool showFullPath = false)
	{
		this.showFullPath = showFullPath;
	}
}