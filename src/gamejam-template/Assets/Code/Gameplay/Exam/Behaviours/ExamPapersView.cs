using System.Text;
using Code.Gameplay.Difficulty.Data;
using Code.Gameplay.Difficulty.Services;
using Code.Gameplay.Exam.Data;
using Code.Gameplay.Exam.Queries;
using Code.Gameplay.Neighbours;
using TMPro;
using UnityEngine;

namespace Code.Gameplay.Exam.Behaviours
{
	public sealed class ExamPapersView : MonoBehaviour
	{
		private sealed class NeighbourPaper
		{
			public NeighbourSide Side;
			public TMP_Text Word;
			public TMP_Text[] Picks;
			public Transform PickCircle;
			public PaperGlyphRow Strokes;
		}

		private readonly StringBuilder _answerBuilder = new();

		private TMP_Text _question;
		private TMP_Text _answer;
		private PaperGlyphRow _answerGlyphs;
		private GameObject _stamp;
		private NeighbourPaper[] _neighbourPapers;
		private Color _answerColor;
		private float _wrongSecondsLeft;
		private int _wrongIndex = -1;

		private IExamQuery _examQuery;
		private IDifficultyService _difficultyService;

		private static readonly Color DangerColor = new Color32(232, 76, 76, 255);

		private const string TypedColorTag = "<color=#4FCB7A>";
		private const string ColorEndTag = "</color>";
		private const string BlankAnswer = "____";
		private const string PickLetters = "ABCD";
		private const float WrongSeconds = 0.4f;

		public void Bind(IExamQuery examQuery, IDifficultyService difficultyService)
		{
			Unbind();
			_examQuery = examQuery;
			_difficultyService = difficultyService;
			FindPapers();
			_examQuery.OnCurrentQuestionChanged += HandleQuestionChanged;
			_examQuery.OnAnswerReadableChanged += HandleAnswerReadableChanged;
			_examQuery.OnAnswerProgressChanged += HandleAnswerProgressChanged;
			_examQuery.OnAnswerCopied += HandleAnswerCopied;
			_examQuery.OnWrongInput += HandleWrongInput;
			Refresh();
		}

		public void Unbind()
		{
			if (_examQuery != null)
			{
				_examQuery.OnCurrentQuestionChanged -= HandleQuestionChanged;
				_examQuery.OnAnswerReadableChanged -= HandleAnswerReadableChanged;
				_examQuery.OnAnswerProgressChanged -= HandleAnswerProgressChanged;
				_examQuery.OnAnswerCopied -= HandleAnswerCopied;
				_examQuery.OnWrongInput -= HandleWrongInput;
			}

			_examQuery = null;
			_difficultyService = null;
		}

		private void OnDestroy() => Unbind();

		private void FindPapers()
		{
			Transform playerPaper = null;
			Transform[] transforms = FindObjectsByType<Transform>(FindObjectsInactive.Include, FindObjectsSortMode.None);

			foreach (Transform transform in transforms)
			{
				if (transform.name == "PlayerPaper")
					playerPaper = transform;
			}

			if (playerPaper != null)
			{
				_question = FindText(playerPaper, "question");
				_answer = FindText(playerPaper, "Answer");
				_answerGlyphs = FindTransform(playerPaper, "AnswerGlyphs")?.GetComponent<PaperGlyphRow>();
				_stamp = FindTransform(playerPaper, "stamp_copied")?.gameObject;
			}

			if (_answer != null)
				_answerColor = _answer.color;

			_neighbourPapers = new NeighbourPaper[2];
			int paperIndex = 0;
			foreach (Transform transform in transforms)
			{
				if (transform.name != "NeighbourPaper" || paperIndex >= _neighbourPapers.Length)
					continue;

				NeighbourPaper paper = new()
				{
					Side = transform.position.x < 0f ? NeighbourSide.Left : NeighbourSide.Right,
					Word = FindText(transform, "Word"),
					Picks = new[]
					{
						FindText(transform, "Pick1"), FindText(transform, "Pick2"),
						FindText(transform, "Pick3"), FindText(transform, "Pick4")
					},
					PickCircle = FindTransform(transform, "glyph_pick_circle"),
					Strokes = FindTransform(transform, "StrokeGlyphs")?.GetComponent<PaperGlyphRow>()
				};
				_neighbourPapers[paperIndex++] = paper;
			}
		}

		private void Update()
		{
			if (_wrongSecondsLeft <= 0f)
				return;

			_wrongSecondsLeft -= Time.deltaTime;

			if (_wrongSecondsLeft <= 0f)
			{
				_wrongIndex = -1;
				Refresh();
			}
		}

