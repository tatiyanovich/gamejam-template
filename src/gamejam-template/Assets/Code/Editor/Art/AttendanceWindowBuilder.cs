using System;
using Code.UI.Attendance;
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
	public static class AttendanceWindowBuilder
	{
		private const string Folder = "Assets/AddressableResources/Content/UI/Attendance";
		private const string PrefabPath = Folder + "/AttendanceWindow.prefab";
		private const string ConfigPath = "Assets/AddressableResources/Configs/UI/Windows/Window_Attendance.asset";

		[MenuItem("COPYCAT/Art/Build B2 Attendance Window")]
		public static void Build()
		{
			if (EditorApplication.isPlaying)
				throw new InvalidOperationException("Stop Play Mode before building the attendance window.");

			if (AssetDatabase.IsValidFolder(Folder) == false)
				AssetDatabase.CreateFolder("Assets/AddressableResources/Content/UI", "Attendance");

			GameObject root = new GameObject("AttendanceWindow", typeof(RectTransform), typeof(AttendanceWindow));
			try
			{
				RectTransform bounds = (RectTransform)root.transform;
				bounds.anchorMin = Vector2.zero;
				bounds.anchorMax = Vector2.one;
				bounds.sizeDelta = Vector2.zero;
				root.layer = 5;
				root.AddComponent<Image>().color = new Color32(39, 65, 53, 255);
				RectTransform layout = Rectangle(root.transform, "Layout", new Rect(0f, 0f, 1920f, 1080f));
				layout.anchorMin = layout.anchorMax = layout.pivot = Vector2.one * 0.5f;
				layout.anchoredPosition = Vector2.zero;
				Picture(layout, "UI/Copycat/panel_paper_9slice", new Rect(460f, 50f, 1000f, 980f));
				Label(layout, "ATTENDANCE SHEET", new Rect(520f, 105f, 880f, 80f)).fontSize = 56f;
				Label(layout, "Sign in before the exam.", new Rect(520f, 190f, 880f, 60f));
				Label(layout, "Student name", new Rect(610f, 280f, 700f, 50f));
				TMP_InputField input = NameInput(layout);
				((RectTransform)input.transform).anchoredPosition = new Vector2(610f, -340f);
				Label(layout, "12 characters max.", new Rect(610f, 440f, 700f, 45f)).fontSize = 28f;
				Label(layout, "MIC CHECK", new Rect(610f, 520f, 700f, 55f)).fontSize = 40f;
				Image track = Rectangle(layout, "MicrophoneTrack", new Rect(610f, 605f, 700f, 44f))
					.gameObject.AddComponent<Image>();
				track.raycastTarget = false;
				Image fill = Rectangle(track.transform, "MicrophoneFill", new Rect(0f, 0f, 700f, 44f))
					.gameObject.AddComponent<Image>();
				fill.sprite = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/UISprite.psd");
				fill.type = Image.Type.Filled;
				fill.fillMethod = Image.FillMethod.Horizontal;
				fill.fillOrigin = 0;
				fill.color = new Color32(80, 150, 76, 255);
				fill.raycastTarget = false;
				RectTransform threshold = Rectangle(track.transform, "Threshold", new Rect(0f, 0f, 6f, 60f));
				threshold.pivot = Vector2.one * 0.5f;
				threshold.sizeDelta = new Vector2(6f, 16f);
				Image marker = threshold.gameObject.AddComponent<Image>();
				marker.color = new Color32(170, 54, 43, 255);
				marker.raycastTarget = false;
				TextMeshProUGUI hint = Label(layout, "Meow to test your mic", new Rect(650f, 680f, 660f, 65f));
				RectTransform checkmark = Rectangle(layout, "Checkmark", new Rect(606f, 700f, 36f, 30f));
				CheckStroke(checkmark, new Rect(0f, 12f, 8f, 20f), 40f);
				CheckStroke(checkmark, new Rect(10f, 26f, 8f, 38f), -40f);
				checkmark.gameObject.SetActive(false);
				UnityEngine.UI.Button start = Button(layout, "START EXAM", new Rect(730f, 835f, 460f, 100f));
				SerializedObject window = new SerializedObject(root.GetComponent<AttendanceWindow>());
				Assign(window, "layout", layout);
				Assign(window, "studentName", input);
				Assign(window, "startExamButton", start);
				Assign(window, "microphoneFill", fill);
				Assign(window, "microphoneTrack", track);
				Assign(window, "microphoneThreshold", threshold);
				Assign(window, "microphoneHint", hint);
				Assign(window, "microphoneCheckmark", checkmark.gameObject);
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

		private static void CheckStroke(Transform parent, Rect rectangle, float rotation)
		{
			Image stroke = Rectangle(parent, "Stroke", rectangle).gameObject.AddComponent<Image>();
			stroke.color = new Color32(80, 150, 76, 255);
			stroke.raycastTarget = false;
			stroke.rectTransform.localRotation = Quaternion.Euler(0f, 0f, rotation);
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
			serialized.FindProperty("guid").stringValue = Addresses.UI.AttendanceWindow;
			serialized.FindProperty("prefab").FindPropertyRelative("m_AssetGUID").stringValue =
				AssetDatabase.AssetPathToGUID(PrefabPath);
			serialized.FindProperty("layerId").stringValue = "Overlay";
			serialized.FindProperty("ignoreBack").boolValue = true;
			serialized.FindProperty("requiresCursor").boolValue = true;
			serialized.ApplyModifiedPropertiesWithoutUndo();
			AddressableAssetSettings settings = AddressableAssetSettingsDefaultObject.Settings;
			AddressableAssetGroup group = settings.FindAssetEntry(AssetDatabase.AssetPathToGUID(
				"Assets/AddressableResources/Content/UI/Launch/LaunchWindow.prefab")).parentGroup;
			AddressableAssetEntry entry = settings.CreateOrMoveEntry(AssetDatabase.AssetPathToGUID(PrefabPath), group);
			entry.address = "attendance_window_prefab";
			entry.SetLabel(Addresses.Labels.UI, true);
			settings.SetDirty(AddressableAssetSettings.ModificationEvent.EntryMoved, config, true);
		}
	}
}
