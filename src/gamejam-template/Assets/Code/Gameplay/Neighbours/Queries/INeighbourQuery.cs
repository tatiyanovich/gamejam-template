using System;

namespace Code.Gameplay.Neighbours.Queries
{
	public interface INeighbourQuery
	{
		event Action<NeighbourSide, bool, float> OnPawChanged;

		bool IsPawLifted(NeighbourSide side);
		float GetPawWindowTimeLeft(NeighbourSide side);
		float GetPawWindowProgress(NeighbourSide side);
	}
}
