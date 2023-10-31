using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityRandom = UnityEngine.Random;

	[CustomPropertyDrawer(typeof(NavMeshMaskAttribute))]
	public class NavMeshMaskDrawer : PropertyDrawer
	{
		public override void OnGUI(Rect position, SerializedProperty serializedProperty, GUIContent label)
		{
			var width = position.width;
			position.width = EditorGUIUtility.labelWidth;
			EditorGUI.PrefixLabel(position, label);
         
			var areaNames  = GameObjectUtility.GetNavMeshAreaNames();
			var mask = serializedProperty.intValue;
			position.x += EditorGUIUtility.labelWidth;
			position.width = width - EditorGUIUtility.labelWidth;
 
			EditorGUI.BeginChangeCheck();
			mask = EditorGUI.MaskField(position, mask, areaNames);
			if (EditorGUI.EndChangeCheck())
				serializedProperty.intValue = mask;
		}
	}
