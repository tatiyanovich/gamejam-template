using System;
using Framework.StateManagement;

namespace Code.Infrastructure.StateManagement.Branching
{
	public interface IStateBranch : IDisposable
	{
		IState Current { get; }

		event Action<IState> OnStateEntered;
		event Action<IState> OnStateExited;

		void Enter<TState>() where TState : IState;
		void Enter<TState, TPayload>(TPayload payload) where TState : IState, IPayloadedEnter<TPayload>;

		void Tick();
		void FixedTick();
		void LateTick();
	}
}
