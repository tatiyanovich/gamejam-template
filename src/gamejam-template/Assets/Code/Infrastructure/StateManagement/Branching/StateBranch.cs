using System;
using System.Collections.Generic;
using Framework.StateManagement;
using Framework.StateManagement.Factories;
using UnityEngine;

namespace Code.Infrastructure.StateManagement.Branching
{
	public sealed class StateBranch : IStateBranch
	{
		private readonly IStateFactory _stateFactory;
		private readonly Dictionary<Type, IState> _createdStates = new();
		private readonly bool _useStateCaching;
		private readonly string _name;

		private IState _activeState;
		private Action _pendingTransition;

		public IState Current => _activeState;

		public event Action<IState> OnStateEntered;
		public event Action<IState> OnStateExited;

		public StateBranch(IStateFactory stateFactory, string name, bool useStateCaching = true)
		{
			_stateFactory = stateFactory;
			_name = name;
			_useStateCaching = useStateCaching;
		}

		public void Dispose()
		{
			_pendingTransition = null;
			ExitActiveState();
			_createdStates.Clear();
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

		public void Enter<TState>() where TState : IState
		{
			if (_activeState is TState)
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

			if (_useStateCaching && _createdStates.TryGetValue(typeof(TState), out IState cached))
			{
				_activeState = cached;
			}
			else
			{
				_activeState = _stateFactory.Create<TState>();
				(_activeState as IBranchAware)?.BindBranch(this);

				if (_useStateCaching)
					_createdStates[typeof(TState)] = _activeState;
			}

			Debug.Log($"<color=orange>Branch {_name}</color> -> <color=cyan>{typeof(TState).Name}</color>");

			return _activeState;
		}

		private void ExitActiveState()
		{
			if (_activeState == null)
				return;

			(_activeState as IExit)?.Exit();
			OnStateExited?.Invoke(_activeState);
			_activeState = null;
		}
	}
}
