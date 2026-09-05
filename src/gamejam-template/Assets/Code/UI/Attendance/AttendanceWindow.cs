using System.Threading;
using Code.Gameplay.Camera.Services;
using Code.Gameplay.CoreLoop.Services;
using Code.Gameplay.Meow.Queries;
using Code.Gameplay.Progress.Services;
using Code.Infrastructure.CoreLoop;
using Code.Infrastructure.Microphone;
using Code.UI.Fade;
using Cysharp.Threading.Tasks;
using Framework.UI.UiManagement.Elements.Windows;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
using SF = UnityEngine.SerializeField;

namespace Code.UI.Attendance
{
	public class AttendanceWindow : WindowBase
	{
		[SF] private RectTransform layout;
		[SF] private TMP_InputField studentName;
		[SF] private Button startExamButton;
		[SF] private Image microphoneFill;
		[SF] private Image microphoneTrack;
		[SF] private RectTransform microphoneThreshold;
		[SF] private TMP_Text microphoneHint;
		[SF] private GameObject microphoneCheckmark;

		private bool _isStarting;
		private bool _isOpen;
		private bool _microphonePassed;
		private bool _microphoneAvailable;
		private float _microphoneLevel;
		private float _quietSeconds;

		private IProgressFactory _progressFactory;
		private IMeowQuery _meowQuery;
		private IMicrophoneService _microphoneService;
		private ICameraSwitcher _cameraSwitcher;
		private ICoreLoopRequestFactory _coreLoopRequestFactory;

		[Inject]
		public void Construct(
			IProgressFactory progressFactory,
			IMeowQuery meowQuery,
			IMicrophoneService microphoneService,
			ICameraSwitcher cameraSwitcher,
			ICoreLoopRequestFactory coreLoopRequestFactory)
		{
			_progressFactory = progressFactory;
			_meowQuery = meowQuery;
			_microphoneService = microphoneService;
			_cameraSwitcher = cameraSwitcher;
			_coreLoopRequestFactory = coreLoopRequestFactory;
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
			_isStarting = false;
			_isOpen = true;
			_microphonePassed = false;
			_quietSeconds = 0f;
			studentName.characterLimit = 12;
			studentName.onValidateInput = ValidateName;
			studentName.SetTextWithoutNotify(string.Empty);
			studentName.interactable = true;
			startExamButton.interactable = true;
			startExamButton.onClick.AddListener(HandleStartExam);
			_meowQuery.OnMicrophoneLevelChanged += HandleMicrophoneLevelChanged;
			_meowQuery.OnMicrophoneTestPassed += HandleMicrophoneTestPassed;
			float threshold = Mathf.Clamp01(_meowQuery.GetThresholdLevel() / 100f);
			microphoneThreshold.anchorMin = new Vector2(threshold, 0f);
			microphoneThreshold.anchorMax = new Vector2(threshold, 1f);
			microphoneThreshold.anchoredPosition = Vector2.zero;
			OnRectTransformDimensionsChange();
			HandleMicrophoneLevelChanged(_meowQuery.GetMicrophoneLevel());
			studentName.ActivateInputField();
			return base.OnOpen(cancellationToken);
		}

		protected override UniTask OnClose(CancellationToken cancellationToken = default)
		{
			_isOpen = false;
			startExamButton.onClick.RemoveListener(HandleStartExam);
			studentName.onValidateInput = null;
			_meowQuery.OnMicrophoneLevelChanged -= HandleMicrophoneLevelChanged;
			_meowQuery.OnMicrophoneTestPassed -= HandleMicrophoneTestPassed;
			return base.OnClose(cancellationToken);
		}

		protected override void OnUpdate()
		{
			if (_isOpen == false)
				return;

			if (_microphoneAvailable != _microphoneService.IsAvailable)
				RefreshMicrophone();

			if (_microphonePassed || _microphoneAvailable == false || _microphoneLevel < 20f || _microphoneLevel > 40f)
			{
				_quietSeconds = 0f;
				return;
			}

			_quietSeconds += Time.unscaledDeltaTime;
			if (_quietSeconds >= 0.5f)
				microphoneHint.text = "LOUDER!";
		}

		private void RefreshMicrophone()
		{
			_microphoneAvailable = _microphoneService.IsAvailable;
			microphoneFill.fillAmount = _microphoneAvailable ? Mathf.Clamp01(_microphoneLevel / 100f) : 0f;
			microphoneTrack.color = _microphoneAvailable
				? new Color32(220, 208, 180, 255)
				: new Color32(150, 150, 150, 255);
			microphoneCheckmark.SetActive(_microphonePassed);
			microphoneHint.text = _microphoneAvailable == false ? "No mic — press M to meow"
				: _microphonePassed ? "LOUD ENOUGH!"
				: _quietSeconds >= 0.5f ? "LOUDER!" : "Meow to test your mic";
		}

		private async UniTask StartExam()
		{
			CancellationToken cancellationToken = Cts.Token;
			await UniTask.NextFrame(cancellationToken);
			FadeWindow fadeWindow = await _uiService.OpenWindow<FadeWindow>(withAnimation: false);
			cancellationToken.ThrowIfCancellationRequested();
			await fadeWindow.FadeIn(0.3f, cancellationToken);
			cancellationToken.ThrowIfCancellationRequested();
			_cameraSwitcher.SwitchTo(LoopNodeId.Exam);
			_coreLoopRequestFactory.CreateGoToBranchRequest(LoopNodeId.Exam);
		}

		private char ValidateName(string text, int index, char character)
		{
			return character is >= 'A' and <= 'Z' or >= 'a' and <= 'z' or >= '0' and <= '9'
				? character
				: '\0';
		}

		private void HandleMicrophoneLevelChanged(float level)
		{
			_microphoneLevel = level;
			if (level < 20f || level > 40f)
				_quietSeconds = 0f;
			RefreshMicrophone();
		}

		private void HandleMicrophoneTestPassed()
		{
			_microphonePassed = true;
			RefreshMicrophone();
		}

		private void HandleStartExam()
		{
			if (_isStarting)
				return;

			_isStarting = true;
			startExamButton.interactable = false;
			studentName.interactable = false;
			string playerName = string.IsNullOrWhiteSpace(studentName.text) ? "Nameless Kitten" : studentName.text;
			_progressFactory.CreateSetPlayerNameRequest(playerName);
			StartExam().Forget();
		}
	}
}
