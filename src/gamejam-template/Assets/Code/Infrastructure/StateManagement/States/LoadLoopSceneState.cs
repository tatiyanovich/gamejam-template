using Code.Infrastructure.EntityComponentSystem.Destruct.Services;
using Code.Infrastructure.Scenes;
using Code.UI.Fade;
using Code.UI.Loading;
using Cysharp.Threading.Tasks;
using Framework.Essentials.SceneManagement;
using Framework.StateManagement;
using Framework.UI.UiManagement.Services;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;

namespace Code.Infrastructure.StateManagement.States
{
	public class LoadLoopSceneState : IState, IPayloadedEnter<LoopScenePayload>
	{
		private readonly IGameStateMachine _gameStateMachine;
		private readonly ISceneLoadService _sceneLoadService;
		private readonly ILoadedSceneRegistry _loadedSceneRegistry;
		private readonly IUiService _uiService;
		private readonly ILoopEntityWipeService _loopEntityWipeService;

		public LoadLoopSceneState(
			IGameStateMachine gameStateMachine,
			ISceneLoadService sceneLoadService,
			ILoadedSceneRegistry loadedSceneRegistry,
			IUiService uiService,
			ILoopEntityWipeService loopEntityWipeService)
		{
			_gameStateMachine = gameStateMachine;
			_sceneLoadService = sceneLoadService;
			_loadedSceneRegistry = loadedSceneRegistry;
			_uiService = uiService;
			_loopEntityWipeService = loopEntityWipeService;
		}

		public void Enter(LoopScenePayload loopScenePayload)
		{
			Load(loopScenePayload).Forget();
		}

		private async UniTaskVoid Load(LoopScenePayload loopScenePayload)
		{
			FadeWindow fadeWindow = await _uiService.OpenWindow<FadeWindow>(withAnimation: false);

			_loopEntityWipeService.WipeNodeScopedEntities();

			LoadingWindow loadingWindow = _uiService.GetWindow<LoadingWindow>();

			if (loadingWindow)
				loadingWindow.SetProgress(0.66f);
			else
				await fadeWindow.FadeIn(1);

			SceneInstance scene = await _sceneLoadService.LoadScene(
				loopScenePayload.SceneAddress,
				LoadSceneMode.Single,
				onLoaded: () => ChooseNextState(loopScenePayload));

			_loadedSceneRegistry.Register(loopScenePayload.LoopNodeId, scene);
		}

		private void ChooseNextState(LoopScenePayload loopScenePayload)
		{
			_gameStateMachine.Enter<PrepareLoopSceneState, LoopScenePayload>(loopScenePayload);
		}
	}
}
