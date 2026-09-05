using System;
using Code.UI.Result;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using static Code.Editor.Art.LaunchWindowBuilder;
using Object = UnityEngine.Object;

namespace Code.Editor.Art
{
	public static class ResultWindowBuilder
	{
		private const string Content = "Assets/AddressableResources/Content/";
		private const string Prefab = Content + "UI/Result/ResultScreen.prefab";
		private const int LeaderboardRowCount = 10;

		private static readonly Color Ink = new Color32(43, 33, 24, 255);
		private static readonly Color PencilInk = new Color32(59, 58, 140, 255);
		private static readonly Color PaperShade = new Color32(234, 223, 198, 255);
		private static readonly Color Accent = new Color32(255, 210, 63, 255);

		private static readonly string[] Grades = { "F", "D", "C", "B", "A", "A+" };

		private static readonly string[] StatLabels =
		{
			"Answers copied",
			"Time",
			"Meows",
			"Almost caught",
			"Ducks thrown"
		};

		[MenuItem("COPYCAT/Art/Build B5 Result Window")]
		public static void Build()
		{
			if (EditorApplication.isPlaying)
				throw new InvalidOperationException("Stop Play Mode before building the result window.");

			GameObject root = PrefabUtility.LoadPrefabContents(Prefab);
			try
			{
				while (root.transform.childCount > 0)
					Object.DestroyImmediate(root.transform.GetChild(0).gameObject);

				Image backdrop = root.GetComponent<Image>();
				if (backdrop == null)
					backdrop = root.AddComponent<Image>();
				backdrop.sprite = null;
				backdrop.color = new Color32(28, 24, 20, 168);
				backdrop.raycastTarget = true;

				RectTransform layout = Rectangle(root.transform, "Layout", new Rect(0f, 0f, 1920f, 1080f));
				layout.anchorMin = layout.anchorMax = layout.pivot = Vector2.one * 0.5f;
				layout.anchoredPosition = Vector2.zero;

				RectTransform content = Rectangle(layout, "Content", new Rect(0f, 0f, 1920f, 1080f));
				content.anchorMin = content.anchorMax = content.pivot = Vector2.one * 0.5f;
				content.anchoredPosition = Vector2.zero;

				Picture(content, "UI/Copycat/panel_paper_9slice", new Rect(185f, 80f, 1550f, 928f));

				TMP_Text title = Heading(content, string.Empty, new Rect(285f, 112f, 1350f, 100f));
				title.fontSize = 76f;
				TMP_Text subtitle = Label(content, string.Empty, new Rect(285f, 214f, 1350f, 54f));
				subtitle.font = Handwriting();
				subtitle.fontSize = 38f;
				subtitle.color = PencilInk;

				Divider(content, new Rect(270f, 284f, 1380f, 3f));
				Divider(content, new Rect(1017f, 296f, 3f, 476f));

				Image gradeStamp = Picture(content, "UI/Copycat/stamp_grade_A+", new Rect(300f, 350f, 220f, 220f));
				gradeStamp.name = "GradeStamp";

				Image[] stars = new Image[3];
				for (int index = 0; index < stars.Length; index++)
					stars[index] = Picture(content, "UI/Copycat/star_empty",
						new Rect(254f + 104f * index, 590f, 96f, 96f));

				TMP_Text gradeMessage = Label(content, string.Empty, new Rect(250f, 696f, 740f, 60f));
				gradeMessage.font = Handwriting();
				gradeMessage.fontSize = 34f;
				gradeMessage.color = PencilInk;

				TMP_Text[] statValues = new TMP_Text[StatLabels.Length];
				for (int index = 0; index < StatLabels.Length; index++)
				{
					float top = 358f + 58f * index;
					TMP_Text statLabel = Label(content, StatLabels[index], new Rect(590f, top, 260f, 50f));
					statLabel.fontSize = 34f;
					statLabel.alignment = TextAlignmentOptions.Left;
					TMP_Text statValue = Label(content, string.Empty, new Rect(860f, top, 120f, 50f));
					statValue.fontSize = 36f;
					statValue.alignment = TextAlignmentOptions.Right;
					statValues[index] = statValue;
				}

				Heading(content, "TOP COPYCATS", new Rect(1070f, 292f, 535f, 56f)).fontSize = 40f;
				Column(content, "#", new Rect(1072f, 358f, 40f, 30f), TextAlignmentOptions.Center);
				Column(content, "NAME", new Rect(1120f, 358f, 200f, 30f), TextAlignmentOptions.Left);
				Column(content, "ANSWERS", new Rect(1320f, 358f, 120f, 30f), TextAlignmentOptions.Center);
				Column(content, "TIME", new Rect(1445f, 358f, 80f, 30f), TextAlignmentOptions.Center);
				Column(content, "GRADE", new Rect(1530f, 358f, 80f, 30f), TextAlignmentOptions.Center);

				ResultLeaderboardRow[] rows = new ResultLeaderboardRow[LeaderboardRowCount];
				for (int index = 0; index < rows.Length; index++)
					rows[index] = Row(content, 392f + 30f * index);

				TMP_Text status = Label(content, string.Empty, new Rect(1070f, 470f, 535f, 60f));
				status.name = "Status";
				status.fontSize = 32f;
				TMP_Text ownRank = Label(content, string.Empty, new Rect(1070f, 688f, 535f, 40f));
				ownRank.name = "OwnRank";
				ownRank.fontSize = 28f;
				TMP_Text personalBest = Label(content, string.Empty, new Rect(1070f, 726f, 535f, 40f));
				personalBest.name = "PersonalBest";
				personalBest.fontSize = 28f;

				Button retake = Button(content, "RETAKE EXAM [R]", new Rect(310f, 820f, 600f, 108f));
				Button menu = Button(content, "MAIN MENU", new Rect(1040f, 820f, 540f, 108f));

				SerializedObject window = new(root.GetComponent<ResultWindow>());
				Assign(window, "layout", layout);
				Assign(window, "content", content);
				Assign(window, "title", title);
				Assign(window, "subtitle", subtitle);
				Assign(window, "gradeStamp", gradeStamp);
				AssignArray(window, "gradeStamps", GradeStamps());
				AssignArray(window, "stars", stars);
				Assign(window, "starFilled", Sprite("star_filled"));
				Assign(window, "starEmpty", Sprite("star_empty"));
				Assign(window, "gradeMessage", gradeMessage);
				AssignArray(window, "statValues", statValues);
				AssignArray(window, "leaderboardRows", rows);
				Assign(window, "leaderboardStatus", status);
				Assign(window, "ownRank", ownRank);
				Assign(window, "personalBest", personalBest);
				Assign(window, "retakeButton", retake);
				Assign(window, "menuButton", menu);
				window.ApplyModifiedPropertiesWithoutUndo();
				PrefabUtility.SaveAsPrefabAsset(root, Prefab);
			}
			finally
			{
				PrefabUtility.UnloadPrefabContents(root);
			}

			AssetDatabase.SaveAssets();
		}

