using Code.Gameplay.Exam.Services;
using Entitas;

namespace Code.Gameplay.Exam.Systems
{
	public class InitializeExamRunSystem : IInitializeSystem
	{
		private readonly IExamFactory _examFactory;

		private readonly IGroup<GameEntity> _runs;

		public InitializeExamRunSystem(GameContext game, IExamFactory examFactory)
		{
			_examFactory = examFactory;

			_runs = game.GetGroup(GameMatcher
				.AllOf(
					GameMatcher.ExamRun));
		}

		public void Initialize()
		{
			if (_runs.count > 0)
				return;

			_examFactory.CreateRun();
		}
	}
}
