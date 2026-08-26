using Code.Infrastructure.CoreLoop;

namespace Code.Infrastructure.StateManagement.Branching
{
	public interface IBranchedStateMachine
	{
		LoopNodeId ActiveNode { get; }

		IStateBranch OpenBranch(LoopNodeId nodeId);
		bool TryGetBranch(LoopNodeId nodeId, out IStateBranch branch);
		void SetActiveNode(LoopNodeId nodeId);
		void CloseBranch(LoopNodeId nodeId);
		void CloseAll();

		void ExecuteBranches();
		void FixedExecuteBranches();
		void LateExecuteBranches();
	}
}
