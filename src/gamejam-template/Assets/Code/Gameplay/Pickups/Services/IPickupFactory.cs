using UnityEngine;

namespace Code.Gameplay.Pickups.Services
{
	public interface IPickupFactory
	{
		GameEntity CreatePickup(Vector3 at);
		GameEntity CreateScoreHolder(int score);
	}
}
