using System.Threading;
using Code.Gameplay.Camera.Services;
using Code.Gameplay.CoreLoop.Services;
using Code.Gameplay.Progress.Queries;
using Code.Gameplay.Progress.Services;
using Code.Infrastructure.CoreLoop;
using Code.UI.Fade;
using Cysharp.Threading.Tasks;
using Framework.UI.UiManagement.Elements.Windows;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
using SF = UnityEngine.SerializeField;

namespace Code.UI.Launch
{
	public class LaunchWindow : WindowBase
	{
		[SF] private RectTransform layout;
		[SF] private GameObject menu;
		[SF] private GameObject attendanceSheet;
		[SF] private Button playButton;
		[SF] private Button quitButton;
		[SF] private Button startExamButton;
		[SF] private TMP_InputField studentName;

		private bool _isStarting;

		private ICoreLoopRequestFactory _coreLoopRequestFactory;
		private ICameraSwitcher _cameraSwitch;
		private IProgressQuery _progressQuery;
		private IProgressFactory _progressFactory;

		private const float FadeInDuration = 0.3f;

		[Inject]
		public void Construct(
			ICoreLoopRequestFactory coreLoopRequestFactory,
			ICameraSwitcher cameraSwitch,
			IProgressQuery progressQuery,
			IProgressFactory progressFactory)
		{
			_coreLoopRequestFactory = coreLoopRequestFactory;
			_cameraSwitch = cameraSwitch;
			_progressQuery = progressQuery;
			_progressFactory = progressFactory;
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
			OnRectTransformDimensionsChange();
			menu.SetActive(true);
			attendanceSheet.SetActive(false);
			SetInteractable(true);
			playButton.onClick.AddListener(HandlePlay);
			quitButton.onClick.AddListener(HandleQuit);
			startExamButton.onClick.AddListener(HandleStartExam);
			studentName.onValidateInput = ValidateName;
			return base.OnOpen(cancellationToken);
		}

		protected override UniTask OnClose(CancellationToken cancellationToken = default)
		{
			playButton.onClick.RemoveListener(HandlePlay);
			quitButton.onClick.RemoveListener(HandleQuit);
			startExamButton.onClick.RemoveListener(HandleStartExam);
			studentName.onValidateInput = null;
			return base.OnClose(cancellationToken);
		}

		private void SetInteractable(bool interactable)
		{
			playButton.interactable = interactable;
			quitButton.interactable = interactable;
			startExamButton.interactable = interactable;
			studentName.interactable = interactable;
		}

		private void StartExam()
		{
			_isStarting = true;
			SetInteractable(false);
			FadeToBlackThenTransition().Forget();
		}

		private async UniTask FadeToBlackThenTransition()
		{
			CancellationToken cancellationToken = Cts.Token;
			await UniTask.NextFrame(cancellationToken);
			FadeWindow fadeWindow = await _uiService.OpenWindow<FadeWindow>(withAnimation: false);
			cancellationToken.ThrowIfCancellationRequested();
			await fadeWindow.FadeIn(FadeInDuration, cancellationToken);
			cancellationToken.ThrowIfCancellationRequested();
			_cameraSwitch.SwitchTo(LoopNodeId.Exam);
			_coreLoopRequestFactory.CreateGoToBranchRequest(LoopNodeId.Exam);
		}

		private char ValidateName(string text, int index, char character)
		{
			return character is >= 'A' and <= 'Z' or >= 'a' and <= 'z' or >= '0' and <= '9'
				? character
				: '\0';
		}

		private void HandlePlay()
		{
			if (_isStarting)
				return;

			if (string.IsNullOrWhiteSpace(_progressQuery.GetPlayerName()))
			{
				menu.SetActive(false);
				attendanceSheet.SetActive(true);
				studentName.SetTextWithoutNotify(string.Empty);
				studentName.ActivateInputField();
				return;
			}

			StartExam();
		}

		private void HandleStartExam()
		{
			if (_isStarting)
				return;

			string playerName = string.IsNullOrWhiteSpace(studentName.text) ? "Nameless Kitten" : studentName.text;
			_progressFactory.CreateSetPlayerNameRequest(playerName);
			StartExam();
		}

		private void HandleQuit()
		{
#if UNITY_EDITOR
			UnityEditor.EditorApplication.isPlaying = false;
#else
			Application.Quit();
#endif
		}
	}
}
