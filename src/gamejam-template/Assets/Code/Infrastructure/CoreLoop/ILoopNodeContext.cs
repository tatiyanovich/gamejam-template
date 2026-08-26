namespace Code.Infrastructure.CoreLoop
{
	public interface ILoopNodeContext
	{
		LoopNodeId Current { get; set; }
	}
}
