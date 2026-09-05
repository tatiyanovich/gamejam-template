using Code.Gameplay.Camera.Services;
using Code.Gameplay.Debugging.Services;
using Code.Gameplay.Difficulty.Services;
using Code.Gameplay.Exam.Services;
using Code.Gameplay.Movement.Services;
using Code.Gameplay.Neighbours.Services;
using Code.Gameplay.Teacher.Services;
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
			Container.BindInterfacesTo<DifficultyService>().AsSingle();
		}

		private void BindFactories()
		{
			Container.BindInterfacesTo<CameraFactory>().AsSingle();
			Container.BindInterfacesTo<VfxFactory>().AsSingle();
			Container.BindInterfacesTo<ExamFactory>().AsSingle();
			Container.BindInterfacesTo<NeighbourFactory>().AsSingle();
			Container.BindInterfacesTo<TeacherFactory>().AsSingle();
		}

		private void BindDebugServices()
		{
			Container.BindInterfacesTo<TriggerCameraShakeDebugAction>().AsSingle();
			Container.BindInterfacesTo<ExamSessionChangeDebugAction>().AsSingle();
		}
	}
}
