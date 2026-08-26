#if UNITY_EDITOR

using System;
using Code.UI;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ProgressBar))]
public class ProgressBarEditor : Editor
{
	public override void OnInspectorGUI()
	{
		base.OnInspectorGUI();

		ProgressBar myTarget = (ProgressBar) target;

		float previousValue = myTarget.Value;
		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.LabelField("Value", GUILayout.MaxWidth(212.0f));
		myTarget.Value = EditorGUILayout.Slider(myTarget.Value, 0, 1, GUILayout.MaxWidth(700.0f));
		EditorGUILayout.EndHorizontal();

		if (Application.isPlaying) return;

		float newValue = myTarget.Value;
		if (Math.Abs(previousValue - newValue) > 0.001f)
		{
			myTarget.Value = newValue;
			EditorUtility.SetDirty(myTarget);
		}
	}
}

#endif