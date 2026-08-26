using Code.Gameplay.Camera.Services;
using Code.Infrastructure.CoreLoop;
using Code.UI.Fade;
using Cysharp.Threading.Tasks;
using Framework.UI.UiManagement.Services;

namespace Code.Gameplay.CoreLoop.Services
{
	public class LoopNodeTransitionService : ILoopNodeTransitionService
	{
		private readonly IUiService _uiService;
		private readonly ICameraSwitcher _cameraSwitcher;
		private readonly ICoreLoopRequestFactory _coreLoopRequestFactory;

		private const float FadeInDuration = 0.3f;

		public LoopNodeTransitionService(
			IUiService uiService,
			ICameraSwitcher cameraSwitcher,
			ICoreLoopRequestFactory coreLoopRequestFactory)
		{
			_uiService = uiService;
			_cameraSwitcher = cameraSwitcher;
			_coreLoopRequestFactory = coreLoopRequestFactory;
		}

		public async UniTask GoTo(LoopNodeId nodeId)
		{
			FadeWindow fadeWindow = await _uiService.OpenWindow<FadeWindow>(withAnimation: false);
			await fadeWindow.FadeIn(FadeInDuration);

			_cameraSwitcher.SwitchTo(nodeId);
			_coreLoopRequestFactory.CreateGoToBranchRequest(nodeId);
		}
	}
}
