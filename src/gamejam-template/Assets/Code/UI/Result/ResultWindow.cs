using System.Threading;
using Code.Gameplay.Camera.Services;
using Code.Gameplay.CoreLoop.Services;
using Code.Gameplay.Duck.Queries;
using Code.Gameplay.Exam;
using Code.Gameplay.Exam.Queries;
using Code.Gameplay.Exam.Services;
using Code.Gameplay.Leaderboard.Data;
using Code.Gameplay.Leaderboard.Services;
using Code.Gameplay.Progress.Queries;
using Code.Gameplay.Teacher.Queries;
using Code.Infrastructure.CoreLoop;
using Code.Infrastructure.Input;
using Code.UI.Fade;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Framework.UI.UiManagement.Elements.Windows;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
using SF = UnityEngine.SerializeField;

namespace Code.UI.Result
{
	public class ResultWindow : WindowBase
	{
		[SF] private RectTransform layout;
		[SF] private RectTransform content;
		[SF] private TMP_Text title;
		[SF] private TMP_Text subtitle;
		[SF] private Image gradeStamp;
		[SF] private Sprite[] gradeStamps;
		[SF] private Image[] stars;
		[SF] private Sprite starFilled;
		[SF] private Sprite starEmpty;
		[SF] private TMP_Text gradeMessage;
		[SF] private TMP_Text[] statValues;
		[SF] private ResultLeaderboardRow[] leaderboardRows;
		[SF] private TMP_Text leaderboardStatus;
		[SF] private TMP_Text ownRank;
		[SF] private TMP_Text personalBest;
		[SF] private Button retakeButton;
		[SF] private Button menuButton;

		private bool _isOpen;
		private bool _isLeaving;
		private CancellationTokenSource _submitCts;

		private IExamQuery _exam;
		private ITeacherQuery _teacher;
		private IDuckQuery _duck;
		private IProgressQuery _progress;
		private IExamGradeService _examGradeService;
		private ILeaderboardService _leaderboardService;
		private IInputService _inputService;
		private ICoreLoopRequestFactory _coreLoopRequestFactory;
		private ICameraSwitcher _cameraSwitcher;

		private const float FadeInDuration = 0.3f;
		private const float ScaleDuration = 0.5f;

		[Inject]
		public void Construct(
			IExamQuery exam,
			ITeacherQuery teacher,
			IDuckQuery duck,
			IProgressQuery progress,
			IExamGradeService examGradeService,
			ILeaderboardService leaderboardService,
			IInputService inputService,
			ICoreLoopRequestFactory coreLoopRequestFactory,
			ICameraSwitcher cameraSwitcher)
		{
			_exam = exam;
			_teacher = teacher;
			_duck = duck;
			_progress = progress;
			_examGradeService = examGradeService;
			_leaderboardService = leaderboardService;
			_inputService = inputService;
			_coreLoopRequestFactory = coreLoopRequestFactory;
			_cameraSwitcher = cameraSwitcher;
		}

		private void OnRectTransformDimensionsChange()
		{
			if (layout == null)
				return;

			Rect bounds = ((RectTransform)transform).rect;
			layout.localScale = Vector3.one * Mathf.Min(bounds.width / 1920f, bounds.height / 1080f);
		}

		protected override UniTask OnOpen(CancellationToken cancellationToken = default)
		{
			_isOpen = true;
			_isLeaving = false;
			_submitCts = new CancellationTokenSource();
			content.localScale = Vector3.zero;
			retakeButton.interactable = true;
			menuButton.interactable = true;
			retakeButton.onClick.AddListener(HandleRetakeClicked);
			menuButton.onClick.AddListener(HandleMenuClicked);
			OnRectTransformDimensionsChange();
			ShowReportCard();
			Appear();

			return base.OnOpen(cancellationToken);
		}

		protected override UniTask OnClose(CancellationToken cancellationToken = default)
		{
			Unsubscribe();

			return base.OnClose(cancellationToken);
		}

		public override void Dispose()
		{
			Unsubscribe();
			base.Dispose();
		}

		private void Unsubscribe()
		{
			if (_isOpen == false)
				return;

			_isOpen = false;
			retakeButton.onClick.RemoveListener(HandleRetakeClicked);
			menuButton.onClick.RemoveListener(HandleMenuClicked);
			content.DOKill();

			if (_submitCts == null)
				return;

			_submitCts.Cancel();
			_submitCts.Dispose();
			_submitCts = null;
		}

		private void ShowReportCard()
		{
			int answersCopied = _exam.GetAnswersCopied();
			int ducksThrown = _duck.GetThrowCount();
			int almostCaughtCount = _teacher.GetAlmostCaughtCount();
			float elapsedSeconds = _exam.GetElapsedSeconds();
			ExamOutcome outcome = _exam.GetOutcome();
			ExamGrade grade = _examGradeService.GetGrade(answersCopied);
			int starCount = _examGradeService.GetStars(answersCopied, ducksThrown, almostCaughtCount);

			title.text = GetTitle(outcome);
			subtitle.text = GetSubtitle(outcome);
			statValues[0].text = $"{answersCopied}/{_exam.GetTotalQuestions()}";
			statValues[1].text = ResultTimeFormat.Format(elapsedSeconds);
			statValues[2].text = _exam.GetMeowCount().ToString();
			statValues[3].text = almostCaughtCount.ToString();
			statValues[4].text = ducksThrown.ToString();
			gradeStamp.sprite = gradeStamps[(int)grade];
			gradeMessage.text = GetGradeMessage(grade, starCount);
			ShowStars(starCount);
			personalBest.text = $"Your best: {_progress.GetBestAnswers()} answers · " +
				ResultTimeFormat.Format(_progress.GetBestTimeSeconds());
			ShowSending();
			SubmitResult(new LeaderboardEntry(
				name: GetPlayerName(),
				answers: answersCopied,
				timeSeconds: elapsedSeconds,
				grade: grade.GetLabel())).Forget();
		}

