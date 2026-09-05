using System;
using Code.Infrastructure.EntityComponentSystem;
using Entitas;

namespace Code.Gameplay.Input.Queries
{
	public sealed class InputQuery : IInputQuery, IReactiveQuery
	{
		private readonly IGroup<InputEntity> _leaningInputs;
		private readonly IGroup<InputEntity> _changedInputs;

		public event Action<bool> OnLeanChanged;

		public InputQuery(InputContext input)
		{
			_leaningInputs = input.GetGroup(InputMatcher
				.AllOf(
					InputMatcher.Input,
					InputMatcher.LeanHeld));

			_changedInputs = input.GetGroup(InputMatcher
				.AllOf(
					InputMatcher.Input,
					InputMatcher.LeanHeldChanged));
		}

		public void ReactToChanges()
		{
			foreach (InputEntity input in _changedInputs)
				OnLeanChanged?.Invoke(input.isLeanHeld);
		}

		public bool IsLeaning() => _leaningInputs.count > 0;
	}
}
