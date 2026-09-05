using System.Collections.Generic;
using System.Text;
using Code.Gameplay.Duck;
using Code.Gameplay.Exam;
using Code.Gameplay.Greybox.Data;
using Code.Gameplay.Neighbours;
using Code.Gameplay.Teacher;
using TMPro;
using UnityEngine;

namespace Code.Gameplay.Greybox.Behaviours
{
	public class GreyboxBoard : MonoBehaviour
	{
		private Sprite _blockSprite;
		private Sprite _leftAnchoredSprite;
		private Sprite _bottomAnchoredSprite;

		private SpriteRenderer _teacherBody;
		private SpriteRenderer _teacherEyes;
		private TextMeshPro _teacherLabel;

		private SpriteRenderer _suspicionFill;
		private TextMeshPro _suspicionLabel;

		private SpriteRenderer _meowFill;
		private SpriteRenderer _meowThreshold;

		private readonly SpriteRenderer[] _paws = new SpriteRenderer[2];
		private readonly SpriteRenderer[] _pawWindows = new SpriteRenderer[2];
		private readonly TextMeshPro[] _rows = new TextMeshPro[2];

		private SpriteRenderer _duck;
		private TextMeshPro _duckLabel;

		private SpriteRenderer _kitten;
		private TextMeshPro _questionLabel;
		private TextMeshPro _slipLabel;
		private TextMeshPro _copiedLabel;

		private TextMeshPro _progressLabel;
		private TextMeshPro _timeLabel;
		private TextMeshPro _statusLabel;

		private readonly StringBuilder _rowBuilder = new(64);

		private float _leanTargetX;

		private const float TeacherY = 5f;
		private const float SuspicionY = 3.1f;
		private const float SuspicionWidth = 10f;
		private const float NeighbourX = 8f;
		private const float NeighbourBodyY = 2.8f;
		private const float NeighbourPaperY = 0.2f;
		private const float PawLiftedY = 2.8f;
		private const float PawWindowY = 4.1f;
		private const float RowWidth = 5.4f;
		private const float RowHeight = 2.4f;
		private const float MeowX = -11.3f;
		private const float MeowBottomY = -3.5f;
		private const float MeowHeight = 5f;
		private const float MeowWidth = 1f;
		private const float OwnPaperY = -4f;
		private const float KittenY = -5.8f;
		private const float DuckX = 5.8f;
		private const float DuckSize = 0.9f;
		private const float LeanOffsetX = 2.6f;
		private const float BackdropDepth = 5f;
		private const float PaperDepth = -1f;
		private const float PawDepth = -2f;
		private const float OverlayDepth = -3f;

		private void Awake()
		{
			CreateSprites();

			BuildBackdrop();
			BuildTeacher();
			BuildSuspicion();
			BuildMeow();
			BuildNeighbour(NeighbourSide.Left);
			BuildNeighbour(NeighbourSide.Right);
			BuildOwnDesk();
			BuildOverlay();
		}

		private void CreateSprites()
		{
			_blockSprite = CreateSprite(new Vector2(0.5f, 0.5f));
			_leftAnchoredSprite = CreateSprite(new Vector2(0f, 0.5f));
			_bottomAnchoredSprite = CreateSprite(new Vector2(0.5f, 0f));
		}

		private void BuildBackdrop()
		{
			CreateBlock(new Vector3(0f, 0f, BackdropDepth), new Vector2(26f, 15f), GreyboxColors.Wall);
		}

		private void BuildTeacher()
		{
			_teacherBody = CreateBlock(
				new Vector3(0f, TeacherY, 0f),
				new Vector2(3.2f, 1.8f),
				GreyboxColors.Body);

			_teacherEyes = CreateBlock(
				new Vector3(0f, TeacherY + 0.6f, PaperDepth),
				new Vector2(2.2f, 0.35f),
				GreyboxColors.Eyes);

			_teacherLabel = CreateLabel(new Vector3(0f, TeacherY + 1.5f, OverlayDepth), 7f, TextAlignmentOptions.Center);
			_teacherLabel.color = GreyboxColors.Chalk;
		}

		private void BuildSuspicion()
		{
			CreateBlock(
				new Vector3(0f, SuspicionY, 0f),
				new Vector2(SuspicionWidth, 0.7f),
				GreyboxColors.Slot);

			_suspicionFill = CreateLeftFill(
				new Vector3(-SuspicionWidth * 0.5f, SuspicionY, PaperDepth),
				new Vector2(SuspicionWidth, 0.7f),
				GreyboxColors.SuspicionLow);

			_suspicionLabel = CreateLabel(new Vector3(0f, SuspicionY + 0.7f, OverlayDepth), 6f, TextAlignmentOptions.Center);
			_suspicionLabel.color = GreyboxColors.Chalk;
		}

