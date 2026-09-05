using System;
using Code.Gameplay.Difficulty.Services;
using Code.Infrastructure.EntityComponentSystem;
using Entitas;
using UnityEngine;

namespace Code.Gameplay.Neighbours.Queries
{
	public sealed class NeighbourQuery : INeighbourQuery, IReactiveQuery
	{
		private readonly IDifficultyService _difficultyService;

		private readonly IGroup<GameEntity> _runs;
		private readonly IGroup<GameEntity> _neighbours;
		private readonly IGroup<GameEntity> _changedNeighbours;

		public event Action<NeighbourSide, bool, float> OnPawChanged;

		public NeighbourQuery(GameContext game, IDifficultyService difficultyService)
		{
			_difficultyService = difficultyService;

			_runs = game.GetGroup(GameMatcher
				.AllOf(
					GameMatcher.ExamRun,
					GameMatcher.CurrentQuestionIndex));

			_neighbours = game.GetGroup(GameMatcher
				.AllOf(
					GameMatcher.Neighbour,
					GameMatcher.NeighbourSide,
					GameMatcher.PawWindowTimeLeft));

			_changedNeighbours = game.GetGroup(GameMatcher
				.AllOf(
					GameMatcher.Neighbour,
					GameMatcher.NeighbourSide,
					GameMatcher.PawWindowTimeLeft)
				.AnyOf(
					GameMatcher.PawLiftedChanged,
					GameMatcher.PawWindowTimeLeftChanged));
		}

		public void ReactToChanges()
		{
			foreach (GameEntity neighbour in _changedNeighbours)
			{
				OnPawChanged?.Invoke(
					neighbour.NeighbourSide,
					neighbour.isPawLifted,
					neighbour.PawWindowTimeLeft);
			}
		}

		public bool IsPawLifted(NeighbourSide side)
		{
			GameEntity neighbour = GetNeighbour(side);
			return neighbour != null && neighbour.isPawLifted;
		}

		public float GetPawWindowTimeLeft(NeighbourSide side)
		{
			GameEntity neighbour = GetNeighbour(side);
			return neighbour == null ? 0f : neighbour.PawWindowTimeLeft;
		}

		public float GetPawWindowProgress(NeighbourSide side)
		{
			float duration = GetPawWindowDuration();

			if (duration <= 0f)
				return 0f;

			return Mathf.Clamp01(GetPawWindowTimeLeft(side) / duration);
		}

		private GameEntity GetNeighbour(NeighbourSide side)
		{
			foreach (GameEntity neighbour in _neighbours)
			{
				if (neighbour.NeighbourSide == side)
					return neighbour;
			}

			return null;
		}

		private float GetPawWindowDuration()
		{
			foreach (GameEntity run in _runs)
				return _difficultyService.GetPhase(run.CurrentQuestionIndex).PawWindow;

			return 0f;
		}
	}
}
