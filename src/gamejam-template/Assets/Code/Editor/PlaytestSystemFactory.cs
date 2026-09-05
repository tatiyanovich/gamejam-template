using Code.Infrastructure.EntityComponentSystem.Factories;
using Entitas;
using Zenject;

namespace Code.Editor
{
	public class PlaytestSystemFactory : ISystemFactory
	{
		private readonly DiContainer _container;

		public PlaytestSystemFactory(DiContainer container)
		{
			_container = container;
		}

		public T Create<T>() where T : ISystem => _container.Instantiate<T>();

		public T Create<T>(params object[] arguments) where T : ISystem => _container.Instantiate<T>(arguments);
	}
}