		private void BuildMeow()
		{
			CreateBlock(
				new Vector3(MeowX, MeowBottomY + MeowHeight * 0.5f, 0f),
				new Vector2(MeowWidth, MeowHeight),
				GreyboxColors.Slot);

			_meowFill = CreateBottomFill(
				new Vector3(MeowX, MeowBottomY, PaperDepth),
				new Vector2(MeowWidth, MeowHeight),
				GreyboxColors.MeowArmed);

			_meowThreshold = CreateBlock(
				new Vector3(MeowX, MeowBottomY, PawDepth),
				new Vector2(MeowWidth + 0.4f, 0.12f),
				GreyboxColors.Threshold);

			TextMeshPro meowLabel = CreateLabel(
				new Vector3(MeowX, MeowBottomY - 0.7f, OverlayDepth),
				6f,
				TextAlignmentOptions.Center);

			meowLabel.color = GreyboxColors.Chalk;
			meowLabel.text = "MEOW";
		}

		private void BuildNeighbour(NeighbourSide side)
		{
			float x = SideX(side);
			int index = IndexOf(side);

			CreateBlock(new Vector3(x, NeighbourBodyY, 0f), new Vector2(3f, 2f), GreyboxColors.Body);
			CreateBlock(new Vector3(x, NeighbourPaperY, 0f), new Vector2(RowWidth, RowHeight), GreyboxColors.Paper);

			_rows[index] = CreateLabel(new Vector3(x, NeighbourPaperY, PaperDepth), 5f, TextAlignmentOptions.Center);
			_rows[index].rectTransform.sizeDelta = new Vector2(RowWidth - 0.2f, RowHeight - 0.2f);

			_pawWindows[index] = CreateLeftFill(
				new Vector3(x - RowWidth * 0.5f, PawWindowY, PaperDepth),
				new Vector2(RowWidth, 0.25f),
				GreyboxColors.PawWindow);

			_paws[index] = CreateBlock(
				new Vector3(x, NeighbourPaperY, PawDepth),
				new Vector2(RowWidth, RowHeight),
				GreyboxColors.Paw);
		}

		private void BuildOwnDesk()
		{
			CreateBlock(new Vector3(0f, OwnPaperY, 0f), new Vector2(9f, 2.2f), GreyboxColors.Paper);

			_questionLabel = CreateLabel(new Vector3(0f, OwnPaperY + 1.9f, OverlayDepth), 6f, TextAlignmentOptions.Center);
			_questionLabel.color = GreyboxColors.Chalk;

			_slipLabel = CreateLabel(new Vector3(0f, OwnPaperY + 0.4f, PaperDepth), 7f, TextAlignmentOptions.Center);
			_slipLabel.rectTransform.sizeDelta = new Vector2(8.6f, 1f);

			_copiedLabel = CreateLabel(new Vector3(0f, OwnPaperY - 0.6f, PaperDepth), 9f, TextAlignmentOptions.Center);
			_copiedLabel.color = GreyboxColors.SuspicionHigh;

			_kitten = CreateBlock(new Vector3(0f, KittenY, 0f), new Vector2(1.6f, 1.4f), GreyboxColors.Kitten);

			_duck = CreateBlock(
				new Vector3(DuckX, OwnPaperY, PawDepth),
				new Vector2(DuckSize, DuckSize),
				GreyboxColors.Duck);

			_duckLabel = CreateLabel(new Vector3(DuckX, OwnPaperY - 0.9f, OverlayDepth), 5f, TextAlignmentOptions.Center);
			_duckLabel.color = GreyboxColors.Chalk;
			_duckLabel.rectTransform.sizeDelta = new Vector2(6f, 1f);
		}

		private void BuildOverlay()
		{
			_progressLabel = CreateLabel(new Vector3(-7.3f, 6.6f, OverlayDepth), 8f, TextAlignmentOptions.Left);
			_progressLabel.color = GreyboxColors.Chalk;
			_progressLabel.rectTransform.sizeDelta = new Vector2(10f, 1f);

			_timeLabel = CreateLabel(new Vector3(-7.3f, 5.8f, OverlayDepth), 8f, TextAlignmentOptions.Left);
			_timeLabel.color = GreyboxColors.Chalk;
			_timeLabel.rectTransform.sizeDelta = new Vector2(10f, 1f);

			_statusLabel = CreateLabel(new Vector3(0f, -1.4f, OverlayDepth), 11f, TextAlignmentOptions.Center);
			_statusLabel.color = GreyboxColors.SuspicionHigh;
			_statusLabel.rectTransform.sizeDelta = new Vector2(24f, 2f);

			TextMeshPro controlsLabel = CreateLabel(new Vector3(0f, -6.75f, OverlayDepth), 4.5f, TextAlignmentOptions.Center);
			controlsLabel.color = GreyboxColors.Chalk;
			controlsLabel.rectTransform.sizeDelta = new Vector2(24f, 1f);
			controlsLabel.text = "SPACE lean - M meow - Q duck - arrows/WASD strokes - 1-4 pick - A-Z word - ESC retake";
		}

