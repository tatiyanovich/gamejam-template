using Code.Infrastructure.EntityComponentSystem.Systems;
using Code.Infrastructure.Health;
using Entitas;
using Framework.Storage;
using UnityEngine;

namespace Code.Storage.Systems
{
	public class SaveProgressByRequestSystem : RequestHandlerSystem<GameEntity>
	{
		private readonly ISaveLoadService _saveLoadService;
		private readonly IApplicationHealthService _applicationHealthService;

		public SaveProgressByRequestSystem(
			GameContext game,
			ISaveLoadService saveLoadService,
			IApplicationHealthService applicationHealthService)
			: base(game.GetGroup(GameMatcher.SaveProgressRequest))
		{
			_saveLoadService = saveLoadService;
			_applicationHealthService = applicationHealthService;
		}

		protected override void OnExecute(IGroup<GameEntity> requests)
		{
			if (_applicationHealthService.HasCriticalErrors)
			{
				Debug.LogError("Skipping auto-save due to critical errors during session.");
				return;
			}

			_saveLoadService.SaveProgress();
		}
	}
}
