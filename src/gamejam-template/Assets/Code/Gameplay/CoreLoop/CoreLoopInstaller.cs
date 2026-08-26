using Code.Gameplay.CoreLoop.Services;
using Framework.Essentials.DependencyInjection;
using Zenject;

namespace Code.Gameplay.CoreLoop
{
    public class CoreLoopInstaller : PlainAbstractInstaller
    {
        public CoreLoopInstaller(DiContainer container) : base(container)
        {

        }

        public override void InstallBindings()
        {
            Container.BindInterfacesTo<CoreLoopRequestFactory>().AsSingle();
            Container.BindInterfacesTo<LoopNodeTransitionService>().AsSingle();
        }
    }
}