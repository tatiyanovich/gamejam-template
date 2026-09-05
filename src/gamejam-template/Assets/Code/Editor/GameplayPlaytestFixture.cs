using System;
using Code.Gameplay;
using Code.Gameplay.Duck.Services;
using Code.Gameplay.Exam.Services;
using Code.Gameplay.Neighbours.Services;
using Code.Gameplay.Progress.Services;
using Code.Gameplay.Teacher.Services;
using Code.Infrastructure.CoreLoop;
using Code.Infrastructure.EntityComponentSystem.Factories;
using Code.Infrastructure.EntityComponentSystem.Events.Systems;
using Code.Infrastructure.EntityComponentSystem.Identifiers;
using Code.Common.Cooldown;
using Code.Gameplay.Lifetime;
using Framework.Essentials.TimeManagement;
using Zenject;

namespace Code.Editor
{
	public class GameplayPlaytestFixture : IDisposable
	{
		public GameContext Game { get; } = new();
		public InputContext Input { get; } = new();
		public PlaytestTimeService Time { get; } = new();
		public DiContainer Container { get; }
		public GameplayCoreFeature Core { get; }
		public IExamFactory Exams { get; }
		public GameEntity Run { get; }
		public InputEntity Keyboard { get; }

		public GameplayPlaytestFixture(DiContainer parent)
		{
			Container = new DiContainer(parent);
			Container.BindInstance(Game);
			Container.BindInstance(Input);
			Container.Bind<ITimeService>().FromInstance(Time);
			Container.Bind<ILoopNodeContext>().FromInstance(new LoopNodeContext());
			Container.Bind<IIdentifierService>().To<IdentifierService>().AsSingle();
			Container.Bind<ISystemFactory>().FromInstance(new PlaytestSystemFactory(Container));
			Container.BindInterfacesTo<EntityFactory>().AsSingle();
			Container.BindInterfacesTo<ExamFactory>().AsSingle();
			Container.BindInterfacesTo<NeighbourFactory>().AsSingle();
			Container.BindInterfacesTo<TeacherFactory>().AsSingle();
			Container.BindInterfacesTo<DuckFactory>().AsSingle();
			Container.BindInterfacesTo<ProgressFactory>().AsSingle();
			Exams = Container.Resolve<IExamFactory>();
			Run = Exams.CreateRun();
			Keyboard = Container.Resolve<IEntityFactory>().Input();
			Core = Container.Instantiate<GameplayCoreFeature>();
		}

		public void Dispose()
		{
			Game.DestroyAllEntities();
			Input.DestroyAllEntities();
		}

		public void Tick(float seconds)
		{
			Time.DeltaTime = seconds;
			Container.Instantiate<EventsReadySystem>().Execute();
			Core.Execute();
			Container.Instantiate<CooldownFeature>().Execute();
			Container.Instantiate<LifetimeFeature>().Execute();
			Container.Instantiate<EventsCleanupSystem>().Cleanup();
		}
	}
}
