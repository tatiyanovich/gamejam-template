using Code.Gameplay.Neighbours.Services;
using Entitas;

namespace Code.Gameplay.Neighbours.Systems
{
	public class InitializeNeighboursSystem : IInitializeSystem
	{
		private readonly INeighbourFactory _neighbourFactory;

		private readonly IGroup<GameEntity> _neighbours;

		public InitializeNeighboursSystem(GameContext game, INeighbourFactory neighbourFactory)
		{
			_neighbourFactory = neighbourFactory;

			_neighbours = game.GetGroup(GameMatcher
				.AllOf(
					GameMatcher.Neighbour));
		}

		public void Initialize()
		{
			if (_neighbours.count > 0)
				return;

			_neighbourFactory.CreateNeighbour(NeighbourSide.Left);
			_neighbourFactory.CreateNeighbour(NeighbourSide.Right);
		}
	}
}
