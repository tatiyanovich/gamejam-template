using Entitas;

namespace Code.Infrastructure.EntityComponentSystem.Factories
{
    public interface ISystemFactory
    {
        T Create<T>() where T : ISystem;
        T Create<T>(params object[] args) where T : ISystem;
    }
}