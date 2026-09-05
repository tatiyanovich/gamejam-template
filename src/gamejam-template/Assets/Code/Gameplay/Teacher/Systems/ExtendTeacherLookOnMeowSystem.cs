using System.Collections.Generic;
using Code.Gameplay.Meow;
using Code.Gameplay.Teacher.Services;
using Code.Infrastructure.EntityComponentSystem.Events.Extensions;
using Entitas;

namespace Code.Gameplay.Teacher.Systems
{
	public class ExtendTeacherLookOnMeowSystem : IExecuteSystem
	{
		private readonly ITeacherConfigsService _teacherConfigsService;

		private readonly IGroup<GameEntity> _meowEvents;
		private readonly IGroup<GameEntity> _scheduledTeachers;

		private readonly List<GameEntity> _buffer = new(1);

		public ExtendTeacherLookOnMeowSystem(GameContext game, ITeacherConfigsService teacherConfigsService)
		{
			_teacherConfigsService = teacherConfigsService;

			_meowEvents = game.GetEvents(GameMatcher
				.AllOf(
					GameMatcher.MeowEvent));

			_scheduledTeachers = game.GetGroup(GameMatcher
				.AllOf(
					GameMatcher.Teacher,
					GameMatcher.TeacherAttention,
					GameMatcher.TeacherAttentionTimeLeft));
		}

		public void Execute()
		{
			if (_meowEvents.count == 0)
				return;

			float lookExtensionSeconds = _teacherConfigsService.TeacherConfig.MeowLookExtensionSeconds;

			foreach (GameEntity teacher in _scheduledTeachers.GetEntities(_buffer))
			{
				if (IsFacingClass(teacher.TeacherAttention) == false)
					continue;

				teacher.ReplaceTeacherAttentionTimeLeft(teacher.TeacherAttentionTimeLeft + lookExtensionSeconds);
			}
		}

		private static bool IsFacingClass(TeacherAttention attention)
		{
			return attention == TeacherAttention.Watching || attention == TeacherAttention.Staring;
		}
	}
}
