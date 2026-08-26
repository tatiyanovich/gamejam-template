using Entitas;
using Framework.Instantiation;

namespace Code.Infrastructure.EntityComponentSystem.Factories
{
    public class SystemFactory : ISystemFactory
    {
        private readonly IInstantiateService _instantiateService;

        public SystemFactory(IInstantiateService instantiateService)
        {
            _instantiateService = instantiateService;
        }
        
        public T Create<T>() where T : ISystem => _instantiateService.Instantiate<T>();
        public T Create<T>(params object[] args) where T : ISystem => _instantiateService.Instantiate<T>(args);
    }
}