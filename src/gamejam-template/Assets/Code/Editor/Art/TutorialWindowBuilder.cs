using System;
using Code.UI.Tutorial;
using Framework.UI.UiManagement;
using TMPro;
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;
using UnityEngine;
using UnityEngine.UI;
using static Code.Editor.Art.LaunchWindowBuilder;
using Object = UnityEngine.Object;

namespace Code.Editor.Art
{
	public static class TutorialWindowBuilder
	{
		private const string Folder = "Assets/AddressableResources/Content/UI/Tutorial";
		private const string PrefabPath = Folder + "/TutorialWindow.prefab";
		private const string TexturePath = Folder + "/how_to_play_v2.png";
		private const string ConfigPath = "Assets/AddressableResources/Configs/UI/Windows/Window_Tutorial.asset";

		[MenuItem("COPYCAT/Art/Build Tutorial Window")]
		public static void Build()
		{
			if (EditorApplication.isPlaying)
				throw new InvalidOperationException("Stop Play Mode before building the tutorial window.");

			ConfigureTexture();
			GameObject root = new GameObject("TutorialWindow", typeof(RectTransform), typeof(TutorialWindow));
			try
			{
				RectTransform bounds = (RectTransform)root.transform;
				bounds.anchorMin = Vector2.zero;
				bounds.anchorMax = Vector2.one;
				bounds.sizeDelta = Vector2.zero;
				root.layer = 5;
				Image background = root.AddComponent<Image>();
				background.color = new Color32(22, 29, 27, 255);
				background.raycastTarget = true;
				RectTransform layout = Rectangle(root.transform, "Layout", new Rect(0f, 0f, 1920f, 1080f));
				layout.anchorMin = layout.anchorMax = layout.pivot = Vector2.one * 0.5f;
				layout.anchoredPosition = Vector2.zero;
				Image tutorial = Rectangle(layout, "HowToPlay", new Rect(0f, 0f, 1920f, 1080f))
					.gameObject.AddComponent<Image>();
				tutorial.sprite = AssetDatabase.LoadAssetAtPath<Sprite>(TexturePath);
				tutorial.preserveAspect = true;
				tutorial.raycastTarget = false;
				Button close = CloseButton(layout);
				SerializedObject window = new SerializedObject(root.GetComponent<TutorialWindow>());
				Assign(window, "dismissButton", close);
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

		private static Button CloseButton(Transform parent)
		{
			Image circle = Rectangle(parent, "Close", new Rect(1780f, 44f, 96f, 96f)).gameObject.AddComponent<Image>();
			circle.sprite = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/Knob.psd");
			circle.color = new Color32(207, 49, 45, 255);
			circle.raycastTarget = true;
			Button button = circle.gameObject.AddComponent<Button>();
			button.targetGraphic = circle;
			CrossStroke(circle.transform, 45f);
			CrossStroke(circle.transform, -45f);
			return button;
		}

		private static void CrossStroke(Transform parent, float rotation)
		{
			Image stroke = Rectangle(parent, "Stroke", new Rect(0f, 0f, 56f, 10f)).gameObject.AddComponent<Image>();
			stroke.color = Color.white;
			stroke.raycastTarget = false;
			stroke.rectTransform.anchorMin = Vector2.one * 0.5f;
			stroke.rectTransform.anchorMax = Vector2.one * 0.5f;
			stroke.rectTransform.pivot = Vector2.one * 0.5f;
			stroke.rectTransform.anchoredPosition = Vector2.zero;
			stroke.rectTransform.localRotation = Quaternion.Euler(0f, 0f, rotation);
		}

		private static void ConfigureTexture()
		{
			TextureImporter importer = (TextureImporter)AssetImporter.GetAtPath(TexturePath);
			if (importer.textureType == TextureImporterType.Sprite)
				return;

			importer.textureType = TextureImporterType.Sprite;
			importer.spriteImportMode = SpriteImportMode.Single;
			importer.alphaIsTransparency = true;
			importer.SaveAndReimport();
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
			serialized.FindProperty("guid").stringValue = Addresses.UI.TutorialWindow;
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
			entry.address = "tutorial_window_prefab";
			entry.SetLabel(Addresses.Labels.UI, true);
			settings.SetDirty(AddressableAssetSettings.ModificationEvent.EntryMoved, config, true);
		}
	}
}
