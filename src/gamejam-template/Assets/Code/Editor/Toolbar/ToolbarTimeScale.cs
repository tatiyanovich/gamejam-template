using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Toolbars;
using UnityEngine;

namespace Code.Editor
{
	[InitializeOnLoad]
	public class ToolbarTimeScale
	{
		private const string ToolbarElementName = "Toolbar/Timescale";
		private const float MaxTimeScale = 5f;

		static ToolbarTimeScale()
		{
			EditorApplication.playModeStateChanged -= HandlePlayModeStateChanged;
			EditorApplication.playModeStateChanged += HandlePlayModeStateChanged;
		}

		[MainToolbarElement(ToolbarElementName, defaultDockPosition = MainToolbarDockPosition.Middle)]
		private static IEnumerable<MainToolbarElement> CreateTimeScaleControl()
		{
			if (Application.isPlaying == false)
				yield break;
			
			yield return new MainToolbarButton(
				new MainToolbarContent("Reset", "Reset time scale to 1x"),
				HandleResetClicked);

			yield return new MainToolbarSlider(
				new MainToolbarContent("TimeScale", "Adjust game time scale"),
				Time.timeScale,
				0f,
				MaxTimeScale,
				HandleSliderValueChanged);
		}

		private static void HandleResetClicked()
		{
			if (Application.isPlaying == false)
				return;

			Time.timeScale = 1f;
			MainToolbar.Refresh(ToolbarElementName);
		}

		private static void HandleSliderValueChanged(float value)
		{
			if (Application.isPlaying == false)
				return;

			Time.timeScale = value;
			MainToolbar.Refresh(ToolbarElementName);
		}

		private static void HandlePlayModeStateChanged(PlayModeStateChange state)
		{
			if (state == PlayModeStateChange.EnteredEditMode)
				Time.timeScale = 1f;

			MainToolbar.Refresh(ToolbarElementName);
		}
	}
}
