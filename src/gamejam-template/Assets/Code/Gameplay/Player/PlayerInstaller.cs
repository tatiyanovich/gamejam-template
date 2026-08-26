using Code.Gameplay.Player.Queries;
using Framework.Essentials.DependencyInjection;
using Zenject;

namespace Code.Gameplay.Player
{
	public class PlayerInstaller : PlainAbstractInstaller
	{
		public PlayerInstaller(DiContainer container) : base(container)
		{
		}

		public override void InstallBindings() => Container.BindInterfacesTo<PlayerQuery>().AsSingle();
	}
}
