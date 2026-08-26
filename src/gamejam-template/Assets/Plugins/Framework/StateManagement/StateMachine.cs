using System;
using System.Collections.Generic;
using Framework.StateManagement.Factories;
using UnityEngine;
using Zenject;

namespace Framework.StateManagement
{
	public abstract class StateMachine : IStateMachine, ITickable, IFixedTickable, ILateTickable, IDisposable
	{
		private readonly Dictionary<Type, IState> _createdStates = new();
		private IState _activeState;
		private Action _pendingTransition;

		protected readonly IStateFactory StateFactory;

		protected abstract string Name { get; }
		protected virtual bool DebugLogsEnabled => false;
		protected virtual bool UseStateCaching => true;

		public event Action<IState> OnStateEntered;
		public event Action<IState> OnStateExited;

		protected StateMachine(IStateFactory stateFactory)
		{
			StateFactory = stateFactory;
		}

		public virtual void Dispose()
		{
			_pendingTransition = null;
			ExitActiveState();
			_createdStates.Clear();

			if (DebugLogsEnabled)
				Debug.Log($"<color=green>{Name}</color> disposed");
		}

		public void Tick() => (_activeState as IExecutable)?.Execute();

		public void FixedTick() => (_activeState as IFixedExecutable)?.FixedExecute();

		public void LateTick()
		{
			(_activeState as ILateExecutable)?.LateExecute();

			if (_pendingTransition != null)
			{
				Action transition = _pendingTransition;
				_pendingTransition = null;
				transition();
			}
		}

		public IState GetCurrentState() => _activeState;

		public void Enter<TState>() where TState : IState
		{
			if (GetCurrentState() is TState)
			{
				_pendingTransition = null;
				return;
			}

			PerformOrDeferTransition(() =>
			{
				IState state = ChangeActiveStateTo<TState>();
				(state as IEnter)?.Enter();
				OnStateEntered?.Invoke(_activeState);
			});
		}

		public void Enter<TState, TPayload>(TPayload payload)
			where TState : IState, IPayloadedEnter<TPayload>
		{
			PerformOrDeferTransition(() =>
			{
				IState state = ChangeActiveStateTo<TState>();
				(state as IPayloadedEnter<TPayload>)?.Enter(payload);
				OnStateEntered?.Invoke(_activeState);
			});
		}

		public void Enter<TState, TPayload1, TPayload2>(TPayload1 payload1, TPayload2 payload2)
			where TState : IState, IPayloadedEnter<TPayload1, TPayload2>
		{
			PerformOrDeferTransition(() =>
			{
				IState state = ChangeActiveStateTo<TState>();
				(state as IPayloadedEnter<TPayload1, TPayload2>)?.Enter(payload1, payload2);
				OnStateEntered?.Invoke(_activeState);
			});
		}

		public void Enter<TState, TPayload1, TPayload2, TPayload3>(TPayload1 payload1, TPayload2 payload2, TPayload3 payload3)
			where TState : IState, IPayloadedEnter<TPayload1, TPayload2, TPayload3>
		{
			PerformOrDeferTransition(() =>
			{
				IState state = ChangeActiveStateTo<TState>();
				(state as IPayloadedEnter<TPayload1, TPayload2, TPayload3>)?.Enter(payload1, payload2, payload3);
				OnStateEntered?.Invoke(_activeState);
			});
		}

		protected virtual IState CreateState<TState>() where TState : IState
		{
			return StateFactory.Create<TState>();
		}

		private void PerformOrDeferTransition(Action transition)
		{
			if (_activeState is IEndOfFrameExit)
			{
				_pendingTransition = transition;
				return;
			}

			transition();
		}

		private IState ChangeActiveStateTo<TState>() where TState : IState
		{
			ExitActiveState();

			if (UseStateCaching && _createdStates.TryGetValue(typeof(TState), out IState state))
			{
				_activeState = state;
			}
			else
			{
				_activeState = CreateState<TState>();

				if (UseStateCaching)
					_createdStates[typeof(TState)] = _activeState;
			}

			if (DebugLogsEnabled)
				Debug.Log($"<color=green>{Name}</color> switched to <color=cyan>{typeof(TState).Name}</color> state");

			return _activeState;
		}

		private void ExitActiveState()
		{
			if (_activeState != null)
			{
				(_activeState as IExit)?.Exit();
				OnStateExited?.Invoke(_activeState);
				_activeState = null;
			}
		}
	}
}
