namespace Code.Infrastructure.CoreLoop
{
	public static class LoopNodes
	{
		// A persistent node keeps its feature pipeline and its scene alive when the branch closes,
		// so re-entering it resumes instead of rebuilding. Nothing needs that yet in the template.
		public static bool IsPersistent(LoopNodeId nodeId) => false;
	}
}
