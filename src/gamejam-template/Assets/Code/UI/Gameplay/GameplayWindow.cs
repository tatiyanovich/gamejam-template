using System;
using System.Threading;
using Code.Gameplay.Bell.Queries;
using Code.Gameplay.Duck;
using Code.Gameplay.Duck.Queries;
using Code.Gameplay.Duck.Services;
using Code.Gameplay.Exam;
using Code.Gameplay.Exam.Queries;
using Code.Gameplay.Input.Behaviours;
using Code.Gameplay.Input.Queries;
using Code.Gameplay.Meow.Queries;
using Code.Gameplay.Neighbours.Queries;
using Code.Gameplay.Suspicion.Queries;
using Code.Gameplay.Teacher;
using Code.Gameplay.Teacher.Behaviours;
using Code.Gameplay.Teacher.Queries;
using Code.UI.Result;
using Cysharp.Threading.Tasks;
using Framework.UI.UiManagement.Elements.Windows;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
using SF = UnityEngine.SerializeField;

namespace Code.UI.Gameplay
{
	public class GameplayWindow : WindowBase
	{
		[SF] private RectTransform layout;
		[SF] private TMP_Text answers;
		[SF] private TMP_Text clock;
		[SF] private Image suspicionFill;
		[SF] private Image microphoneFill;
		[SF] private RectTransform microphoneThreshold;
		[SF] private TMP_Text microphoneHint;
		[SF] private Image cooldownFill;
		[SF] private Button duckButton;
		[SF] private GameObject bubble;
		[SF] private TMP_Text speech;
		[SF] private RectTransform hintBubble;
		[SF] private TMP_Text hint;
		[SF] private GameObject hintStrokes;
		[SF] private DangerVignetteView vignette;
		[SF] private FlashStackView flashes;

		private PawTimerView[] _pawTimers;
		private TeacherView _teacherView;
		private KittenView _kittenView;
		private bool _worldViewsBound;
		private float _speechSeconds;
		private int _watchingLine;
		private bool _isOpen;
		private bool _finished;
		private bool _announced;
		private bool _reportCardRequested;
		private bool _isHot;
		private float _microphoneLevel;
		private float _quietSeconds;

		private IExamQuery _exam;
		private IBellQuery _bell;
		private ISuspicionQuery _suspicion;
		private IMeowQuery _meow;
		private IDuckQuery _duck;
		private IDuckFactory _duckFactory;
		private ITeacherQuery _teacher;
		private INeighbourQuery _neighbours;
		private IInputQuery _input;

		private static readonly Color OkTint = new Color32(79, 203, 122, 255);
		private static readonly Color WarnTint = new Color32(255, 154, 61, 255);
		private static readonly Color DangerTint = new Color32(232, 76, 76, 255);
		private static readonly Color DuckTint = new Color32(255, 216, 61, 255);

		private const float ReportCardDelaySeconds = 1.6f;
		private const float HotSuspicionShare = 0.8f;
		private const float QuietMinimumLevel = 20f;
		private const float QuietMaximumLevel = 40f;
		private const float QuietSeconds = 0.5f;

