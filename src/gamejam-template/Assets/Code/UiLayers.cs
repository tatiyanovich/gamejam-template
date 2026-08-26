using System.Collections.Generic;

namespace Code
{
	public static class UiLayers
	{
		public const string Input = "Input";
		public const string Main = "Main";
		public const string Overlay = "Overlay";

		public static readonly List<string> AllLayers = new()
		{
			Input,
			Main,
			Overlay
		};
	}
}
