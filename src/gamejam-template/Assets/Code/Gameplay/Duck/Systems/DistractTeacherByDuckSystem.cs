using System.Collections.Generic;
using Code.Gameplay.Duck.Services;
using Code.Gameplay.Teacher;
using Code.Infrastructure.EntityComponentSystem.Events.Extensions;
using Entitas;

namespace Code.Gameplay.Duck.Systems
{
	public class DistractTeacherByDuckSystem : IExecuteSystem
	{
		private readonly IDuckConfigsService _duckConfigsService;

		private readonly IGroup<GameEntity> _duckThrownEvents;
		private readonly IGroup<GameEntity> _teachers;

		private readonly List<GameEntity> _buffer = new(1);

		public DistractTeacherByDuckSystem(GameContext game, IDuckConfigsService duckConfigsService)
		{
			_duckConfigsService = duckConfigsService;

			_duckThrownEvents = game.GetEvents(GameMatcher
				.AllOf(
					GameMatcher.DuckThrownEvent));

			_teachers = game.GetGroup(GameMatcher
				.AllOf(
					GameMatcher.Teacher,
					GameMatcher.TeacherAttention));
		}

		public void Execute()
		{
			if (_duckThrownEvents.count == 0)
				return;

			foreach (GameEntity teacher in _teachers.GetEntities(_buffer))
			{
				teacher.SwitchAttention(
					TeacherAttention.Distracted,
					_duckConfigsService.DuckConfig.DistractionSeconds);
			}
		}
	}
}