		private void Refresh()
		{
			if (_examQuery == null || _neighbourPapers == null)
				return;

			QuestionDefinition question = _examQuery.GetCurrentQuestion();
			if (question == null)
				return;

			int progress = _examQuery.GetAnswerProgress();
			bool copied = _examQuery.IsAnswerCopied();
			RefreshPlayerPaper(question, progress, copied);

			foreach (NeighbourPaper paper in _neighbourPapers)
				SetPaper(paper, question, _examQuery.IsAnswerReadable() && paper.Side == question.Neighbour);
		}

		private void RefreshPlayerPaper(QuestionDefinition question, int progress, bool copied)
		{
			if (_question != null)
				_question.text = question.Text;

			if (_stamp != null)
				_stamp.SetActive(copied);

			if (_answer != null)
			{
				_answer.color = _wrongIndex >= 0 ? DangerColor : _answerColor;
				_answer.text = PlayerAnswerOf(question, progress);
			}

			if (_answerGlyphs == null)
				return;

			if (question.Type == QuestionType.Strokes)
				_answerGlyphs.ShowTypedStrokes(question.Strokes, progress);
			else
				_answerGlyphs.Clear();
		}

		private string PlayerAnswerOf(QuestionDefinition question, int progress)
		{
			if (progress <= 0)
				return BlankAnswer;

			if (question.Type == QuestionType.Word)
				return SpacedWord(question.Word.Substring(0, Mathf.Min(progress, question.Word.Length)), 0);

			if (question.Type == QuestionType.Pick)
				return question.Options[question.CorrectOptionIndex];

			return string.Empty;
		}

		private void SetPaper(NeighbourPaper paper, QuestionDefinition question, bool visible)
		{
			if (paper == null)
				return;

			if (paper.Word != null)
				paper.Word.text = visible && question.Type == QuestionType.Word
					? SpacedWord(question.Word, _examQuery.GetAnswerProgress())
					: string.Empty;

			for (int index = 0; index < paper.Picks.Length; index++)
			{
				if (paper.Picks[index] != null)
					paper.Picks[index].text = visible && question.Type == QuestionType.Pick
						? $"{PickLetters[index]}  {question.Options[index]}"
						: string.Empty;
			}

			SetPickCircle(paper, question, visible);

			if (paper.Strokes == null)
				return;

			if (visible && question.Type == QuestionType.Strokes)
				paper.Strokes.ShowStrokes(question.Strokes, _examQuery.GetAnswerProgress(), _wrongIndex);
			else
				paper.Strokes.Clear();
		}

		private void SetPickCircle(NeighbourPaper paper, QuestionDefinition question, bool visible)
		{
			if (paper.PickCircle == null)
				return;

			bool circled = visible && question.Type == QuestionType.Pick && _wrongIndex < 0;
			paper.PickCircle.gameObject.SetActive(circled);

			if (circled == false)
				return;

			TMP_Text cell = paper.Picks[question.CorrectOptionIndex];
			if (cell != null)
				paper.PickCircle.localPosition = cell.transform.localPosition;

			SpriteRenderer circle = paper.PickCircle.GetComponent<SpriteRenderer>();
			DifficultyPhase phase = _difficultyService.GetPhase(_examQuery.GetCurrentQuestionIndex());
			Color color = circle.color;
			color.a = phase.FaintPickCircle ? 0.35f : 1f;
			circle.color = color;
		}

		private string SpacedWord(string word, int typed)
		{
			_answerBuilder.Clear();

			for (int index = 0; index < word.Length; index++)
			{
				if (index == 0 && typed > 0)
					_answerBuilder.Append(TypedColorTag);

				if (index == typed && typed > 0)
					_answerBuilder.Append(ColorEndTag);

				if (index > 0)
					_answerBuilder.Append(' ');

				_answerBuilder.Append(word[index]);
			}

			if (typed >= word.Length && typed > 0)
				_answerBuilder.Append(ColorEndTag);

			return _answerBuilder.ToString();
		}

		private static TMP_Text FindText(Transform parent, string name) => FindTransform(parent, name)?.GetComponent<TMP_Text>();

		private static Transform FindTransform(Transform parent, string name)
		{
			foreach (Transform transform in parent.GetComponentsInChildren<Transform>(true))
			{
				if (transform.name == name)
					return transform;
			}

			return null;
		}

		private void HandleQuestionChanged(int questionIndex)
		{
			_wrongIndex = -1;
			_wrongSecondsLeft = 0f;
			Refresh();
		}

		private void HandleAnswerReadableChanged(int questionIndex, bool readable) => Refresh();
		private void HandleAnswerProgressChanged(int questionIndex, int progress, int length) => Refresh();
		private void HandleAnswerCopied(int questionIndex) => Refresh();

		private void HandleWrongInput(int questionIndex)
		{
			_wrongIndex = _examQuery.GetAnswerProgress();
			_wrongSecondsLeft = WrongSeconds;
			Refresh();
		}
	}
}
