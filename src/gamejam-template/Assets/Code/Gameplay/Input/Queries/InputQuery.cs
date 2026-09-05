using Entitas;

namespace Code.Gameplay.Input.Queries
{
	public sealed class InputQuery : IInputQuery
	{
		private readonly IGroup<InputEntity> _leaningInputs;

		public InputQuery(InputContext input)
		{
			_leaningInputs = input.GetGroup(InputMatcher
				.AllOf(
					InputMatcher.Input,
					InputMatcher.LeanHeld));
		}

		public bool IsLeaning() => _leaningInputs.count > 0;
	}
}
