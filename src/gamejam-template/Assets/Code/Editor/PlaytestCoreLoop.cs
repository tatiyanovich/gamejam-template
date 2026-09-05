using System.Collections.Generic;
using Code.Gameplay.Camera.Services;
using Code.Gameplay.CoreLoop.Services;
using Code.Infrastructure.CoreLoop;
using UnityEngine;

namespace Code.Editor
{
	public class PlaytestCoreLoop : ICoreLoopRequestFactory, ICameraSwitcher
	{
		public List<string> Calls { get; } = new(4);

		public void CreateGoToNodeRequest(LoopNodeId loopNodeId) => Calls.Add("node:" + loopNodeId);

		public void CreateGoToBranchRequest(LoopNodeId loopNodeId) => Calls.Add("branch:" + loopNodeId);

		public void CreateCloseBranchRequest(LoopNodeId loopNodeId) => Calls.Add("close:" + loopNodeId);

		public void RegisterCamera(LoopNodeId node, Camera camera) { }

		public void UnregisterCamera(LoopNodeId node) { }

		public void SwitchTo(LoopNodeId node) => Calls.Add("camera:" + node);
	}
}