		private void ShowStars(int starCount)
		{
			for (int index = 0; index < stars.Length; index++)
			{
				stars[index].sprite = index < starCount ? starFilled : starEmpty;
				stars[index].gameObject.SetActive(starCount > 0);
			}
		}

		private void ShowSending()
		{
			leaderboardStatus.text = "Sending your result…";
			leaderboardStatus.gameObject.SetActive(true);
			ownRank.gameObject.SetActive(false);

			foreach (ResultLeaderboardRow row in leaderboardRows)
				row.Hide();
		}

		private void ShowLeaderboard(LeaderboardResponse response)
		{
			if (response.IsOffline)
			{
				leaderboardStatus.text = "Leaderboard offline";
				return;
			}

			leaderboardStatus.gameObject.SetActive(false);

			for (int index = 0; index < leaderboardRows.Length; index++)
			{
				if (index < response.Top.Count)
					leaderboardRows[index].Show(response.Top[index], index + 1, response.Rank == index + 1);
				else
					leaderboardRows[index].Hide();
			}

			ownRank.text = $"#{response.Rank} — you";
			ownRank.gameObject.SetActive(response.Rank > leaderboardRows.Length);
		}

		protected override void OnUpdate()
		{
			if (_isOpen == false || _isLeaving)
				return;

			if (_inputService.IsKeyPressed(KeyCode.R))
				HandleRetakeClicked();
		}

		private void Appear()
		{
			content.DOScale(Vector3.one, ScaleDuration).SetEase(Ease.OutBack).SetUpdate(true);
		}

		private void Leave(LoopNodeId loopNodeId)
		{
			_isLeaving = true;
			retakeButton.interactable = false;
			menuButton.interactable = false;

			content
				.DOScale(Vector3.zero, ScaleDuration)
				.SetEase(Ease.InBack)
				.SetUpdate(true)
				.OnComplete(RunTransition);

			void RunTransition() => FadeToBlackThenTransition(loopNodeId).Forget();
		}

		private async UniTaskVoid SubmitResult(LeaderboardEntry entry)
		{
			(bool isCanceled, LeaderboardResponse response) = await _leaderboardService
				.Submit(entry, _submitCts.Token)
				.SuppressCancellationThrow();

			if (isCanceled)
				return;

			ShowLeaderboard(response);
		}

		private async UniTaskVoid FadeToBlackThenTransition(LoopNodeId loopNodeId)
		{
			FadeWindow fadeWindow = await _uiService.OpenWindow<FadeWindow>(withAnimation: false);
			await fadeWindow.FadeIn(FadeInDuration);

			_cameraSwitcher.SwitchTo(loopNodeId);
			_coreLoopRequestFactory.CreateCloseBranchRequest(LoopNodeId.Exam);

			if (loopNodeId == LoopNodeId.Exam)
				_coreLoopRequestFactory.CreateGoToBranchRequest(LoopNodeId.Exam);
			else
				_coreLoopRequestFactory.CreateGoToNodeRequest(loopNodeId);
		}

		private string GetPlayerName()
		{
			string playerName = _progress.GetPlayerName();

			return string.IsNullOrWhiteSpace(playerName) ? "Nameless Kitten" : playerName;
		}

		private static string GetTitle(ExamOutcome outcome)
		{
			return outcome switch
			{
				ExamOutcome.Caught => "CAUGHT",
				ExamOutcome.BellRang => "BELL RANG",
				_ => "EXAM PASSED!"
			};
		}

		private static string GetSubtitle(ExamOutcome outcome)
		{
			return outcome switch
			{
				ExamOutcome.Caught => "CAUGHT. See me after class.",
				ExamOutcome.BellRang => "RIIING! Pencils down!",
				_ => "...You all passed? Suspicious."
			};
		}

		private static string GetGradeMessage(ExamGrade grade, int starCount)
		{
			if (grade != ExamGrade.APlus)
			{
				return grade switch
				{
					ExamGrade.A => "Almost purrfect. The bell saved her, not you.",
					ExamGrade.B => "Solid cheating. Barely any paw prints.",
					ExamGrade.C => "Average copycat. Whiskerstein noticed.",
					ExamGrade.D => "You cheated like a dog. Cats do better.",
					_ => "Detention. Even the duck is disappointed."
				};
			}

			return starCount switch
			{
				3 => "Purrfect crime. No duck, no evidence.",
				2 => "Nice cheating. A few close calls.",
				_ => "Passed by a whisker."
			};
		}

		private void HandleRetakeClicked()
		{
			if (_isLeaving)
				return;

			Leave(LoopNodeId.Exam);
		}

		private void HandleMenuClicked()
		{
			if (_isLeaving)
				return;

			Leave(LoopNodeId.StartLaunch);
		}
	}
}
