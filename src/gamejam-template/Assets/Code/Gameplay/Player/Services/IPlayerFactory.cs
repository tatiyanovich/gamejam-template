using UnityEngine;

namespace Code.Gameplay.Player.Services
{
	public interface IPlayerFactory
	{
		GameEntity CreatePlayer(Vector3 at);
	}
}
