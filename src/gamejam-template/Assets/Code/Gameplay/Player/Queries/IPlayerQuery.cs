using UnityEngine;

namespace Code.Gameplay.Player.Queries
{
	public interface IPlayerQuery
	{
		bool TryGetPlayerPosition(out Vector3 position);
	}
}
