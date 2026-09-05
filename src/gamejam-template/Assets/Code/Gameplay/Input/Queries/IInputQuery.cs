using System;

namespace Code.Gameplay.Input.Queries
{
	public interface IInputQuery
	{
		event Action<bool> OnLeanChanged;

		bool IsLeaning();
	}
}
