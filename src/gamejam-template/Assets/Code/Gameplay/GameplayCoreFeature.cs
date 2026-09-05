using Code.Gameplay.Camera.Systems;
using Code.Gameplay.Exam;
using Code.Gameplay.Meow;
using Code.Gameplay.Movement;
using Code.Gameplay.Neighbours;
using Code.Gameplay.Teacher;
using Code.Gameplay.Teardown;
using Code.Gameplay.UI;
using Code.Infrastructure.EntityComponentSystem.Factories;
using Code.Infrastructure.EntityComponentSystem.SceneEntities.Systems;

namespace Code.Gameplay
{
	public sealed class GameplayCoreFeature : Feature
	{
		public GameplayCoreFeature(ISystemFactory systemFactory)
		{
			Add(systemFactory.Create<InitializeSceneEntitiesByRequestSystem>());

			Add(systemFactory.Create<InitializeExamCameraSystem>());

			Add(systemFactory.Create<MeowFeature>());

			Add(systemFactory.Create<NeighboursFeature>());

			Add(systemFactory.Create<TeacherFeature>());

			Add(systemFactory.Create<ExamFeature>());

			Add(systemFactory.Create<MovementUpdateFeature>());

			Add(systemFactory.Create<UIFeature>());

			Add(systemFactory.Create<GameplayTeardownSystem>());
		}
	}
}
