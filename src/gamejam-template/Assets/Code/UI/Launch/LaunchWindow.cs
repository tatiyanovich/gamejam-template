using System.Threading;
using Code.Gameplay.Camera.Services;
using Code.Gameplay.CoreLoop.Services;
using Code.Gameplay.Progress.Queries;
using Code.Infrastructure.CoreLoop;
using Code.UI.Attendance;
using Code.UI.Tutorial;
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
		private ICameraSwitcher _cameraSwitcher;
		private IProgressQuery _progressQuery;

		[Inject]
		public void Construct(
			ICoreLoopRequestFactory coreLoopRequestFactory,
			ICameraSwitcher cameraSwitcher,
			IProgressQuery progressQuery)
		{
			_coreLoopRequestFactory = coreLoopRequestFactory;
			_cameraSwitcher = cameraSwitcher;
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

		private async UniTask OpenTutorial()
		{
			_isStarting = true;
			SetInteractable(false);
			await _uiService.CloseWindow<LaunchWindow>(withAnimation: false);
			await _uiService.OpenWindow<TutorialWindow>(
				beforeOpen: window => window.Prepare(StartExam));
		}

		private void StartExam()
		{
			_cameraSwitcher.SwitchTo(LoopNodeId.Exam);
			_coreLoopRequestFactory.CreateCloseBranchRequest(LoopNodeId.Exam);
			_coreLoopRequestFactory.CreateGoToBranchRequest(LoopNodeId.Exam);
		}

		private async UniTask OpenAttendance()
		{
			await _uiService.CloseWindow<LaunchWindow>(withAnimation: false);
			await _uiService.OpenWindow<AttendanceWindow>();
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

			OpenTutorial().Forget();
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
