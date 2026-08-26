using Code.Infrastructure.EntityComponentSystem.Factories;
using UnityEngine;

namespace Code.Gameplay.Debugging.Services
{
	// Template for gameplay cheats: implement IGameplayDebugInputAction, bind it in
	// GameplayInstaller, and TriggerGameplayDebugActionsSystem fires it every frame it reports.
	public class AddScoreDebugAction : IGameplayDebugInputAction
	{
		private readonly IEntityFactory _entityFactory;

		private const int ScoreGranted = 10;

		public AddScoreDebugAction(IEntityFactory entityFactory)
		{
			_entityFactory = entityFactory;
		}

		public bool WasTriggeredThisFrame() => UnityEngine.Input.GetKeyDown(KeyCode.F1);

		public void Execute(Vector3 pointerWorldPosition)
		{
			_entityFactory.Event()
				.AddPickupCollectedEvent(ScoreGranted);
		}
	}
}
