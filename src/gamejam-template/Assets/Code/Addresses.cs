using Code.Infrastructure.CoreLoop;

namespace Code
{
	public static class Addresses
	{
		public const string CameraPrefab = "camera_prefab";

		public static class Labels
		{
			public const string UI = "ui";
		}

		public static class UI
		{
			public const string FadeWindow = "f8cb932a-43f6-4d7c-a179-1a5deb54f709";
			public const string LoadingWindow = "ba55290e-f182-4355-a74e-43e716dadad8";
			public const string LaunchWindow = "6d73ca02-6b75-4f1c-b1b6-52c5b72b1c19";
			public const string GameplayWindow = "01bfb656-1111-4f5a-9e0e-4b28f2ccd4d9";
			public const string WorldOverlayWindow = "ffb30672-1a48-4c1a-a8f6-fbeaa607f3cf";
			public const string ResultWindow = "d42e63dc-a28d-43ca-9042-4c10d2bc25a5";
			public const string ErrorWindow = "c600db1d-870e-4976-ade5-812c93188d53";
		}

		public static class SceneNames
		{
			public const string LaunchScene = "launch_scene";
			public const string GameplayScene = "gameplay_scene";

			public static string Boot => "Boot";

			public static string GetLoopScene(LoopNodeId loopNodeId)
			{
				return loopNodeId switch
				{
					LoopNodeId.StartLaunch => LaunchScene,
					LoopNodeId.Exam => GameplayScene,
					_ => GameplayScene
				};
			}
		}
	}
}
