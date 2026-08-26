using System;
using System.Reflection;
using Plugins.RequiredField.Runtime;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Plugins.RequiredField.Editor
{
	[CustomPropertyDrawer(typeof(Object), true)]
	public class RequiredDrawer : PropertyDrawer
	{
		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			int indent = EditorGUI.indentLevel;
			EditorGUI.indentLevel = 0;

			using (EditorGUI.ChangeCheckScope beginChangeCheck = new())
			{
				position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);
				EditorGUI.PropertyField(position, property, GUIContent.none, true);

				if (beginChangeCheck.changed)
				{
					property.serializedObject.ApplyModifiedProperties();
				}
			}

			Object value = property.objectReferenceValue;
			bool containsFieldAttribute = fieldInfo.GetCustomAttribute<CanBeNull>() != null;
			bool containsClassAttribute = property.serializedObject.targetObject.GetType().GetCustomAttribute<CanBeNull>() != null;
			bool isExcluded = IsExcludedType(property.serializedObject.targetObject.GetType());

			if (value == false && containsFieldAttribute == false && containsClassAttribute == false && isExcluded == false)
			{
				bool containsKey = RequiredData.Data.TryGetKey(fieldInfo.ToString());

				if (RequiredData.Data.ShowBorder && containsKey == false)
				{
					DrawBorderRect(property, label, position, RequiredData.Data.BorderColor, 1f);
				}

				if (RequiredData.Data.ShowIcon)
				{
					position.x -= 22f;

					if (GUI.Button(position,
						    new GUIContent(EditorGUIUtility.IconContent(containsKey == false ? RequiredData.Data.GetIconType() : "Refresh")),
						    GUIStyle.none))
					{
						if (containsKey == false)
						{
							RequiredData.Data.AddKey(fieldInfo.ToString());
						}
						else
						{
							RequiredData.Data.RemoveKey(fieldInfo.ToString());
						}
					}
				}
			}

			EditorGUI.indentLevel = indent;
		}

		private bool IsExcludedType(Type parentType)
		{
			return Array.Find(RequiredExcludedTypes.ExcludedTypes, excludedType => excludedType == parentType) != null;
		}

		private void DrawBorderRect(SerializedProperty property, GUIContent label, Rect area, Color color,
			float borderWidth)
		{
			//------------------------------------------------
			float x1 = area.x;
			float y1 = area.y;
			float x2 = area.width;
			float y2 = borderWidth;

			Rect lineRect = new(x1, y1, x2, y2);

			EditorGUI.DrawRect(lineRect, color);

			//------------------------------------------------
			x1 = area.x + area.width - 1f;
			y1 = area.y;
			x2 = borderWidth;
			y2 = area.height;

			lineRect = new Rect(x1, y1, x2, y2);

			EditorGUI.DrawRect(lineRect, color);

			//------------------------------------------------
			x1 = area.x;
			y1 = area.y;
			x2 = borderWidth;
			y2 = area.height;

			lineRect = new Rect(x1, y1, x2, y2);

			EditorGUI.DrawRect(lineRect, color);

			//------------------------------------------------
			x1 = area.x;
			y1 = area.y + GetPropertyHeight(property, label) - 1f;
			x2 = area.width;
			y2 = borderWidth;

			lineRect = new Rect(x1, y1, x2, y2);

			EditorGUI.DrawRect(lineRect, color);
		}
	}
}