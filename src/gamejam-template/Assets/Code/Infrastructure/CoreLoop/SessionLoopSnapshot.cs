using System;
using Code.Infrastructure.CoreLoop;

namespace Code.Gameplay.CoreLoop
{
	[Serializable]
	public class SessionLoopSnapshot
	{
		public bool HasCompletedFirstLaunch;
		public LoopNodeId CurrentNode;

		public SessionLoopSnapshot(bool hasCompletedFirstLaunch, LoopNodeId currentNode)
		{
			HasCompletedFirstLaunch = hasCompletedFirstLaunch;
			CurrentNode = currentNode;
		}

		public static SessionLoopSnapshot CreateForNewProgress()
		{
			return new SessionLoopSnapshot(
				hasCompletedFirstLaunch: false,
				currentNode: LoopNodeId.StartLaunch);
		}

		public static SessionLoopSnapshot CreateForLegacyProgress()
		{
			return new SessionLoopSnapshot(
				hasCompletedFirstLaunch: true,
				currentNode: LoopNodeId.Exam);
		}
	}
}
