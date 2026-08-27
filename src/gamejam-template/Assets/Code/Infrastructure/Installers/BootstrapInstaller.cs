using Code.Gameplay.Camera.Services;
using Code.Gameplay.CoreLoop;
using Code.Gameplay.Fuel;
using Code.Gameplay.Fuel.Services;
using Code.Gameplay.Lifetime;
using Code.Gameplay.Pickups;
using Code.Gameplay.Pickups.Services;
using Code.Gameplay.Player;
using Code.Gameplay.Player.Services;
using Code.Infrastructure.EntityComponentSystem.Destruct.Services;
using Code.Infrastructure.EntityComponentSystem.Installers;
using Code.Infrastructure.ErrorHandler;
using Code.Infrastructure.Health;
using Code.Infrastructure.Input;
using Code.Infrastructure.Randomization;
using Code.Infrastructure.Satellite;
using Code.Infrastructure.Settings.Services;
using Code.Infrastructure.Scenes;
using Code.Infrastructure.StateManagement;
using Code.Infrastructure.StateManagement.Branching;
using Code.Infrastructure.StateManagement.Sessions;
using Code.Infrastructure.UiManagement.Services;
using Code.Storage.SaveFiles;
using Code.Storage.Services;
using Framework.AssetManagement;
using Framework.Essentials.CursorManagement;
using Framework.Essentials.SceneManagement;
using Framework.Essentials.TimeManagement;
using Framework.Essentials.ViewManagement;
using Framework.Instantiation;
using Framework.StateManagement.Factories;
using Framework.Storage;
using Framework.UI.UiManagement;
using Framework.UI.UiManagement.Services;
using Zenject;
using SF = UnityEngine.SerializeField;

namespace Code.Infrastructure.Installers
{
	// Lives on the Boot scene and survives every scene swap. Anything a state, a query or a
	// service needs across scenes is bound here; per-scene bindings go to GameplayInstaller.
	public class BootstrapInstaller : MonoInstaller
	{
		[SF] private UiHolder uiHolder;

		public override void InstallBindings()
		{
			BindInfrastructureServices();
			BindInputServices();
			BindConfigServices();
		}

		private void BindConfigServices()
		{
			Container.BindInterfacesTo<CameraConfigsService>().AsSingle();
			Container.BindInterfacesTo<PlayerConfigsService>().AsSingle();
			Container.BindInterfacesTo<PickupConfigsService>().AsSingle();
			Container.BindInterfacesTo<FuelConfigsService>().AsSingle();
		}

		private void BindInfrastructureServices()
		{
			Container.BindInterfacesTo<EntryPoint>().AsSingle();
			Container.BindInterfacesTo<GameStateMachine>().AsSingle();
			Container.BindInterfacesTo<StateFactory>().AsSingle();
			Container.BindInterfacesTo<ApplicationHealthService>().AsSingle();
			Container.BindInterfacesTo<ExceptionCatchService>().AsSingle();
			Container.BindInterfacesTo<LogGuardService>().AsSingle();

			Contexts contexts = new();
			new EcsInstaller(Container, contexts).InstallBindings();

			BindInstallers();

			Container.BindInterfacesTo<LoopEntityWipeService>().AsSingle();
			Container.BindInterfacesTo<BranchedStateMachine>().AsSingle();
			Container.BindInterfacesTo<SessionService>().AsSingle();
			Container.BindInterfacesTo<SessionWindowsPresenter>().AsSingle();
			Container.BindInterfacesTo<SessionRevealGate>().AsSingle();

			Container.BindInterfacesTo<InstantiateService>().AsSingle();

			Container.BindInterfacesTo<SceneLoadService>().AsSingle();
			Container.BindInterfacesTo<LoadedSceneRegistry>().AsSingle();
			new ViewManagementInstaller(Container).InstallBindings();

			Container.BindInterfacesTo<AssetsService>().AsSingle();

			new UiInstaller(Container, UiLayers.AllLayers, uiHolder).InstallBindings();
			Container.BindInterfacesTo<CursorLockService>().AsSingle();
			Container.BindInterfacesTo<UnityTimeService>().AsSingle();
			Container.BindInterfacesTo<RandomService>().AsSingle();

			Container.BindInterfacesTo<FallbackBaseCameraService>().AsSingle();
			Container.BindInterfacesTo<SettingsService>().AsSingle();
			Container.BindInterfacesTo<SatelliteService>().AsSingle();

			new SaveManagementInstaller(Container, new()
			{
				typeof(GeneralSaveFile),
				typeof(SettingsSaveFile)
			})
				.InstallBindings();

			Container.BindInterfacesTo<RefreshStorageService>()
				.FromNewComponentOnNewGameObject()
				.UnderTransform(transform)
				.AsSingle();
		}

		// Feature installers live here rather than in GameplayInstaller because their queries and
		// services are resolved by states and windows that outlive any single gameplay scene.
		private void BindInstallers()
		{
			new CoreLoopInstaller(Container).InstallBindings();
			new LifetimeInstaller(Container).InstallBindings();
			new PlayerInstaller(Container).InstallBindings();
			new PickupsInstaller(Container).InstallBindings();
			new FuelInstaller(Container).InstallBindings();

			Container.BindInterfacesTo<CameraQuery>().AsSingle();
			Container.BindInterfacesTo<CameraSwitcher>().AsSingle();
		}

		private void BindInputServices()
		{
			Container.BindInterfacesTo<JoystickInputService>().AsSingle();
		}
	}
}
