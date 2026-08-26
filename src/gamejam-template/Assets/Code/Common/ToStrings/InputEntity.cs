using System;
using System.Collections.Generic;
using System.Text;
using Code.Common.ToStrings;
using Code.Gameplay.Input;
using Entitas;

// ReSharper disable once CheckNamespace
public sealed partial class InputEntity : INamedEntity
{
	private EntityPrinter _printer;

	private static readonly Dictionary<string, Func<InputEntity, string>> _componentPrinters = new()
	{
		{nameof(Input), entity => entity.PrintInput()},
	};

	public override string ToString()
	{
		_printer ??= new EntityPrinter(this);

		return _printer.BuildToString();
	}

	public string EntityName(IComponent[] components)
	{
		if (components.Length == 0)
			return "Unknown";

		foreach (IComponent component in components)
		{
			if (_componentPrinters.TryGetValue(component.GetType().Name, out Func<InputEntity, string> printer))
			{
				return printer(this);
			}
		}

		return components[0].GetType().Name;
	}

	private string PrintInput() => BuildEntityString("Input", hasHorizontalAxis ? $"H:{HorizontalAxis}" : null);

	private string BuildEntityString(string entityType, params string[] parts)
	{
		StringBuilder stringBuilder = new(entityType);
		foreach (string part in parts)
		{
			if (!string.IsNullOrEmpty(part))
			{
				stringBuilder.Append(" ").Append(part);
			}
		}

		return stringBuilder.ToString();
	}
}