using System.Text;
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
			public GameObject PickCircle;
		}

		private readonly StringBuilder _answerBuilder = new();

		private IExamQuery _examQuery;
		private TMP_Text _question;
		private TMP_Text _answer;
		private NeighbourPaper[] _neighbourPapers;

		public void Bind(IExamQuery examQuery)
		{
			Unbind();
			_examQuery = examQuery;
			FindPapers();
			_examQuery.OnCurrentQuestionChanged += HandleQuestionChanged;
			_examQuery.OnAnswerReadableChanged += HandleAnswerReadableChanged;
			_examQuery.OnAnswerProgressChanged += HandleAnswerProgressChanged;
			Refresh();
		}

		public void Unbind()
		{
			if (_examQuery != null)
			{
				_examQuery.OnCurrentQuestionChanged -= HandleQuestionChanged;
				_examQuery.OnAnswerReadableChanged -= HandleAnswerReadableChanged;
				_examQuery.OnAnswerProgressChanged -= HandleAnswerProgressChanged;
			}

			_examQuery = null;
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
			}

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
					PickCircle = FindTransform(transform, "PickCircle")?.gameObject
				};
				_neighbourPapers[paperIndex++] = paper;
			}
		}

		private void Refresh()
		{
			if (_examQuery == null || _neighbourPapers == null)
				return;

			QuestionDefinition question = _examQuery.GetCurrentQuestion();
			if (question == null)
				return;

			if (_question != null)
				_question.text = question.Text;

			if (_answer != null)
				_answer.text = "____";

			foreach (NeighbourPaper paper in _neighbourPapers)
				SetPaper(paper, question, _examQuery.IsAnswerReadable() && paper.Side == question.Neighbour);
		}

		private void SetPaper(NeighbourPaper paper, QuestionDefinition question, bool visible)
		{
			if (paper == null)
				return;

			string answer = visible ? AnswerOf(question) : string.Empty;
			if (paper.Word != null)
			{
				paper.Word.text = question.Type switch
				{
					QuestionType.Pick when visible => $"CIRCLED: {question.CorrectOptionIndex + 1}",
					QuestionType.Pick => string.Empty,
					_ => answer
				};
			}

			for (int index = 0; index < paper.Picks.Length; index++)
			{
				if (paper.Picks[index] != null)
					paper.Picks[index].text = visible && question.Type == QuestionType.Pick
						? index == question.CorrectOptionIndex ? $"[{question.Options[index]}]" : question.Options[index]
						: string.Empty;
			}

			if (paper.PickCircle != null)
				paper.PickCircle.SetActive(visible && question.Type == QuestionType.Pick);
		}

		private string AnswerOf(QuestionDefinition question)
		{
			if (question.Type == QuestionType.Word)
				return question.Word;

			if (question.Type == QuestionType.Pick)
				return string.Empty;

			_answerBuilder.Clear();
			foreach (StrokeDirection stroke in question.Strokes)
			{
				if (_answerBuilder.Length > 0)
					_answerBuilder.Append(' ');

				_answerBuilder.Append(stroke switch
				{
					StrokeDirection.Up => "[↑]",
					StrokeDirection.Right => "[→]",
					StrokeDirection.Down => "[↓]",
					_ => "[←]"
				});
			}

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

		private void HandleQuestionChanged(int questionIndex) => Refresh();
		private void HandleAnswerReadableChanged(int questionIndex, bool readable) => Refresh();
		private void HandleAnswerProgressChanged(int questionIndex, int progress, int length) => Refresh();
	}
}