		[Inject]
		public void Construct(
			IExamQuery exam,
			IBellQuery bell,
			ISuspicionQuery suspicion,
			IMeowQuery meow,
			IDuckQuery duck,
			IDuckFactory duckFactory,
			ITeacherQuery teacher,
			INeighbourQuery neighbours,
			IInputQuery input)
		{
			_exam = exam;
			_bell = bell;
			_suspicion = suspicion;
			_meow = meow;
			_duck = duck;
			_duckFactory = duckFactory;
			_teacher = teacher;
			_neighbours = neighbours;
			_input = input;
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
			_reportCardRequested = false;
			_finished = _exam.IsFinished();
			_announced = _bell.IsAnnounced();
			_watchingLine = 0;
			_speechSeconds = 0f;
			_isHot = _suspicion.GetLevel() >= _suspicion.GetMaximumLevel() * HotSuspicionShare;
			_microphoneLevel = _meow.GetMicrophoneLevel();
			_quietSeconds = 0f;
			bubble.SetActive(false);
			hintBubble.gameObject.SetActive(false);
			vignette.Hide();
			flashes.Clear();
			_exam.OnAnswersCopiedChanged += HandleAnswers;
			_exam.OnAnswerCopied += HandleAnswerCopied;
			_exam.OnWrongInput += HandleWrongInput;
			_exam.OnExamFinished += HandleFinished;
			_exam.OnTutorialHintChanged += HandleHint;
			_bell.OnTimeLeftChanged += HandleTime;
			_bell.OnAnnounced += HandleAnnouncement;
			_suspicion.OnLevelChanged += HandleSuspicion;
			_meow.OnMicrophoneLevelChanged += HandleMicrophone;
			_duck.OnStateChanged += HandleDuck;
			_teacher.OnAttentionChanged += HandleAttention;
			_teacher.OnRemark += HandleRemark;
			duckButton.onClick.AddListener(HandleThrowDuck);
			BindWorldViews();
			OnRectTransformDimensionsChange();
			HandleAnswers(_exam.GetAnswersCopied());
			HandleTime(_bell.GetTimeLeft());
			HandleSuspicion(_suspicion.GetLevel());
			HandleMicrophone(_meow.GetMicrophoneLevel());
			HandleHint(_exam.GetTutorialHint());
			RefreshDuck();
			if (_finished)
				HandleFinished(_exam.GetOutcome());
			else
				HandleAttention(_teacher.GetAttention());
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
			_exam.OnAnswersCopiedChanged -= HandleAnswers;
			_exam.OnAnswerCopied -= HandleAnswerCopied;
			_exam.OnWrongInput -= HandleWrongInput;
			_exam.OnExamFinished -= HandleFinished;
			_exam.OnTutorialHintChanged -= HandleHint;
			_bell.OnTimeLeftChanged -= HandleTime;
			_bell.OnAnnounced -= HandleAnnouncement;
			_suspicion.OnLevelChanged -= HandleSuspicion;
			_meow.OnMicrophoneLevelChanged -= HandleMicrophone;
			_duck.OnStateChanged -= HandleDuck;
			_teacher.OnAttentionChanged -= HandleAttention;
			_teacher.OnRemark -= HandleRemark;
			duckButton.onClick.RemoveListener(HandleThrowDuck);
			UnbindWorldViews();
			bubble.SetActive(false);
			hintBubble.gameObject.SetActive(false);
			vignette.Hide();
			flashes.Clear();
		}

		private void BindWorldViews()
		{
			_pawTimers = FindObjectsByType<PawTimerView>(FindObjectsInactive.Include, FindObjectsSortMode.None);
			_teacherView = FindFirstObjectByType<TeacherView>(FindObjectsInactive.Include);
			_kittenView = FindFirstObjectByType<KittenView>(FindObjectsInactive.Include);
			if (_pawTimers.Length == 0 || _teacherView == null || _kittenView == null)
				return;

			foreach (PawTimerView timer in _pawTimers)
				timer.Bind(_neighbours);

			_teacherView.Bind(_teacher);
			_kittenView.Bind(_input, _exam, _teacher);
			_worldViewsBound = true;
		}

		private void UnbindWorldViews()
		{
			foreach (PawTimerView timer in _pawTimers)
			{
				if (timer != null)
					timer.Unbind();
			}

			if (_teacherView != null)
				_teacherView.Unbind();

			if (_kittenView != null)
				_kittenView.Unbind();

			_pawTimers = Array.Empty<PawTimerView>();
			_teacherView = null;
			_kittenView = null;
			_worldViewsBound = false;
		}

