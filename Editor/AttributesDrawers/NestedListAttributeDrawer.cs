﻿using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEngine;

namespace AttributesDrawers
{
	[DrawerPriority(DrawerPriorityLevel.SuperPriority)]
	public class NestedListAttributeDrawer<TList, T> : OdinAttributeDrawer<NestedListAttribute, TList> where TList : List<T> where T : ScriptableObject
	{
		UnityEngine.Object Parent => (UnityEngine.Object)Property.Parent.ValueEntry.WeakSmartValue;

		protected override void Initialize()
		{
			Attribute.Type = typeof(T);
			base.Initialize();
		}
		protected override void DrawPropertyLayout(GUIContent label)
		{
			CallNextDrawer(label);
			if(Attribute.objectsToRemove.Count > 0)
			{
				UnityEngine.Object objectToRemove = Attribute.objectsToRemove[0];
				Attribute.objectsToRemove.Remove(objectToRemove);
				if (ValueEntry.SmartValue.Contains(objectToRemove))
				{
					AssetDatabase.Refresh();
					ValueEntry.SmartValue.Remove((T)objectToRemove);
					UnityEngine.Object.DestroyImmediate(objectToRemove, true);
					if (!Application.isPlaying)
					{
						AssetDatabase.ForceReserializeAssets(new[] {AssetDatabase.GetAssetPath(Parent)});
						AssetDatabase.SaveAssets();
						AssetDatabase.Refresh();
					}
				}
			}
			if(Attribute.objectsToCreate.Count > 0)
			{
				ScriptableObject objectToCreate = Attribute.objectsToCreate[0];
				Attribute.objectsToCreate.Remove(objectToCreate);
				objectToCreate.name = "_" + objectToCreate.GetType().Name;
				AssetDatabase.AddObjectToAsset(objectToCreate, Parent);
				AssetDatabase.SaveAssets();
				AssetDatabase.Refresh();
			}
		}
	}
}