using Code.Gameplay.Camera.Services;
using Code.Gameplay.CoreLoop;
using Code.Gameplay.Exam.Services;
using Code.Gameplay.Lifetime;
using Code.Gameplay.Meow.Services;
using Code.Gameplay.Suspicion.Services;
using Code.Gameplay.Teacher.Services;
using Code.Infrastructure.EntityComponentSystem.Destruct.Services;
using Code.Infrastructure.EntityComponentSystem.Installers;
using Code.Infrastructure.ErrorHandler;
using Code.Infrastructure.Health;
using Code.Infrastructure.Input;
using Code.Infrastructure.Microphone;
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
			Container.BindInterfacesTo<ExamConfigsService>().AsSingle();
			Container.BindInterfacesTo<MeowConfigsService>().AsSingle();
			Container.BindInterfacesTo<SuspicionConfigsService>().AsSingle();
			Container.BindInterfacesTo<TeacherConfigsService>().AsSingle();
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

		private void BindInstallers()
		{
			new CoreLoopInstaller(Container).InstallBindings();
			new LifetimeInstaller(Container).InstallBindings();
			Container.BindInterfacesTo<CameraQuery>().AsSingle();
			Container.BindInterfacesTo<CameraSwitcher>().AsSingle();
		}

		private void BindInputServices()
		{
			Container.BindInterfacesTo<KeyboardInputService>().AsSingle();
			Container.BindInterfacesTo<MicrophoneService>().AsSingle();
		}
	}
}