		public void SetTeacher(TeacherAttention attention, bool facingClass, int almostCaught)
		{
			_teacherBody.color = GreyboxColors.OfAttention(attention);
			_teacherEyes.enabled = facingClass;
			_teacherLabel.text = $"{attention} / ALMOST CAUGHT {almostCaught}";
		}

		public void SetSuspicion(float level, float maximumLevel)
		{
			float ratio = Mathf.Clamp01(level / maximumLevel);

			_suspicionFill.transform.localScale = new Vector3(SuspicionWidth * ratio, 0.7f, 1f);
			_suspicionFill.color = GreyboxColors.OfSuspicion(ratio);
			_suspicionLabel.text = $"SUSPICION {level:0}";
		}

		public void SetMeow(float level, float thresholdLevel, bool armed)
		{
			float ratio = Mathf.Clamp01(level * 0.01f);

			_meowFill.transform.localScale = new Vector3(MeowWidth, MeowHeight * ratio, 1f);
			_meowFill.color = armed ? GreyboxColors.MeowArmed : GreyboxColors.MeowSpent;
			_meowThreshold.transform.localPosition = new Vector3(
				MeowX,
				MeowBottomY + MeowHeight * Mathf.Clamp01(thresholdLevel * 0.01f),
				PawDepth);
		}

		public void SetPaw(NeighbourSide side, bool lifted, float windowRatio)
		{
			int index = IndexOf(side);
			float x = SideX(side);

			_paws[index].transform.localPosition = new Vector3(x, lifted ? PawLiftedY : NeighbourPaperY, PawDepth);
			_pawWindows[index].transform.localScale = new Vector3(RowWidth * Mathf.Clamp01(windowRatio), 0.25f, 1f);
		}

		public void SetDuck(DuckState state, float timeLeft, int throwCount)
		{
			_duck.transform.localPosition = DuckPositionOf(state);
			_duck.color = state == DuckState.OnDesk ? GreyboxColors.Duck : GreyboxColors.DuckAway;
			_duckLabel.text = DuckTextOf(state, timeLeft, throwCount);
		}

		public void SetProgress(int answersCopied, int totalQuestions, float elapsedSeconds)
		{
			_progressLabel.text = $"ANSWERS {answersCopied} / {totalQuestions}";
			_timeLabel.text = $"TIME {elapsedSeconds:0.0}";
		}

		public void SetQuestion(int questionIndex, QuestionType type, NeighbourSide side)
		{
			_slipLabel.text = $"Q{questionIndex + 1} / {type} / ANSWER FROM {side}".ToUpperInvariant();
			_leanTargetX = side == NeighbourSide.Left ? -LeanOffsetX : LeanOffsetX;
		}

		public void SetQuestionText(string text)
		{
			_questionLabel.text = text;
		}

		public void SetCopied(bool copied)
		{
			_copiedLabel.text = copied ? "COPIED" : string.Empty;
		}

		public void SetLean(bool leaning)
		{
			_kitten.transform.localPosition = new Vector3(leaning ? _leanTargetX : 0f, KittenY, 0f);
			_kitten.color = leaning ? GreyboxColors.KittenLeaning : GreyboxColors.Kitten;
		}

		public void SetOutcome(ExamOutcome outcome)
		{
			_statusLabel.text = outcome == ExamOutcome.None ? string.Empty : $"{outcome} - ESC TO RETAKE".ToUpperInvariant();
		}

		public void SetStrokeRow(NeighbourSide side, IReadOnlyList<StrokeDirection> strokes, int progress)
		{
			_rowBuilder.Clear();

			for (int index = 0; index < strokes.Count; index++)
			{
				AppendCell(GlyphOf(strokes[index]), index, progress);
			}

			ShowRow(side, _rowBuilder.ToString());
		}

		public void SetPickRow(NeighbourSide side, IReadOnlyList<string> options, int correctOptionIndex)
		{
			_rowBuilder.Clear();

			for (int index = 0; index < options.Count; index++)
			{
				AppendOption($"{index + 1}:{options[index]}", index == correctOptionIndex);
			}

			ShowRow(side, _rowBuilder.ToString());
		}

		public void SetWordRow(NeighbourSide side, string word, int progress)
		{
			_rowBuilder.Clear();

			for (int index = 0; index < word.Length; index++)
			{
				AppendCell(word[index].ToString(), index, progress);
			}

			ShowRow(side, _rowBuilder.ToString());
		}

