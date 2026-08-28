using Code.Gameplay.Camera.Services;
using Code.Gameplay.Debugging.Services;
using Code.Gameplay.Movement.Services;
using Code.Gameplay.Drilling.Services;
using Code.Gameplay.Player.Services;
using Code.Gameplay.Vfx.Services;
using Framework.Instantiation;
using Zenject;

namespace Code.Infrastructure.Installers
{
	public class GameplayInstaller : MonoInstaller
	{
		public override void InstallBindings()
		{
			Container.BindInterfacesTo<InstantiatorSetter>().AsSingle();

			BindFactories();
			BindServices();
			BindDebugServices();
		}

		private void BindServices()
		{
			Container.BindInterfacesTo<KinematicCollision2DResolver>().AsSingle();
		}

		private void BindFactories()
		{
			Container.BindInterfacesTo<CameraFactory>().AsSingle();
			Container.BindInterfacesTo<VfxFactory>().AsSingle();
			Container.BindInterfacesTo<PlayerFactory>().AsSingle();
			Container.BindInterfacesTo<DrillRunFactory>().AsSingle();
		}

		private void BindDebugServices()
		{
			Container.BindInterfacesTo<TriggerCameraShakeDebugAction>().AsSingle();
			Container.BindInterfacesTo<BattleSessionChangeDebugAction>().AsSingle();
		}
	}
}
