using System;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Plugins.RequiredField.Editor
{
	public static class RequiredExcludedTypes
	{
		public static readonly Type[] ExcludedTypes =
		{
			typeof(Image),
			typeof(EventSystem),
			typeof(Text),
			typeof(ScrollRect)
		};
	}
}
