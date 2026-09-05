using System;
using Code.UI.Animations;
using Code.UI.Launch;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace Code.Editor.Art
{
	public static class LaunchWindowBuilder
	{
		private const string Content = "Assets/AddressableResources/Content/";
		private const string Prefab = Content + "UI/Launch/LaunchWindow.prefab";
		private static readonly Color Ink = new Color32(43, 33, 24, 255);
		private static readonly Color Paper = new Color32(255, 248, 231, 255);

		[MenuItem("COPYCAT/Art/Build B1 Launch Window")]
		public static void Build()
		{
			if (EditorApplication.isPlaying)
				throw new InvalidOperationException("Stop Play Mode before building the launch window.");

			GameObject root = PrefabUtility.LoadPrefabContents(Prefab);
			try
			{
				while (root.transform.childCount > 0)
					Object.DestroyImmediate(root.transform.GetChild(0).gameObject);

				Image background = root.GetComponent<Image>();
				if (background == null)
					background = root.AddComponent<Image>();
				background.color = new Color32(39, 65, 53, 255);
				background.raycastTarget = true;

				RectTransform layout = Rectangle(root.transform, "Layout", new Rect(0f, 0f, 1920f, 1080f));
				layout.anchorMin = layout.anchorMax = layout.pivot = new Vector2(0.5f, 0.5f);
				layout.anchoredPosition = Vector2.zero;
				Picture(layout, "Classroom/classroom_wall", new Rect(0f, 0f, 1920f, 700f));
				Picture(layout, "Classroom/classroom_floor", new Rect(0f, 410f, 1920f, 670f));
				Picture(layout, "Classroom/blackboard", new Rect(510f, 76f, 900f, 300f));
				Picture(layout, "Classroom/teacher_desk", new Rect(700f, 470f, 520f, 160f));
				Picture(layout, "Classroom/desk_neighbour_left", new Rect(44f, 520f, 620f, 260f));
				Picture(layout, "Classroom/desk_neighbour_right", new Rect(1256f, 520f, 620f, 260f));
				Picture(layout, "Classroom/desk_player", new Rect(0f, 733f, 1920f, 360f));
				Image shade = Rectangle(layout, "Shade", new Rect(0f, 0f, 1920f, 1080f)).gameObject.AddComponent<Image>();
				shade.color = new Color32(39, 65, 53, 174);
				shade.raycastTarget = false;

				RectTransform menu = Rectangle(layout, "Menu", new Rect(0f, 0f, 1920f, 1080f));
				Picture(menu, "UI/Copycat/logo_copycat", new Rect(510f, 170f, 900f, 300f));
				TextMeshProUGUI slogan = Label(menu, "Cheat to win.", new Rect(510f, 430f, 900f, 60f));
				slogan.font = Font("PatrickHand-Regular");
				slogan.fontSize = 44f;
				slogan.color = Paper;
				Picture(menu, "UI/Copycat/panel_paper_9slice", new Rect(600f, 498f, 720f, 350f));
				Button play = Button(menu, "PLAY", new Rect(730f, 530f, 460f, 100f));
				Button quit = Button(menu, "QUIT", new Rect(730f, 656f, 460f, 100f));
				TextMeshProUGUI footer = Label(menu, "Microphone required. You will have to meow out loud.",
					new Rect(160f, 932f, 1600f, 70f));
				footer.fontSize = 32f;
				footer.color = Paper;

				SerializedObject window = new SerializedObject(root.GetComponent<LaunchWindow>());
				Assign(window, "layout", layout);
				Assign(window, "menu", menu.gameObject);
				Assign(window, "playButton", play);
				Assign(window, "quitButton", quit);
				window.ApplyModifiedPropertiesWithoutUndo();
				PrefabUtility.SaveAsPrefabAsset(root, Prefab);
			}
			finally
			{
				PrefabUtility.UnloadPrefabContents(root);
			}

			AssetDatabase.SaveAssets();
		}

		internal static RectTransform Rectangle(Transform parent, string name, Rect rectangle)
		{
			RectTransform result = new GameObject(name, typeof(RectTransform)).GetComponent<RectTransform>();
			result.gameObject.layer = 5;
			result.SetParent(parent, false);
			result.anchorMin = result.anchorMax = result.pivot = new Vector2(0f, 1f);
			result.anchoredPosition = new Vector2(rectangle.x, -rectangle.y);
			result.sizeDelta = rectangle.size;
			return result;
		}

		internal static Image Picture(Transform parent, string path, Rect rectangle)
		{
			Image image = Rectangle(parent, path.Substring(path.LastIndexOf('/') + 1), rectangle)
				.gameObject.AddComponent<Image>();
			image.sprite = AssetDatabase.LoadAssetAtPath<Sprite>(Content + path + ".png");
			image.type = image.sprite.border != Vector4.zero ? Image.Type.Sliced : Image.Type.Simple;
			image.raycastTarget = false;
			return image;
		}

		private static TMP_FontAsset Font(string name)
		{
			return AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(Content + "CopycatShared/Fonts/" + name + "-SDF.asset");
		}

		internal static TextMeshProUGUI Label(Transform parent, string text, Rect rectangle)
		{
			TextMeshProUGUI label = Rectangle(parent, "Label", rectangle).gameObject.AddComponent<TextMeshProUGUI>();
			label.font = Font("Nunito-Bold");
			label.text = text;
			label.fontSize = 36f;
			label.color = Ink;
			label.alignment = TextAlignmentOptions.Center;
			label.raycastTarget = false;
			label.richText = false;
			return label;
		}

		internal static Button Button(Transform parent, string text, Rect rectangle)
		{
			Image image = Picture(parent, "UI/Copycat/button_yellow_9slice", rectangle);
			image.name = text;
			image.raycastTarget = true;
			TextMeshProUGUI label = Label(image.transform, text, new Rect(0f, 0f, rectangle.width - 64f, 64f));
			label.rectTransform.anchorMin = label.rectTransform.anchorMax = label.rectTransform.pivot = Vector2.one * 0.5f;
			label.rectTransform.anchoredPosition = Vector2.zero;
			label.fontSize = 40f;
			ButtonLabelOffset button = image.gameObject.AddComponent<ButtonLabelOffset>();
			button.targetGraphic = image;
			button.transition = Selectable.Transition.SpriteSwap;
			button.spriteState = new SpriteState
			{
				highlightedSprite = Sprite("button_yellow_9slice_hover"),
				selectedSprite = Sprite("button_yellow_9slice_hover"),
				pressedSprite = Sprite("button_yellow_9slice_pressed"),
				disabledSprite = Sprite("button_yellow_9slice")
			};
			SerializedObject serialized = new SerializedObject(button);
			Assign(serialized, "label", label.rectTransform);
			serialized.ApplyModifiedPropertiesWithoutUndo();
			return button;
		}

		private static Sprite Sprite(string name)
		{
			return AssetDatabase.LoadAssetAtPath<Sprite>(Content + "UI/Copycat/" + name + ".png");
		}

		internal static TMP_InputField NameInput(Transform parent)
		{
			Image image = Picture(parent, "UI/Copycat/panel_paper_9slice", new Rect(610f, 500f, 700f, 95f));
			image.name = "StudentName";
			image.raycastTarget = true;
			RectTransform viewport = Rectangle(image.transform, "Viewport", new Rect(28f, 10f, 644f, 75f));
			viewport.gameObject.AddComponent<RectMask2D>();
			TextMeshProUGUI text = Label(viewport, string.Empty, new Rect(0f, 0f, 644f, 75f));
			TextMeshProUGUI placeholder = Label(viewport, "Nameless Kitten", new Rect(0f, 0f, 644f, 75f));
			placeholder.color = new Color(Ink.r, Ink.g, Ink.b, 0.5f);
			TMP_InputField input = image.gameObject.AddComponent<TMP_InputField>();
			input.targetGraphic = image;
			input.textViewport = viewport;
			input.textComponent = text;
			input.placeholder = placeholder;
			input.characterLimit = 12;
			input.lineType = TMP_InputField.LineType.SingleLine;
			input.richText = false;
			return input;
		}

		internal static void Assign(SerializedObject target, string property, Object value)
		{
			target.FindProperty(property).objectReferenceValue = value;
		}
	}
}
