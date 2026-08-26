namespace Code.Infrastructure.CoreLoop
{
	public class LoopNodeContext : ILoopNodeContext
	{
		public LoopNodeId Current { get; set; } = LoopNodeId.Unknown;
	}
}
