using Code.Gameplay.Teacher.Systems;
using Code.Infrastructure.EntityComponentSystem.Factories;

namespace Code.Gameplay.Teacher
{
	public sealed class TeacherFeature : Feature
	{
		public TeacherFeature(ISystemFactory systemFactory)
		{
			Add(systemFactory.Create<InitializeTeacherSystem>());

			Add(systemFactory.Create<ScheduleTeacherCheckSystem>());
			Add(systemFactory.Create<TickTeacherAttentionSystem>());

			Add(systemFactory.Create<AlertTeacherOnMeowSystem>());
			Add(systemFactory.Create<AlertTeacherOnPencilSnapSystem>());
			Add(systemFactory.Create<ExtendTeacherLookOnMeowSystem>());

			Add(systemFactory.Create<TelegraphTeacherTurnSystem>());
			Add(systemFactory.Create<WatchClassSystem>());
			Add(systemFactory.Create<KeepStaringWhileLeaningSystem>());
			Add(systemFactory.Create<ReturnTeacherToWritingSystem>());

			Add(systemFactory.Create<MarkTeacherFacingClassSystem>());
		}
	}
}
