using System;
using Code.UI.Gameplay;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using static Code.Editor.Art.LaunchWindowBuilder;
using Object = UnityEngine.Object;

namespace Code.Editor.Art
{
	public static class GameplayWindowBuilder
	{
		private const string Content = "Assets/AddressableResources/Content/";
		private const string Prefab = Content + "UI/Gameplay/GameplayWindow.prefab";

		[MenuItem("COPYCAT/Art/Build B3 Gameplay Window")]
		public static void Build()
		{
			if (EditorApplication.isPlaying)
				throw new InvalidOperationException("Stop Play Mode before building the gameplay window.");

			BuildDangerVignette();

			GameObject root = PrefabUtility.LoadPrefabContents(Prefab);
			try
			{
				while (root.transform.childCount > 0)
					Object.DestroyImmediate(root.transform.GetChild(0).gameObject);

				DangerVignetteView vignette = Vignette(root.transform);
				RectTransform layout = Rectangle(root.transform, "Layout", new Rect(0f, 0f, 1920f, 1080f));
				layout.anchorMin = layout.anchorMax = layout.pivot = Vector2.one * 0.5f;
				layout.anchoredPosition = Vector2.zero;
				Picture(layout, "UI/Copycat/chip_hud", new Rect(42f, 8f, 280f, 60f)).pixelsPerUnitMultiplier = 1.5f;
				Label(layout, "COPYCAT", new Rect(42f, 8f, 280f, 60f)).fontSize = 28f;
				Picture(layout, "UI/Copycat/chip_hud", new Rect(732f, 8f, 456f, 60f)).pixelsPerUnitMultiplier = 1.5f;
				TMP_Text answers = Label(layout, "", new Rect(732f, 8f, 456f, 60f));
				answers.fontSize = 28f;
				TMP_Text clock = Label(layout, "", new Rect(1580f, 130f, 192f, 62f));
				clock.fontSize = 48f;
				clock.characterSpacing = 4f;
				RectTransform suspicion = Widget(layout, "SuspicionMeter", new Vector2(650f, -388f));
				suspicion.localScale = Vector3.one * 0.75f;
				RectTransform meow = Widget(layout, "MeowMeter", new Vector2(80f, -760f));
				TMP_Text hint = Label(layout, "", new Rect(25f, 1008f, 350f, 56f));
				hint.fontSize = 24f;
				Image cooldown = Picture(meow, "Papers/ring_timer", new Rect(0f, 0f, 240f, 240f));
				cooldown.name = "Cooldown";
				cooldown.type = Image.Type.Filled;
				cooldown.fillMethod = Image.FillMethod.Radial360;
				cooldown.fillOrigin = (int)Image.Origin360.Top;
				cooldown.color = new Color32(150, 150, 150, 255);
				RectTransform threshold = (RectTransform)meow.Find("meow_threshold_line");
				threshold.pivot = Vector2.one * 0.5f;
				Image duck = Picture(layout, "Duck/keycap_q", new Rect(1618f, 918f, 64f, 64f));
				Image duckHitArea = Rectangle(layout, "DuckButton", new Rect(1560f, 780f, 200f, 210f))
					.gameObject.AddComponent<Image>();
				duckHitArea.color = Color.clear;
				duckHitArea.raycastTarget = true;
				Button duckButton = duckHitArea.gameObject.AddComponent<Button>();
				duckButton.targetGraphic = duck;
				RectTransform bubble = Rectangle(layout, "TeacherBubble", new Rect(1140f, 378f, 260f, 112f));
				Picture(bubble, "UI/Copycat/panel_paper_9slice", new Rect(0f, 0f, 260f, 112f));
				TMP_Text speech = Label(bubble, "", new Rect(16f, 10f, 228f, 92f));
				speech.fontSize = 24f;
				RectTransform hintBubble = Rectangle(layout, "HintBubble", new Rect(560f, 472f, 560f, 112f));
				RectTransform hintPanel = Picture(hintBubble, "UI/Copycat/panel_paper_9slice",
					new Rect(0f, 0f, 560f, 112f)).rectTransform;
				hintPanel.anchorMin = Vector2.zero;
				hintPanel.anchorMax = Vector2.one;
				hintPanel.offsetMin = Vector2.zero;
				hintPanel.offsetMax = Vector2.zero;
				TMP_Text hintText = Label(hintBubble, "", new Rect(24f, 10f, 512f, 92f));
				hintText.fontSize = 26f;
				RectTransform hintStrokes = Rectangle(hintBubble, "HintStrokes", new Rect(24f, 104f, 512f, 44f));
				Picture(hintStrokes, "Papers/glyph_arrow_left_normal", new Rect(178f, 0f, 44f, 44f));
				Picture(hintStrokes, "Papers/glyph_arrow_up_normal", new Rect(234f, 0f, 44f, 44f));
				Picture(hintStrokes, "Papers/glyph_arrow_right_normal", new Rect(290f, 0f, 44f, 44f));
				FlashStackView flashes = FlashStack(layout);
				SerializedObject window = new(root.GetComponent<GameplayWindow>());
				Assign(window, "layout", layout);
				Assign(window, "answers", answers);
				Assign(window, "clock", clock);
				Assign(window, "suspicionFill", suspicion.Find("bar_fill").GetComponent<Image>());
				Assign(window, "microphoneFill", meow.Find("meow_fill").GetComponent<Image>());
				Assign(window, "microphoneThreshold", threshold);
				Assign(window, "microphoneHint", hint);
				Assign(window, "cooldownFill", cooldown);
				Assign(window, "duckButton", duckButton);
				Assign(window, "bubble", bubble.gameObject);
				Assign(window, "speech", speech);
				Assign(window, "hintBubble", hintBubble);
				Assign(window, "hint", hintText);
				Assign(window, "hintStrokes", hintStrokes.gameObject);
				Assign(window, "vignette", vignette);
				Assign(window, "flashes", flashes);
				window.ApplyModifiedPropertiesWithoutUndo();
				PrefabUtility.SaveAsPrefabAsset(root, Prefab);
			}
			finally
			{
				PrefabUtility.UnloadPrefabContents(root);
			}

			BuildPawTimer();
			RemoveStaticKeycap();
			RemoveClockHands();
			AssetDatabase.SaveAssets();
		}

