using Framework.Essentials.ScriptableObjects;
using UnityEditor;
using UnityEngine;

namespace Framework.Essentials.Editor
{
	[CustomPropertyDrawer(typeof(ID))]
	public class IDDrawer : PropertyDrawer
	{
		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			if (string.IsNullOrEmpty(property.stringValue))
			{
				property.stringValue = IDUtility.GenerateID();
			}

			float buttonSize = 25f;
			Rect propertyRectField = new(position.x, position.y, position.width - (buttonSize * 2), position.height);
			Rect buttonRectField = new(position.x + position.width - buttonSize, position.y, buttonSize, position.height);
			Rect copyButtonRect = new(position.x + position.width - buttonSize * 2, position.y, buttonSize, position.height);

			if (GUI.Button(buttonRectField, EditorGUIUtility.IconContent("d_Refresh@2x")))
			{
				if (EditorUtility.DisplayDialog(
					    "Regenerate ID",
					    "You are about to regenerate the ID for this object, which will break all existing references to it. Do you want to proceed?",
					    "Yes", "No"))
				{
					property.stringValue = IDUtility.GenerateID();
					Debug.Log($"Regenerated ID for {property.serializedObject.targetObject.name} is {property.stringValue}");
				}
			}

			if (GUI.Button(copyButtonRect, EditorGUIUtility.IconContent("Clipboard")))
			{
				GUIUtility.systemCopyBuffer = property.stringValue;
			}

			GUI.enabled = false;
			EditorGUI.PropertyField(propertyRectField, property, label, true);
			GUI.enabled = true;
		}
	}
}
