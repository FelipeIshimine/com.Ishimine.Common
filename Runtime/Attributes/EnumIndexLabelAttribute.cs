using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnumIndexLabelAttribute : PropertyAttribute
{
	public System.Type EnumType { get; private set; }

	public EnumIndexLabelAttribute(System.Type enumType)
	{
		if (!enumType.IsEnum)
		{
			Debug.LogError("EnumIndexLabelAttribute must be used with an Enum type.");
			return;
		}

		EnumType = enumType;
	}
}