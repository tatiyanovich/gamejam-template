namespace Framework.StateManagement.Factories
{
	public interface IStateFactory
	{
		T Create<T>() where T : IState;
		T Create<T>(object[] args) where T : IState;
	}
}
