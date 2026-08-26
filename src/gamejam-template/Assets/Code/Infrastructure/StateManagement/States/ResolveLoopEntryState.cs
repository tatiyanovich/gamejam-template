using Code.Infrastructure.CoreLoop;
using Code.Storage.SaveFiles;
using Framework.StateManagement;
using Framework.Storage;

namespace Code.Infrastructure.StateManagement.States
{
	// Picks the loop node the player left off at, so a relaunch resumes where the save says.
	public class ResolveLoopEntryState : IState, IEnter
	{
		private readonly IGameStateMachine _gameStateMachine;
		private readonly ISaveLoadService _saveLoadService;

		public ResolveLoopEntryState(
			IGameStateMachine gameStateMachine,
			ISaveLoadService saveLoadService)
		{
			_gameStateMachine = gameStateMachine;
			_saveLoadService = saveLoadService;
		}

		public void Enter()
		{
			_gameStateMachine.Enter<LoadLoopSceneState, LoopScenePayload>(ResolveSceneAddress());
		}

		private LoopScenePayload ResolveSceneAddress()
		{
			GeneralSaveFile saveFile = _saveLoadService.Get<GeneralSaveFile>();
			LoopNodeId loopNodeId = saveFile.SessionLoop?.CurrentNode ?? LoopNodeId.StartLaunch;

			return new LoopScenePayload(loopNodeId, Addresses.SceneNames.GetLoopScene(loopNodeId));
		}
	}
}
