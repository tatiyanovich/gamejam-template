using Code.Infrastructure.CoreLoop;
using Code.Infrastructure.EntityComponentSystem.Physics;
using Code.Infrastructure.EntityComponentSystem.Factories;
using Code.Infrastructure.EntityComponentSystem.Identifiers;
using Framework.Essentials.DependencyInjection;
using Zenject;

namespace Code.Infrastructure.EntityComponentSystem.Installers
{
	public class EcsInstaller : PlainAbstractInstaller
	{
		private readonly Contexts _contexts;

		public EcsInstaller(DiContainer container, Contexts contexts) : base(container)
		{
			_contexts = contexts;
		}

		public override void InstallBindings()
		{
			Container.BindInterfacesAndSelfTo<Contexts>().FromInstance(_contexts).AsSingle();
			Container.BindInterfacesAndSelfTo<GameContext>().FromInstance(_contexts.game).AsSingle();
			Container.BindInterfacesAndSelfTo<InputContext>().FromInstance(_contexts.input).AsSingle();

			Container.BindInterfacesTo<LoopNodeContext>().AsSingle();
			Container.BindInterfacesTo<EntityFactory>().AsSingle();

			Container.BindInterfacesTo<EntityCollidersRegistry>().AsSingle();
			Container.BindInterfacesTo<EntityPhysicsService>().AsSingle();

			Container.BindInterfacesTo<EntityViewFactory>().AsSingle();
			Container.BindInterfacesTo<IdentifierService>().AsSingle();

			Container.BindInterfacesTo<SystemFactory>().AsSingle();
		}
	}
}
