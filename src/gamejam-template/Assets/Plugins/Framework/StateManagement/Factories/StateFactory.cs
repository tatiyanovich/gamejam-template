using Framework.Instantiation;

namespace Framework.StateManagement.Factories
{
	public class StateFactory : IStateFactory
	{
		private readonly IInstantiateService _instantiateService;

		public StateFactory(IInstantiateService instantiateService)
		{
			_instantiateService = instantiateService;
		}

		public T Create<T>() where T : IState => _instantiateService.Instantiate<T>();
		public T Create<T>(object[] args) where T : IState => _instantiateService.Instantiate<T>(args);
	}
}
