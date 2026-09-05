using System;
using Code.Gameplay.Duck.Services;
using Code.Infrastructure.EntityComponentSystem;
using Entitas;

namespace Code.Gameplay.Duck.Queries
{
	public sealed class DuckQuery : IDuckQuery, IReactiveQuery
	{
		private readonly IDuckConfigsService _duckConfigsService;

		private readonly IGroup<GameEntity> _ducks;
		private readonly IGroup<GameEntity> _changedDucks;

		public event Action<DuckState> OnStateChanged;
		public event Action<int> OnThrowCountChanged;

		public DuckQuery(GameContext game, IDuckConfigsService duckConfigsService)
		{
			_duckConfigsService = duckConfigsService;

			_ducks = game.GetGroup(GameMatcher
				.AllOf(
					GameMatcher.Duck,
					GameMatcher.DuckState,
					GameMatcher.DuckThrowCount));

			_changedDucks = game.GetGroup(GameMatcher
				.AllOf(
					GameMatcher.Duck,
					GameMatcher.DuckState,
					GameMatcher.DuckThrowCount)
				.AnyOf(
					GameMatcher.DuckStateChanged,
					GameMatcher.DuckThrowCountChanged));
		}

		public void ReactToChanges()
		{
			foreach (GameEntity duck in _changedDucks)
			{
				if (duck.isDuckStateChanged)
					OnStateChanged?.Invoke(duck.DuckState);

				if (duck.isDuckThrowCountChanged)
					OnThrowCountChanged?.Invoke(duck.DuckThrowCount);
			}
		}

		public DuckState GetState()
		{
			foreach (GameEntity duck in _ducks)
				return duck.DuckState;

			return DuckState.Confiscated;
		}

		public int GetThrowCount()
		{
			foreach (GameEntity duck in _ducks)
				return duck.DuckThrowCount;

			return 0;
		}

		public float GetDistractionSeconds() => _duckConfigsService.DuckConfig.DistractionSeconds;

		public bool CanThrow() => GetState() == DuckState.OnDesk;
	}
}
