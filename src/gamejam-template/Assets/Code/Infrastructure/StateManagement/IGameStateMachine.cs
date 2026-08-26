using Framework.StateManagement;

namespace Code.Infrastructure.StateManagement
{
	public interface IGameStateMachine : IStateMachine
	{
		void RebootGame();
	}
}