using System;
using Code.UI.Loading;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using static Code.Editor.Art.LaunchWindowBuilder;
using Object = UnityEngine.Object;

namespace Code.Editor.Art
{
	public static class LoadingWindowBuilder
	{
		private const string Content = "Assets/AddressableResources/Content/";
		private const string Prefab = Content + "UI/Loading/LoadingWindow.prefab";

		private static readonly Color Backdrop = new Color32(39, 65, 53, 255);
		private static readonly Color Shade = new Color32(39, 65, 53, 196);
		private static readonly Color Paper = new Color32(255, 246, 227, 255);
		private static readonly Color PaperShade = new Color32(234, 223, 198, 255);
		private static readonly Color PencilInk = new Color32(59, 58, 140, 255);

		[MenuItem("COPYCAT/Art/Build Loading Window")]
		public static void Build()
		{
			if (EditorApplication.isPlaying)
				throw new InvalidOperationException("Stop Play Mode before building the loading window.");

			GameObject root = PrefabUtility.LoadPrefabContents(Prefab);
			try
			{
				while (root.transform.childCount > 0)
					Object.DestroyImmediate(root.transform.GetChild(0).gameObject);

				Image background = root.GetComponent<Image>();
				if (background == null)
					background = root.AddComponent<Image>();
				background.sprite = null;
				background.color = Backdrop;
				background.raycastTarget = true;

				RectTransform layout = Rectangle(root.transform, "Layout", new Rect(0f, 0f, 1920f, 1080f));
				layout.anchorMin = layout.anchorMax = layout.pivot = Vector2.one * 0.5f;
				layout.anchoredPosition = Vector2.zero;

				Picture(layout, "Classroom/classroom_wall", new Rect(0f, 0f, 1920f, 700f));
				Picture(layout, "Classroom/classroom_floor", new Rect(0f, 410f, 1920f, 670f));
				Picture(layout, "Classroom/blackboard", new Rect(510f, 76f, 900f, 300f));
				Picture(layout, "Classroom/teacher_desk", new Rect(700f, 470f, 520f, 160f));
				Picture(layout, "Classroom/desk_neighbour_left", new Rect(44f, 520f, 620f, 260f));
				Picture(layout, "Classroom/desk_neighbour_right", new Rect(1256f, 520f, 620f, 260f));
				Picture(layout, "Classroom/desk_player", new Rect(0f, 733f, 1920f, 360f));

				Image shade = Rectangle(layout, "Shade", new Rect(0f, 0f, 1920f, 1080f)).gameObject.AddComponent<Image>();
				shade.color = Shade;
				shade.raycastTarget = false;

				RectTransform logo = Picture(layout, "UI/Copycat/logo_copycat", new Rect(580f, 108f, 760f, 253f))
					.rectTransform;
				logo.pivot = Vector2.one * 0.5f;
				logo.anchoredPosition = new Vector2(960f, -234.5f);

				RectTransform duck = Picture(layout, "Duck/duck_idle", new Rect(1392f, 604f, 200f, 200f)).rectTransform;

				Picture(layout, "UI/Copycat/panel_paper_9slice", new Rect(410f, 640f, 940f, 260f));

				TextMeshProUGUI status = Label(layout, string.Empty, new Rect(450f, 676f, 860f, 70f));
				status.name = "Status";
				status.font = Handwriting();
				status.fontSize = 46f;
				status.color = PencilInk;

				RectTransform bar = Rectangle(layout, "ProgressBar", new Rect(475f, 766f, 810f, 96f));
				Image track = Rectangle(bar, "Track", new Rect(29f, 21f, 752f, 54f)).gameObject.AddComponent<Image>();
				track.color = PaperShade;
				track.raycastTarget = false;
				Image fill = Picture(bar, "UI/Copycat/bar_fill", new Rect(29f, 21f, 752f, 54f));
				fill.name = "Fill";
				fill.type = Image.Type.Filled;
				fill.fillMethod = Image.FillMethod.Horizontal;
				fill.fillOrigin = (int)Image.OriginHorizontal.Left;
				fill.fillAmount = 0f;
				Picture(bar, "UI/Copycat/bar_frame", new Rect(0f, 0f, 810f, 96f)).name = "Frame";

				TextMeshProUGUI footer = Label(layout, "Cheat to win.", new Rect(410f, 940f, 1100f, 60f));
				footer.name = "Footer";
				footer.font = Handwriting();
				footer.fontSize = 40f;
				footer.color = Paper;

				SerializedObject window = new SerializedObject(root.GetComponent<LoadingWindow>());
				Assign(window, "layout", layout);
				Assign(window, "logo", logo);
				Assign(window, "duck", duck);
				Assign(window, "progressFill", fill);
				Assign(window, "status", status);
				window.ApplyModifiedPropertiesWithoutUndo();
				PrefabUtility.SaveAsPrefabAsset(root, Prefab);
			}
			finally
			{
				PrefabUtility.UnloadPrefabContents(root);
			}

			AssetDatabase.SaveAssets();
		}

		private static TMP_FontAsset Handwriting()
		{
			return AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(
				Content + "CopycatShared/Fonts/PatrickHand-Regular-SDF.asset");
		}
	}
}
