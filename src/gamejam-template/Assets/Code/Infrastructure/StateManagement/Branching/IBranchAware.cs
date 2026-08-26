namespace Code.Infrastructure.StateManagement.Branching
{
	public interface IBranchAware
	{
		void BindBranch(IStateBranch branch);
	}
}
