using UnityEngine;

namespace Code.Gameplay.Input.Data
{
	public readonly struct KeyBinding<TValue>
	{
		public readonly KeyCode Key;
		public readonly TValue Value;

		public KeyBinding(KeyCode key, TValue value)
		{
			Key = key;
			Value = value;
		}
	}
}