		private static ResultLeaderboardRow Row(Transform parent, float top)
		{
			RectTransform row = Rectangle(parent, "Row", new Rect(1062f, top, 552f, 30f));
			Image highlight = Rectangle(row, "Highlight", new Rect(0f, 2f, 552f, 28f))
				.gameObject.AddComponent<Image>();
			highlight.color = Accent;
			highlight.raycastTarget = false;
			highlight.enabled = false;

			ResultLeaderboardRow view = row.gameObject.AddComponent<ResultLeaderboardRow>();
			SerializedObject serialized = new(view);
			Assign(serialized, "highlight", highlight);
			Assign(serialized, "rank", Cell(row, new Rect(10f, 0f, 40f, 30f), TextAlignmentOptions.Center));
			Assign(serialized, "playerName", Cell(row, new Rect(58f, 0f, 200f, 30f), TextAlignmentOptions.Left));
			Assign(serialized, "answers", Cell(row, new Rect(258f, 0f, 120f, 30f), TextAlignmentOptions.Center));
			Assign(serialized, "time", Cell(row, new Rect(383f, 0f, 80f, 30f), TextAlignmentOptions.Center));
			Assign(serialized, "grade", Cell(row, new Rect(468f, 0f, 80f, 30f), TextAlignmentOptions.Center));
			serialized.ApplyModifiedPropertiesWithoutUndo();
			row.gameObject.SetActive(false);

			return view;
		}

		private static TMP_Text Cell(Transform parent, Rect rectangle, TextAlignmentOptions alignment)
		{
			TMP_Text cell = Label(parent, string.Empty, rectangle);
			cell.fontSize = 26f;
			cell.alignment = alignment;
			cell.textWrappingMode = TextWrappingModes.NoWrap;

			return cell;
		}

		private static TMP_Text Column(Transform parent, string text, Rect rectangle, TextAlignmentOptions alignment)
		{
			TMP_Text column = Label(parent, text, rectangle);
			column.fontSize = 24f;
			column.alignment = alignment;
			column.textWrappingMode = TextWrappingModes.NoWrap;

			return column;
		}

		private static TMP_Text Heading(Transform parent, string text, Rect rectangle)
		{
			TMP_Text heading = Label(parent, text, rectangle);
			heading.font = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(
				Content + "CopycatShared/Fonts/LuckiestGuy-SDF.asset");
			heading.color = Ink;

			return heading;
		}

		private static TMP_FontAsset Handwriting()
		{
			return AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(
				Content + "CopycatShared/Fonts/PatrickHand-Regular-SDF.asset");
		}

		private static void Divider(Transform parent, Rect rectangle)
		{
			Image divider = Rectangle(parent, "Divider", rectangle).gameObject.AddComponent<Image>();
			divider.color = PaperShade;
			divider.raycastTarget = false;
		}

		private static Sprite[] GradeStamps()
		{
			Sprite[] stamps = new Sprite[Grades.Length];

			for (int index = 0; index < Grades.Length; index++)
				stamps[index] = Sprite("stamp_grade_" + Grades[index]);

			return stamps;
		}

		private static Sprite Sprite(string name)
		{
			return AssetDatabase.LoadAssetAtPath<Sprite>(Content + "UI/Copycat/" + name + ".png");
		}

		private static void AssignArray(SerializedObject target, string property, Object[] values)
		{
			SerializedProperty array = target.FindProperty(property);
			array.arraySize = values.Length;

			for (int index = 0; index < values.Length; index++)
				array.GetArrayElementAtIndex(index).objectReferenceValue = values[index];
		}
	}
}
