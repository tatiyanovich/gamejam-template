using System.Threading;
using Code.Gameplay.Camera.Services;
using Code.Gameplay.CoreLoop.Services;
using Code.Infrastructure.CoreLoop;
using Code.UI.Fade;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Framework.UI.UiManagement.Elements.Buttons;
using Framework.UI.UiManagement.Elements.Windows;
using UnityEngine;
using Zenject;
using SF = UnityEngine.SerializeField;

namespace Code.UI.Result
{
	public class ResultWindow : WindowBase
	{
		[SF] private RectTransform content;
		[SF] private Button replayButton;
		[SF] private Button menuButton;

		private ICoreLoopRequestFactory _coreLoopRequestFactory;
		private ICameraSwitcher _cameraSwitcher;

		private const float FadeInDuration = 0.3f;
		private const float ScaleDuration = 0.5f;

		[Inject]
		public void Construct(
			ICoreLoopRequestFactory coreLoopRequestFactory,
			ICameraSwitcher cameraSwitcher)
		{
			_coreLoopRequestFactory = coreLoopRequestFactory;
			_cameraSwitcher = cameraSwitcher;
		}

		protected override UniTask OnOpen(CancellationToken cancellationToken = default)
		{
			content.localScale = Vector3.zero;

			replayButton.OnClicked += HandleReplayClicked;
			menuButton.OnClicked += HandleMenuClicked;

			Appear();

			return base.OnOpen(cancellationToken);
		}

		protected override UniTask OnClose(CancellationToken cancellationToken = default)
		{
			replayButton.OnClicked -= HandleReplayClicked;
			menuButton.OnClicked -= HandleMenuClicked;

			return base.OnClose(cancellationToken);
		}

		private void Appear()
		{
			content.DOScale(Vector3.one, ScaleDuration).SetEase(Ease.OutBack).SetUpdate(true);
		}

		private void HandleReplayClicked() => Disappear(LoopNodeId.Exam);

		private void HandleMenuClicked() => Disappear(LoopNodeId.StartLaunch);

		private void Disappear(LoopNodeId loopNodeId)
		{
			content
				.DOScale(Vector3.zero, ScaleDuration)
				.SetEase(Ease.InBack)
				.SetUpdate(true)
				.OnComplete(RunTransition);

			void RunTransition() => FadeToBlackThenTransition(loopNodeId).Forget();
		}

		private async UniTaskVoid FadeToBlackThenTransition(LoopNodeId loopNodeId)
		{
			FadeWindow fadeWindow = await _uiService.OpenWindow<FadeWindow>(withAnimation: false);
			await fadeWindow.FadeIn(FadeInDuration);

			_cameraSwitcher.SwitchTo(loopNodeId);

			if (loopNodeId == LoopNodeId.Exam)
			{
				_coreLoopRequestFactory.CreateCloseBranchRequest(LoopNodeId.Exam);
				_coreLoopRequestFactory.CreateGoToBranchRequest(LoopNodeId.Exam);
			}
			else
			{
				_coreLoopRequestFactory.CreateCloseBranchRequest(LoopNodeId.Exam);
				_coreLoopRequestFactory.CreateGoToNodeRequest(loopNodeId);
			}
		}
	}
}
