using System;
using Code.UI.Intro;
using Framework.UI.UiManagement;
using TMPro;
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using static Code.Editor.Art.LaunchWindowBuilder;
using Object = UnityEngine.Object;

namespace Code.Editor.Art
{
	public static class IntroWindowBuilder
	{
		private const string Folder = "Assets/AddressableResources/Content/UI/Intro";
		private const string PrefabPath = Folder + "/IntroWindow.prefab";
		private const string VideoPath = Folder + "/intro.mp4";
		private const string ConfigPath = "Assets/AddressableResources/Configs/UI/Windows/Window_Intro.asset";

		[MenuItem("COPYCAT/Art/Build Intro Window")]
		public static void Build()
		{
			if (EditorApplication.isPlaying)
				throw new InvalidOperationException("Stop Play Mode before building the intro window.");

			GameObject root = new GameObject("IntroWindow", typeof(RectTransform), typeof(IntroWindow));
			try
			{
				RectTransform bounds = (RectTransform)root.transform;
				bounds.anchorMin = Vector2.zero;
				bounds.anchorMax = Vector2.one;
				bounds.sizeDelta = Vector2.zero;
				root.layer = 5;
				Image background = root.AddComponent<Image>();
				background.color = Color.black;
				background.raycastTarget = true;
				RawImage video = Rectangle(root.transform, "Video", new Rect(0f, 0f, 1920f, 1080f))
					.gameObject.AddComponent<RawImage>();
				video.color = Color.white;
				video.raycastTarget = false;
				video.rectTransform.anchorMin = Vector2.zero;
				video.rectTransform.anchorMax = Vector2.one;
				video.rectTransform.pivot = Vector2.one * 0.5f;
				video.rectTransform.anchoredPosition = Vector2.zero;
				video.rectTransform.sizeDelta = Vector2.zero;
				AspectRatioFitter videoAspect = video.gameObject.AddComponent<AspectRatioFitter>();
				videoAspect.aspectMode = AspectRatioFitter.AspectMode.FitInParent;
				videoAspect.aspectRatio = 16f / 9f;
				Image dismissArea = Rectangle(root.transform, "Dismiss", new Rect(0f, 0f, 1920f, 1080f))
					.gameObject.AddComponent<Image>();
				dismissArea.color = Color.clear;
				dismissArea.raycastTarget = true;
				dismissArea.rectTransform.anchorMin = Vector2.zero;
				dismissArea.rectTransform.anchorMax = Vector2.one;
				dismissArea.rectTransform.pivot = Vector2.one * 0.5f;
				dismissArea.rectTransform.anchoredPosition = Vector2.zero;
				dismissArea.rectTransform.sizeDelta = Vector2.zero;
				Button dismiss = dismissArea.gameObject.AddComponent<Button>();
				dismiss.targetGraphic = dismissArea;
				TextMeshProUGUI skip = Label(dismissArea.transform, "SKIP  [ESC / SPACE]",
					new Rect(1430f, 986f, 430f, 54f));
				skip.fontSize = 28f;
				skip.color = new Color(1f, 1f, 1f, 0.8f);
				skip.alignment = TextAlignmentOptions.Right;
				skip.rectTransform.anchorMin = new Vector2(1f, 0f);
				skip.rectTransform.anchorMax = new Vector2(1f, 0f);
				skip.rectTransform.pivot = new Vector2(1f, 0f);
				skip.rectTransform.anchoredPosition = new Vector2(-60f, 40f);
				skip.rectTransform.sizeDelta = new Vector2(430f, 54f);
				AudioSource audio = root.AddComponent<AudioSource>();
				audio.playOnAwake = false;
				audio.spatialBlend = 0f;
				VideoPlayer player = root.AddComponent<VideoPlayer>();
				player.playOnAwake = false;
				player.waitForFirstFrame = true;
				player.skipOnDrop = true;
				player.isLooping = false;
				player.renderMode = VideoRenderMode.RenderTexture;
				player.audioOutputMode = VideoAudioOutputMode.AudioSource;
				player.clip = AssetDatabase.LoadAssetAtPath<VideoClip>(VideoPath);
				player.controlledAudioTrackCount = 1;
				player.EnableAudioTrack(0, true);
				player.SetTargetAudioSource(0, audio);
				SerializedObject window = new SerializedObject(root.GetComponent<IntroWindow>());
				Assign(window, "videoImage", video);
				Assign(window, "dismissButton", dismiss);
				Assign(window, "videoPlayer", player);
				Assign(window, "audioSource", audio);
				window.ApplyModifiedPropertiesWithoutUndo();
				PrefabUtility.SaveAsPrefabAsset(root, PrefabPath);
			}
			finally
			{
				Object.DestroyImmediate(root);
			}

			RegisterWindow();
			AssetDatabase.SaveAssets();
			AssetDatabase.Refresh();
		}

		private static void RegisterWindow()
		{
			WindowConfig config = AssetDatabase.LoadAssetAtPath<WindowConfig>(ConfigPath);
			if (config == null)
			{
				config = ScriptableObject.CreateInstance<WindowConfig>();
				AssetDatabase.CreateAsset(config, ConfigPath);
			}

			SerializedObject serialized = new SerializedObject(config);
			serialized.FindProperty("guid").stringValue = Addresses.UI.IntroWindow;
			serialized.FindProperty("prefab").FindPropertyRelative("m_AssetGUID").stringValue =
				AssetDatabase.AssetPathToGUID(PrefabPath);
			serialized.FindProperty("layerId").stringValue = UiLayers.Overlay;
			serialized.FindProperty("ignoreBack").boolValue = true;
			serialized.FindProperty("requiresCursor").boolValue = true;
			serialized.FindProperty("pausesGame").boolValue = true;
			serialized.ApplyModifiedPropertiesWithoutUndo();
			AddressableAssetSettings settings = AddressableAssetSettingsDefaultObject.Settings;
			AddressableAssetGroup group = settings.FindAssetEntry(AssetDatabase.AssetPathToGUID(
				"Assets/AddressableResources/Content/UI/Launch/LaunchWindow.prefab")).parentGroup;
			AddressableAssetEntry entry = settings.CreateOrMoveEntry(AssetDatabase.AssetPathToGUID(PrefabPath), group);
			entry.address = "intro_window_prefab";
			entry.SetLabel(Addresses.Labels.UI, true);
			settings.SetDirty(AddressableAssetSettings.ModificationEvent.EntryMoved, config, true);
		}
	}
}
