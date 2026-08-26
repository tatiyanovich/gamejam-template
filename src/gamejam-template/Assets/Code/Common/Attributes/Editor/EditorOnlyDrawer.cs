using Code.Common.Attributes;
using UnityEditor;
using UnityEngine;

namespace Code.Common.Attributes.Editor
{
	[CustomPropertyDrawer(typeof(EditorOnlyAttribute))]
	public class EditorOnlyDrawer : PropertyDrawer
	{
		private const float LabelWidth = 62f;
		private const float LabelHeight = 14f;
		private const float Spacing = 2f;

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			Rect fieldRect = position;
			fieldRect.width -= LabelWidth + Spacing;

			EditorGUI.PropertyField(fieldRect, property, label, true);

			Rect labelRect = new Rect(
				fieldRect.xMax + Spacing,
				position.y + (EditorGUIUtility.singleLineHeight - LabelHeight) * 0.5f,
				LabelWidth,
				LabelHeight);

			Color previousColor = GUI.color;
			GUI.color = new Color(0.6f, 0.6f, 0.6f, 0.9f);

			GUIStyle style = new GUIStyle(EditorStyles.miniLabel)
			{
				alignment = TextAnchor.MiddleCenter
			};

			EditorGUI.LabelField(labelRect, "Editor only", style);
			GUI.color = previousColor;
		}

		public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
		{
			return EditorGUI.GetPropertyHeight(property, label, true);
		}
	}
}
