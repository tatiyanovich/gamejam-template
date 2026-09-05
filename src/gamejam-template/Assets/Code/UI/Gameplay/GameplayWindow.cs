using System.Threading;
using Code.Gameplay.Bell.Queries;
using Code.Gameplay.Duck;
using Code.Gameplay.Duck.Queries;
using Code.Gameplay.Duck.Services;
using Code.Gameplay.Exam;
using Code.Gameplay.Exam.Queries;
using Code.Gameplay.Meow.Queries;
using Code.Gameplay.Neighbours.Queries;
using Code.Gameplay.Suspicion.Queries;
using Code.Gameplay.Teacher;
using Code.Gameplay.Teacher.Queries;
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

		private PawTimerView[] _pawTimers;
		private float _speechSeconds;
		private int _watchingLine;
		private bool _isOpen;
		private bool _finished;
		private bool _announced;

		private IExamQuery _exam;
		private IBellQuery _bell;
		private ISuspicionQuery _suspicion;
		private IMeowQuery _meow;
		private IDuckQuery _duck;
		private IDuckFactory _duckFactory;
		private ITeacherQuery _teacher;
		private INeighbourQuery _neighbours;

		[Inject]
		public void Construct(
			IExamQuery exam,
			IBellQuery bell,
			ISuspicionQuery suspicion,
			IMeowQuery meow,
			IDuckQuery duck,
			IDuckFactory duckFactory,
			ITeacherQuery teacher,
			INeighbourQuery neighbours)
		{
			_exam = exam;
			_bell = bell;
			_suspicion = suspicion;
			_meow = meow;
			_duck = duck;
			_duckFactory = duckFactory;
			_teacher = teacher;
			_neighbours = neighbours;
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
			_finished = _exam.IsFinished();
			_announced = _bell.IsAnnounced();
			_watchingLine = 0;
			_speechSeconds = 0f;
			bubble.SetActive(false);
			_exam.OnAnswersCopiedChanged += HandleAnswers;
			_exam.OnExamFinished += HandleFinished;
			_bell.OnTimeLeftChanged += HandleTime;
			_bell.OnAnnounced += HandleAnnouncement;
			_suspicion.OnLevelChanged += HandleSuspicion;
			_meow.OnMicrophoneLevelChanged += HandleMicrophone;
			_duck.OnStateChanged += HandleDuck;
			_teacher.OnAttentionChanged += HandleAttention;
			_teacher.OnRemark += HandleRemark;
			duckButton.onClick.AddListener(HandleThrowDuck);
			_pawTimers = FindObjectsByType<PawTimerView>(FindObjectsInactive.Include, FindObjectsSortMode.None);
			foreach (PawTimerView timer in _pawTimers)
				timer.Bind(_neighbours);
			OnRectTransformDimensionsChange();
			HandleAnswers(_exam.GetAnswersCopied());
			HandleTime(_bell.GetTimeLeft());
			HandleSuspicion(_suspicion.GetLevel());
			HandleMicrophone(_meow.GetMicrophoneLevel());
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
			_exam.OnExamFinished -= HandleFinished;
			_bell.OnTimeLeftChanged -= HandleTime;
			_bell.OnAnnounced -= HandleAnnouncement;
			_suspicion.OnLevelChanged -= HandleSuspicion;
			_meow.OnMicrophoneLevelChanged -= HandleMicrophone;
			_duck.OnStateChanged -= HandleDuck;
			_teacher.OnAttentionChanged -= HandleAttention;
			_teacher.OnRemark -= HandleRemark;
			duckButton.onClick.RemoveListener(HandleThrowDuck);
			foreach (PawTimerView timer in _pawTimers)
			{
				if (timer != null)
					timer.Unbind();
			}
			bubble.SetActive(false);
		}

		protected override void OnUpdate()
		{
			if (_isOpen == false || _speechSeconds <= 0f)
				return;

			_speechSeconds -= Time.deltaTime;
			if (_speechSeconds <= 0f)
				bubble.SetActive(false);
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
		}

		private void HandleMicrophone(float level)
		{
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
				case DuckState.Confiscated: ShowSpeech("That's it. The duck is MINE."); break;
			}
		}

		private void HandleAttention(TeacherAttention attention)
		{
			switch (attention)
			{
				case TeacherAttention.Turning: ShowSpeech("Hmm?"); break;
				case TeacherAttention.Staring: ShowSpeech("MRS. HISSKINS SEES YOU."); break;
				case TeacherAttention.Watching:
					ShowSpeech((_watchingLine++ % 3) switch
					{
						0 => "I'm watching you.",
						1 => "Eyes on your OWN paper.",
						_ => "Whiskers down, everyone."
					});
					break;
			}
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
		}

		private void HandleThrowDuck()
		{
			if (_finished == false && _duck.CanThrow())
				_duckFactory.CreateThrowDuckRequest();
		}
	}
}
