using Code.Infrastructure.EntityComponentSystem;
using Code.Infrastructure.EntityComponentSystem.Extensions;

namespace Code.Gameplay.Player.Providers
{
	public class SpawnPointProvider : EntityComponentProvider
	{
		public override void RegisterComponents()
		{
			Entity
				.AddSpawnPoint(transform)
				.ReplaceWorldPosition(transform.position);
		}

		public override void UnregisterComponents()
		{
			Entity
				.SafeRemoveSpawnPoint()
				.SafeRemoveWorldPosition();
		}
	}
}
