using Code.Infrastructure.StateManagement;
using Framework.Storage;

namespace Code.Gameplay.Progress.Services
{
	public class NewPlayerService : INewPlayerService
	{
		private readonly ISaveLoadService _saveLoadService;
		private readonly IGameStateMachine _gameStateMachine;

		public NewPlayerService(
			ISaveLoadService saveLoadService,
			IGameStateMachine gameStateMachine)
		{
			_saveLoadService = saveLoadService;
			_gameStateMachine = gameStateMachine;
		}

		public void StartNewPlayer()
		{
			_saveLoadService.EraseProgress();
			_gameStateMachine.RebootGame();
		}
	}
}
