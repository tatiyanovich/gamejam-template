using System.Collections.Generic;
using Code.Gameplay.Difficulty.Services;
using Code.Infrastructure.EntityComponentSystem.Events.Extensions;
using Entitas;

namespace Code.Gameplay.Neighbours.Systems
{
	public class LiftPawOnMeowSystem : IExecuteSystem
	{
		private readonly IDifficultyService _difficultyService;

		private readonly IGroup<GameEntity> _meowEvents;
		private readonly IGroup<GameEntity> _runningExams;
		private readonly IGroup<GameEntity> _questions;
		private readonly IGroup<GameEntity> _neighbours;

		private readonly List<GameEntity> _questionBuffer = new(1);
		private readonly List<GameEntity> _neighbourBuffer = new(2);

		public LiftPawOnMeowSystem(GameContext game, IDifficultyService difficultyService)
		{
			_difficultyService = difficultyService;

			_runningExams = game.GetGroup(GameMatcher
				.AllOf(
					GameMatcher.ExamRun)
				.NoneOf(
					GameMatcher.ExamFinished));

			_meowEvents = game.GetEvents(GameMatcher
				.AllOf(
					GameMatcher.MeowEvent));

			_questions = game.GetGroup(GameMatcher
				.AllOf(
					GameMatcher.Question,
					GameMatcher.QuestionIndex,
					GameMatcher.AnswerNeighbourSide)
				.NoneOf(
					GameMatcher.AnswerCopied));

			_neighbours = game.GetGroup(GameMatcher
				.AllOf(
					GameMatcher.Neighbour,
					GameMatcher.NeighbourSide,
					GameMatcher.PawWindowTimeLeft));
		}

		public void Execute()
		{
			if (_meowEvents.count == 0 || _runningExams.count == 0)
				return;

			foreach (GameEntity question in _questions.GetEntities(_questionBuffer))
			{
				LiftPaw(question.AnswerNeighbourSide, _difficultyService.GetPhase(question.QuestionIndex).PawWindow);
			}
		}

		private void LiftPaw(NeighbourSide side, float pawWindow)
		{
			foreach (GameEntity neighbour in _neighbours.GetEntities(_neighbourBuffer))
			{
				if (neighbour.NeighbourSide != side)
					continue;

				neighbour.isPawLifted = true;
				neighbour.ReplacePawWindowTimeLeft(pawWindow);
			}
		}
	}
}
