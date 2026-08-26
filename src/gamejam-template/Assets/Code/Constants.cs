namespace Code
{
	public static class Constants
	{
		public const string GameName = "GameTemplate";

		public static class EditorPrefsKeys
		{
			public const string InitialScene = "InitialScene";
		}
		
		public static class GraphicsQuality
		{
			public const int LowFramerate = 30;
			public const int MidFramerate = 60;
			public const int MaxFramerate = 120;
		}
		
		// TODO: Replace these placeholders with your game's real support links before shipping.
		public static class Support
		{
			public const string DiscordServerUrl = "https://discord.gg/your-invite";
			public const string SupportEmailUrl = "mailto:{0}?subject={1}&body={2}";
			public const string SupportEmail = "support@example.com";
			public const string SupportEmailSubject = "Subject:";
			public const string SupportEmailBody = "Issue: \n\n\n--- Do not erase this information ---\nModel: {0}\nOS: {1}\nVersion: {2}\nDevice ID: {3}";

			public const string SupportRequestUrl = "https://example.com/support?form={0}&a={1}&b={2}&c={3}&d={4}";
		}
	}
}
