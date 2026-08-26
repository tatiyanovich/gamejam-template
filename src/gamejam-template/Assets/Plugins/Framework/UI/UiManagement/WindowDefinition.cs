using System;
using Framework.UI.UiManagement.Elements.Windows;

namespace Framework.UI.UiManagement
{
	/// <summary>
	/// Binds a window class to a <see cref="WindowConfig"/> by guid. Window data
	/// (prefab, layer, flags) lives on the config asset.
	/// </summary>
	public record WindowDefinition
	{
		public readonly Type Type;
		public readonly string Guid;

		public WindowDefinition(Type type, string guid)
		{
#if UNITY_EDITOR
			if (typeof(WindowBase).IsAssignableFrom(type) == false)
				throw new ArgumentException($"Type {type.FullName} must inherit from {nameof(WindowBase)}.");
			if (string.IsNullOrEmpty(guid))
				throw new ArgumentException($"Guid for {type.FullName} cannot be null or empty.");
#endif

			Type = type;
			Guid = guid;
		}
	}
}