		private static DangerVignetteView Vignette(Transform parent)
		{
			GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(Content + "UI/Copycat/DangerVignette.prefab");
			GameObject instance = (GameObject)PrefabUtility.InstantiatePrefab(prefab, parent);
			RectTransform rectangle = (RectTransform)instance.transform;
			rectangle.anchorMin = Vector2.zero;
			rectangle.anchorMax = Vector2.one;
			rectangle.offsetMin = Vector2.zero;
			rectangle.offsetMax = Vector2.zero;
			return instance.GetComponent<DangerVignetteView>();
		}

		private static FlashStackView FlashStack(Transform parent)
		{
			RectTransform stack = Rectangle(parent, "FlashStack", new Rect(40f, 530f, 420f, 176f));
			FlashRowView[] flashRows = new FlashRowView[3];
			for (int index = 0; index < flashRows.Length; index++)
				flashRows[index] = FlashRow(stack, index);

			FlashStackView view = stack.gameObject.AddComponent<FlashStackView>();
			SerializedObject serialized = new(view);
			SerializedProperty rows = serialized.FindProperty("rows");
			rows.arraySize = flashRows.Length;
			for (int index = 0; index < flashRows.Length; index++)
				rows.GetArrayElementAtIndex(index).objectReferenceValue = flashRows[index];
			serialized.ApplyModifiedPropertiesWithoutUndo();
			return view;
		}

