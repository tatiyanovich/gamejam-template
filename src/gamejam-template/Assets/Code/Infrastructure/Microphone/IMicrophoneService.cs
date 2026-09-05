namespace Code.Infrastructure.Microphone
{
	public interface IMicrophoneService
	{
		bool IsAvailable { get; }

		float GetRootMeanSquare();
	}
}