		private void AppendOption(string option, bool circled)
		{
			if (circled)
				_rowBuilder.Append($"<color={GreyboxColors.NextInk}>({option})</color> ");
			else
				_rowBuilder.Append($"<color={GreyboxColors.PendingInk}>{option}</color> ");
		}

		private void AppendCell(string cell, int index, int progress)
		{
			if (index < progress)
				_rowBuilder.Append($"<color={GreyboxColors.DoneInk}>{cell}</color> ");
			else if (index == progress)
				_rowBuilder.Append($"<color={GreyboxColors.NextInk}>[{cell}]</color> ");
			else
				_rowBuilder.Append($"<color={GreyboxColors.PendingInk}>{cell}</color> ");
		}

		private void ShowRow(NeighbourSide side, string row)
		{
			_rows[IndexOf(side)].text = row;
			_rows[OppositeIndexOf(side)].text = string.Empty;
		}

		private SpriteRenderer CreateBlock(Vector3 position, Vector2 size, Color color)
		{
			SpriteRenderer renderer = CreateRenderer(_blockSprite, position, size);
			renderer.color = color;

			return renderer;
		}

		private SpriteRenderer CreateLeftFill(Vector3 position, Vector2 size, Color color)
		{
			SpriteRenderer renderer = CreateRenderer(_leftAnchoredSprite, position, size);
			renderer.color = color;

			return renderer;
		}

		private SpriteRenderer CreateBottomFill(Vector3 position, Vector2 size, Color color)
		{
			SpriteRenderer renderer = CreateRenderer(_bottomAnchoredSprite, position, size);
			renderer.color = color;

			return renderer;
		}

		private SpriteRenderer CreateRenderer(Sprite sprite, Vector3 position, Vector2 size)
		{
			GameObject holder = new("Greybox Block");
			holder.transform.SetParent(transform);
			holder.transform.localPosition = position;
			holder.transform.localScale = new Vector3(size.x, size.y, 1f);

			SpriteRenderer renderer = holder.AddComponent<SpriteRenderer>();
			renderer.sprite = sprite;

			return renderer;
		}

		private TextMeshPro CreateLabel(Vector3 position, float fontSize, TextAlignmentOptions alignment)
		{
			GameObject holder = new("Greybox Label");
			holder.transform.SetParent(transform);
			holder.transform.localPosition = position;

			TextMeshPro label = holder.AddComponent<TextMeshPro>();
			label.fontSize = fontSize;
			label.alignment = alignment;
			label.color = GreyboxColors.Ink;
			label.rectTransform.sizeDelta = new Vector2(14f, 1.6f);

			return label;
		}

		private static Sprite CreateSprite(Vector2 pivot)
		{
			Texture2D texture = new(1, 1);
			texture.SetPixel(0, 0, Color.white);
			texture.Apply();

			return Sprite.Create(texture, new Rect(0f, 0f, 1f, 1f), pivot, 1f);
		}

		private static Vector3 DuckPositionOf(DuckState state)
		{
			return state switch
			{
				DuckState.Flying => new Vector3(3f, 0.6f, PawDepth),
				DuckState.OnFloor => new Vector3(-3.6f, 3.7f, PawDepth),
				DuckState.Carried => new Vector3(-1.3f, 4.6f, PawDepth),
				DuckState.Confiscated => new Vector3(3.4f, 5.2f, PawDepth),
				_ => new Vector3(DuckX, OwnPaperY, PawDepth)
			};
		}

		private static string DuckTextOf(DuckState state, float timeLeft, int throwCount)
		{
			if (state == DuckState.OnDesk)
				return $"DUCK [Q] - THROWN {throwCount}";

			if (state == DuckState.Confiscated)
				return $"DUCK CONFISCATED - THROWN {throwCount}";

			return $"DUCK {state} {timeLeft:0.0} - THROWN {throwCount}".ToUpperInvariant();
		}

		private static string GlyphOf(StrokeDirection direction)
		{
			return direction switch
			{
				StrokeDirection.Up => "^",
				StrokeDirection.Right => ">",
				StrokeDirection.Down => "v",
				StrokeDirection.Left => "<",
				_ => "?"
			};
		}

		private static float SideX(NeighbourSide side)
		{
			return side == NeighbourSide.Left ? -NeighbourX : NeighbourX;
		}

		private static int IndexOf(NeighbourSide side)
		{
			return side == NeighbourSide.Left ? 0 : 1;
		}

		private static int OppositeIndexOf(NeighbourSide side)
		{
			return side == NeighbourSide.Left ? 1 : 0;
		}
	}
}
