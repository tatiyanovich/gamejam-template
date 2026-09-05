using System.Threading;
using Code.Gameplay.Camera.Services;
using Code.Gameplay.CoreLoop.Services;
using Code.Gameplay.Progress.Queries;
using Code.UI.Attendance;
using Code.Infrastructure.CoreLoop;
using Code.UI.Fade;
using Cysharp.Threading.Tasks;
using Framework.UI.UiManagement.Elements.Windows;
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
		[SF] private Button playButton;
		[SF] private Button quitButton;

		private bool _isStarting;

		private ICoreLoopRequestFactory _coreLoopRequestFactory;
		private ICameraSwitcher _cameraSwitch;
		private IProgressQuery _progressQuery;

		private const float FadeInDuration = 0.3f;

		[Inject]
		public void Construct(
			ICoreLoopRequestFactory coreLoopRequestFactory,
			ICameraSwitcher cameraSwitch,
			IProgressQuery progressQuery)
		{
			_coreLoopRequestFactory = coreLoopRequestFactory;
			_cameraSwitch = cameraSwitch;
			_progressQuery = progressQuery;
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
			SetInteractable(true);
			playButton.onClick.AddListener(HandlePlay);
			quitButton.onClick.AddListener(HandleQuit);
			return base.OnOpen(cancellationToken);
		}

		protected override UniTask OnClose(CancellationToken cancellationToken = default)
		{
			playButton.onClick.RemoveListener(HandlePlay);
			quitButton.onClick.RemoveListener(HandleQuit);
			return base.OnClose(cancellationToken);
		}

		private void SetInteractable(bool interactable)
		{
			playButton.interactable = interactable;
			quitButton.interactable = interactable;
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

		private async UniTask OpenAttendance()
		{
			await _uiService.OpenWindow<AttendanceWindow>();
			await _uiService.CloseWindow<LaunchWindow>(withAnimation: false);
		}

		private void HandlePlay()
		{
			if (_isStarting)
				return;

			if (string.IsNullOrWhiteSpace(_progressQuery.GetPlayerName()))
			{
				_isStarting = true;
				SetInteractable(false);
				OpenAttendance().Forget();
				return;
			}

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
