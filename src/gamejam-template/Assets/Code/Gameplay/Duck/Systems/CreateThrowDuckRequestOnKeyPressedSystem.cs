using Code.Gameplay.Duck.Services;
using Code.Gameplay.Input;
using Entitas;

namespace Code.Gameplay.Duck.Systems
{
	public class CreateThrowDuckRequestOnKeyPressedSystem : IExecuteSystem
	{
		private readonly IDuckFactory _duckFactory;

		private readonly IGroup<InputEntity> _duckKeyInputs;

		public CreateThrowDuckRequestOnKeyPressedSystem(InputContext input, IDuckFactory duckFactory)
		{
			_duckFactory = duckFactory;

			_duckKeyInputs = input.GetGroup(InputMatcher
				.AllOf(
					InputMatcher.Input,
					InputMatcher.DuckKeyPressed));
		}

		public void Execute()
		{
			if (_duckKeyInputs.count == 0)
				return;

			_duckFactory.CreateThrowDuckRequest();
		}
	}
}
