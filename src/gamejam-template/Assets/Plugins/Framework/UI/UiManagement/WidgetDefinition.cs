using System;
using Framework.UI.UiManagement.Elements.Windows;

namespace Framework.UI.UiManagement
{
	/// <summary>
	/// Binds a widget class to a <see cref="WidgetConfig"/> by guid. Widget data
	/// (prefab) lives on the config asset.
	/// </summary>
	public record WidgetDefinition
	{
		public readonly Type Type;
		public readonly string Guid;

		public WidgetDefinition(Type type, string guid)
		{
#if UNITY_EDITOR
			if (typeof(WidgetBase).IsAssignableFrom(type) == false)
				throw new ArgumentException($"Type {type.FullName} must inherit from {nameof(WidgetBase)}.");
			if (string.IsNullOrEmpty(guid))
				throw new ArgumentException($"Guid for {type.FullName} cannot be null or empty.");
#endif

			Type = type;
			Guid = guid;
		}
	}
}
