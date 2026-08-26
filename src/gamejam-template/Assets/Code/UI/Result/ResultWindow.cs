using System.Threading;
using Code.Gameplay.Camera.Services;
using Code.Gameplay.CoreLoop.Services;
using Code.Gameplay.Pickups.Queries;
using Code.Infrastructure.CoreLoop;
using Code.UI.Fade;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Framework.UI.UiManagement.Elements.Buttons;
using Framework.UI.UiManagement.Elements.Windows;
using TMPro;
using UnityEngine;
using Zenject;
using SF = UnityEngine.SerializeField;

namespace Code.UI.Result
{
	// A popup over the HUD: shows the run's score and routes back into the core loop through
	// requests. Windows never enter states directly — they create requests the systems handle.
	public class ResultWindow : WindowBase
	{
		[SF] private RectTransform content;
		[SF] private TextMeshProUGUI scoreText;
		[SF] private Button replayButton;
		[SF] private Button menuButton;

		private ICoreLoopRequestFactory _coreLoopRequestFactory;
		private ICameraSwitcher _cameraSwitcher;
		private IScoreQuery _scoreQuery;

		private const float FadeInDuration = 0.3f;
		private const float ScaleDuration = 0.5f;

		[Inject]
		public void Construct(
			ICoreLoopRequestFactory coreLoopRequestFactory,
			ICameraSwitcher cameraSwitcher,
			IScoreQuery scoreQuery)
		{
			_coreLoopRequestFactory = coreLoopRequestFactory;
			_cameraSwitcher = cameraSwitcher;
			_scoreQuery = scoreQuery;
		}

		protected override UniTask OnOpen(CancellationToken cancellationToken = default)
		{
			content.localScale = Vector3.zero;
			scoreText.text = _scoreQuery.GetScore().ToString();

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

		private void HandleReplayClicked() => Disappear(LoopNodeId.Battle);

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

			// Replaying means closing the branch first so the node rebuilds; going back to the menu
			// leaves the branch behind and reloads the launch scene as a plain pipeline.
			if (loopNodeId == LoopNodeId.Battle)
			{
				_coreLoopRequestFactory.CreateCloseBranchRequest(LoopNodeId.Battle);
				_coreLoopRequestFactory.CreateGoToBranchRequest(LoopNodeId.Battle);
			}
			else
			{
				_coreLoopRequestFactory.CreateCloseBranchRequest(LoopNodeId.Battle);
				_coreLoopRequestFactory.CreateGoToNodeRequest(loopNodeId);
			}
		}
	}
}