		private static FlashRowView FlashRow(Transform parent, int index)
		{
			RectTransform row = Rectangle(parent, "FlashRow", new Rect(0f, 120f - index * 60f, 420f, 52f));
			Picture(row, "UI/Copycat/chip_hud", new Rect(0f, 0f, 420f, 52f)).pixelsPerUnitMultiplier = 1.5f;
			TMP_Text label = Label(row, "", new Rect(20f, 0f, 380f, 52f));
			label.fontSize = 30f;
			label.alignment = TextAlignmentOptions.Left;
			FlashRowView view = row.gameObject.AddComponent<FlashRowView>();
			SerializedObject serialized = new(view);
			Assign(serialized, "label", label);
			serialized.ApplyModifiedPropertiesWithoutUndo();
			row.gameObject.SetActive(false);
			return view;
		}

		private static void BuildDangerVignette()
		{
			string path = Content + "UI/Copycat/DangerVignette.prefab";
			GameObject root = PrefabUtility.LoadPrefabContents(path);
			try
			{
				DangerVignetteView view = root.GetComponent<DangerVignetteView>();
				if (view == null)
					view = root.AddComponent<DangerVignetteView>();
				SerializedObject serialized = new(view);
				Assign(serialized, "image", root.GetComponent<Image>());
				serialized.ApplyModifiedPropertiesWithoutUndo();
				PrefabUtility.SaveAsPrefabAsset(root, path);
			}
			finally
			{
				PrefabUtility.UnloadPrefabContents(root);
			}
		}

		internal static void ConfigurePawTimer(GameObject root)
		{
			PawTimerView view = root.GetComponent<PawTimerView>();
			if (view == null)
				view = root.AddComponent<PawTimerView>();
			Canvas canvas = root.GetComponentInChildren<Canvas>();
			SerializedObject serialized = new(view);
			Assign(serialized, "canvas", canvas);
			Assign(serialized, "fill", canvas.transform.Find("ring_timer").GetComponent<Image>());
			serialized.ApplyModifiedPropertiesWithoutUndo();
			canvas.enabled = false;
		}

		private static void RemoveClockHands()
		{
			string path = Content + "Classroom/Classroom.prefab";
			GameObject root = PrefabUtility.LoadPrefabContents(path);
			try
			{
				foreach (string name in new[] { "clock_hand_hour", "clock_hand_minute" })
				{
					Transform hand = root.transform.Find(name);
					if (hand != null)
						Object.DestroyImmediate(hand.gameObject);
				}
				PrefabUtility.SaveAsPrefabAsset(root, path);
			}
			finally
			{
				PrefabUtility.UnloadPrefabContents(root);
			}
		}

		private static void RemoveStaticKeycap()
		{
			UnityEngine.SceneManagement.Scene scene =
				UnityEngine.SceneManagement.SceneManager.GetSceneByPath("Assets/Scenes/Gameplay.unity");
			bool wasLoaded = scene.isLoaded;
			if (wasLoaded == false)
				scene = EditorSceneManager.OpenScene("Assets/Scenes/Gameplay.unity", OpenSceneMode.Additive);
			try
			{
				foreach (GameObject root in scene.GetRootGameObjects())
				{
					if (root.name != "CopycatArt")
						continue;
					Transform keycap = root.transform.Find("keycap_q");
					if (keycap != null)
						Object.DestroyImmediate(keycap.gameObject);
				}
				EditorSceneManager.SaveScene(scene);
			}
			finally
			{
				if (wasLoaded == false)
					EditorSceneManager.CloseScene(scene, true);
			}
		}

		private static void BuildPawTimer()
		{
			string path = Content + "Papers/PawTimer.prefab";
			GameObject root = PrefabUtility.LoadPrefabContents(path);
			try
			{
				ConfigurePawTimer(root);
				PrefabUtility.SaveAsPrefabAsset(root, path);
			}
			finally
			{
				PrefabUtility.UnloadPrefabContents(root);
			}
		}

		private static RectTransform Widget(Transform parent, string name, Vector2 position)
		{
			GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(Content + "UI/Copycat/" + name + ".prefab");
			GameObject instance = (GameObject)PrefabUtility.InstantiatePrefab(prefab, parent);
			RectTransform widget = (RectTransform)instance.transform;
			widget.anchorMin = widget.anchorMax = widget.pivot = new Vector2(0f, 1f);
			widget.anchoredPosition = position;
			return widget;
		}
	}
}
