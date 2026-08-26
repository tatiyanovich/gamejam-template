using System;
using System.Collections.Generic;
using Code.Infrastructure.CoreLoop;
using Code.Infrastructure.EntityComponentSystem.Destruct.Services;
using Framework.StateManagement.Factories;

namespace Code.Infrastructure.StateManagement.Branching
{
	public sealed class BranchedStateMachine : IBranchedStateMachine
	{
		private readonly IStateFactory _stateFactory;
		private readonly ILoopEntityWipeService _loopEntityWipeService;
		private readonly ILoopNodeContext _loopNodeContext;

		private readonly Dictionary<LoopNodeId, IStateBranch> _branches = new(4);

		private readonly List<LoopNodeId> _tickOrder = new(4);

		public LoopNodeId ActiveNode { get; private set; } = LoopNodeId.Unknown;

		public BranchedStateMachine(
			IStateFactory stateFactory,
			ILoopEntityWipeService loopEntityWipeService,
			ILoopNodeContext loopNodeContext)
		{
			_stateFactory = stateFactory;
			_loopEntityWipeService = loopEntityWipeService;
			_loopNodeContext = loopNodeContext;
		}

		public IStateBranch OpenBranch(LoopNodeId nodeId)
		{
			if (_branches.TryGetValue(nodeId, out IStateBranch existing))
				return existing;

			IStateBranch branch = new StateBranch(_stateFactory, nodeId.ToString());
			_branches[nodeId] = branch;

			return branch;
		}

		public bool TryGetBranch(LoopNodeId nodeId, out IStateBranch branch) =>
			_branches.TryGetValue(nodeId, out branch);

		public void SetActiveNode(LoopNodeId nodeId) => ActiveNode = nodeId;

		public void CloseBranch(LoopNodeId nodeId)
		{
			if (_branches.TryGetValue(nodeId, out IStateBranch branch) == false)
				return;

			branch.Dispose();
			_branches.Remove(nodeId);

			if (ActiveNode == nodeId)
				ActiveNode = LoopNodeId.Unknown;

			_loopEntityWipeService.WipeNodeScopedEntities(nodeId);
		}

		public void CloseAll()
		{
			List<LoopNodeId> open = new(_branches.Keys);

			foreach (LoopNodeId nodeId in open)
				CloseBranch(nodeId);
		}

		public void ExecuteBranches() => TickBranches(branch => branch.Tick());

		public void FixedExecuteBranches() => TickBranches(branch => branch.FixedTick());

		public void LateExecuteBranches() => TickBranches(branch => branch.LateTick());

		private void TickBranches(Action<IStateBranch> tick)
		{
			CollectTickOrder();

			foreach (LoopNodeId nodeId in _tickOrder)
			{
				if (_branches.TryGetValue(nodeId, out IStateBranch branch) == false)
					continue;

				_loopNodeContext.Current = nodeId;

				try
				{
					tick(branch);
				}
				finally
				{
					_loopNodeContext.Current = LoopNodeId.Unknown;
				}
			}
		}

		private void CollectTickOrder()
		{
			_tickOrder.Clear();

			foreach (LoopNodeId nodeId in _branches.Keys)
				if (nodeId == ActiveNode || LoopNodes.IsPersistent(nodeId))
					_tickOrder.Add(nodeId);
		}
	}
}