		protected override void OnUpdate()
		{
			if (_isOpen == false)
				return;

			if (_worldViewsBound == false)
				BindWorldViews();

			if (_speechSeconds > 0f)
			{
				_speechSeconds -= Time.deltaTime;
				if (_speechSeconds <= 0f)
					bubble.SetActive(false);
			}

			RefreshVignette();
			RefreshQuietMicrophone();
		}

		private void RefreshVignette()
		{
			if (_finished || _teacher.IsFacingClass() == false)
			{
				vignette.Hide();
				return;
			}

			vignette.Show(_input.IsLeaning());
		}

		private void RefreshQuietMicrophone()
		{
			if (_finished || _meow.IsMicrophoneAvailable() == false
				|| _microphoneLevel < QuietMinimumLevel || _microphoneLevel > QuietMaximumLevel)
			{
				_quietSeconds = 0f;
				return;
			}

			_quietSeconds += Time.deltaTime;
			if (_quietSeconds < QuietSeconds)
				return;

			_quietSeconds = 0f;
			flashes.Show("LOUDER!", WarnTint);
		}

		private void ShowSpeech(string line)
		{
			if (_finished)
				return;

			speech.text = line;
			_speechSeconds = 1.2f;
			bubble.SetActive(true);
		}

		private void RefreshDuck()
		{
			duckButton.interactable = _finished == false && _duck.CanThrow();
		}

		private async UniTaskVoid OpenReportCard()
		{
			bool isCanceled = await UniTask
				.Delay(TimeSpan.FromSeconds(ReportCardDelaySeconds), DelayType.UnscaledDeltaTime,
					cancellationToken: Cts.Token)
				.SuppressCancellationThrow();

			if (isCanceled || _isOpen == false)
				return;

			await _uiService.OpenWindow<ResultWindow>();
			await _uiService.CloseWindow<GameplayWindow>(withAnimation: false);
		}

		private void HandleAnswers(int count) => answers.text = $"ANSWERS {count} / {_exam.GetTotalQuestions()}";

		private void HandleTime(float seconds)
		{
			int remaining = Mathf.CeilToInt(Mathf.Max(0f, seconds));
			clock.text = $"{remaining / 60}:{remaining % 60:00}";
			clock.color = _announced || _bell.IsAnnounced()
				? new Color32(255, 107, 88, 255) : new Color32(196, 239, 171, 255);
		}

		private void HandleAnnouncement()
		{
			_announced = true;
			HandleTime(_bell.GetTimeLeft());
			ShowSpeech("Ten minutes left, class!");
		}

		private void HandleSuspicion(float level)
		{
			float progress = Mathf.Clamp01(level / _suspicion.GetMaximumLevel());
			suspicionFill.fillAmount = progress;
			suspicionFill.color = progress >= 0.8f ? new Color32(194, 54, 43, 255)
				: progress >= 0.5f ? new Color32(238, 153, 53, 255) : new Color32(80, 150, 76, 255);

			if (progress < HotSuspicionShare)
				return;

			if (_isHot == false)
				flashes.Show("TOO HOT!", DangerTint);

			_isHot = true;
		}

		private void HandleMicrophone(float level)
		{
			_microphoneLevel = level;
			bool available = _meow.IsMicrophoneAvailable();
			microphoneFill.fillAmount = available ? Mathf.Clamp01(level / 100f) : 0f;
			microphoneFill.color = available && _meow.IsArmed() && _meow.IsOnCooldown() == false
				? new Color32(80, 150, 76, 255) : new Color32(150, 150, 150, 255);
			microphoneHint.text = available ? "[M] if no mic" : "No mic — press M to meow";
			float threshold = Mathf.Clamp01(_meow.GetThresholdLevel() / 100f);
			float chord = Mathf.Sqrt(Mathf.Max(0f, 1f - Mathf.Pow(2f * threshold - 1f, 2f)));
			microphoneThreshold.anchoredPosition = new Vector2(120f, -216f + 192f * threshold);
			microphoneThreshold.sizeDelta = new Vector2(192f * chord, 12f);
			cooldownFill.fillAmount = _meow.GetCooldownSeconds() <= 0f ? 0f
				: Mathf.Clamp01(_meow.GetCooldownTimeLeft() / _meow.GetCooldownSeconds());
		}

