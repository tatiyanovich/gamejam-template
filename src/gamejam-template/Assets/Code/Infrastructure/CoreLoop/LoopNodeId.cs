namespace Code.Infrastructure.CoreLoop
{
	// One entry per scene the core loop can run. StartLaunch runs as a plain pipeline;
	// every other node runs as a session branch — see RunLoopSceneState.RunsAsSession.
	public enum LoopNodeId
	{
		Unknown = 0,
		StartLaunch = 1,
		Battle = 2
	}
}
