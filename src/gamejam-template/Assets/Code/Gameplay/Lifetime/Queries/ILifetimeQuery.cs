using System;

namespace Code.Gameplay.Lifetime.Queries
{
	public interface ILifetimeQuery
	{
		event Action<int, float> OnEntityDamaged;
		event Action<int, float> OnEntityHealed;
		event Action<int> OnEntityDied;
		float GetCurrentHp(int entityId);
		float GetMaxHp(int entityId);
		bool IsFullHealth(int entityId);
		bool IsAlive(int entityId);
	}
}