		private void HandleDuck(DuckState state)
		{
			RefreshDuck();
			switch (state)
			{
				case DuckState.Flying: ShowSpeech("WHOSE DUCK IS THIS?!"); break;
				case DuckState.OnDesk:
					if (_duck.GetThrowCount() > 0)
						ShowSpeech("Keep. It. Quiet.");
					break;
				case DuckState.Confiscated:
					flashes.Show("DUCK CONFISCATED", WarnTint);
					ShowSpeech("That's it. The duck is MINE.");
					break;
			}
		}

		private void HandleAttention(TeacherAttention attention)
		{
			switch (attention)
			{
				case TeacherAttention.Turning: ShowSpeech("Hmm?"); break;
				case TeacherAttention.Staring: ShowSpeech("MRS. HISSKINS SEES YOU."); break;
				case TeacherAttention.Distracted:
					flashes.Show($"DUCK AWAY! {Mathf.RoundToInt(_duck.GetDistractionSeconds())}s", DuckTint);
					break;
				case TeacherAttention.Watching:
					if (_input.IsLeaning())
						flashes.Show("SHE'S LOOKING!", DangerTint);

					ShowSpeech((_watchingLine++ % 3) switch
					{
						0 => "I'm watching you.",
						1 => "Eyes on your OWN paper.",
						_ => "Whiskers down, everyone."
					});
					break;
			}
		}

		private void HandleAnswerCopied(int questionIndex) => flashes.Show("ANSWER COPIED!", OkTint);

		private void HandleWrongInput(int questionIndex) => flashes.Show("PENCIL SNAP!", DangerTint);

		private void HandleHint(TutorialHint tutorialHint)
		{
			hint.text = tutorialHint switch
			{
				TutorialHint.Meow => _meow.IsMicrophoneAvailable()
					? "MEOW into your mic to get Whiskerstein's attention!"
					: "Press M to get Whiskerstein's attention!",
				TutorialHint.Lean => "Hold SPACE to lean over",
				TutorialHint.Copy => "Copy the strokes:",
				TutorialHint.Dodge => "Psst. She turns around sometimes. Let go of SPACE!",
				TutorialHint.Duck => "Throw the duck when it gets hot [Q]",
				_ => string.Empty
			};
			bool strokes = tutorialHint == TutorialHint.Copy;
			hintStrokes.SetActive(strokes);
			hintBubble.sizeDelta = new Vector2(560f, strokes ? 160f : 112f);
			hintBubble.gameObject.SetActive(tutorialHint != TutorialHint.None);
		}

		private void HandleRemark(TeacherRemark remark)
		{
			ShowSpeech(remark switch
			{
				TeacherRemark.MeowAlert => "Did someone MEOW?",
				TeacherRemark.PencilAlert => "What was that?!",
				_ => "NO. MEOWING."
			});
		}

		private void HandleFinished(ExamOutcome outcome)
		{
			_finished = false;
			ShowSpeech(outcome switch
			{
				ExamOutcome.Caught => "CAUGHT. See me after class.",
				ExamOutcome.BellRang => "RIIING! Pencils down!",
				_ => "...You all passed? Suspicious."
			});
			_finished = true;
			RefreshDuck();
			vignette.Hide();
			flashes.Clear();

			if (_reportCardRequested)
				return;

			_reportCardRequested = true;
			OpenReportCard().Forget();
		}

		private void HandleThrowDuck()
		{
			if (_finished == false && _duck.CanThrow())
				_duckFactory.CreateThrowDuckRequest();
		}
	}
}
