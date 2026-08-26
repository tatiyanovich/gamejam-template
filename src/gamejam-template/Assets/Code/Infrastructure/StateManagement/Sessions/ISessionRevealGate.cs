namespace Code.Infrastructure.StateManagement.Sessions
{
	public interface ISessionRevealGate
	{
		void RegisterPending();
		void NotifyReady();
		void SetNextRevealDelay(float delay);
	}
}
