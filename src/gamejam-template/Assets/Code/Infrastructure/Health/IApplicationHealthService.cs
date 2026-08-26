namespace Code.Infrastructure.Health
{
	public interface IApplicationHealthService
	{
		bool HasCriticalErrors { get; }
	}
}
