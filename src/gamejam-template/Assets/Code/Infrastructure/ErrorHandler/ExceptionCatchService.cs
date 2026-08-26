using System;
using Code.Infrastructure.StateManagement;

namespace Code.Infrastructure.ErrorHandler
{
	public class ExceptionCatchService : IDisposable
	{
		private readonly IGameStateMachine _gameStateMachine;
		private readonly ILogGuardService _logGuardService;

		public ExceptionCatchService(
			IGameStateMachine gameStateMachine,
			ILogGuardService logGuardService)
		{
			_gameStateMachine = gameStateMachine;
			_logGuardService = logGuardService;

			_logGuardService.OnFirstExceptionHappened += HandleFirstException;
		}

		public void Dispose()
		{
			_logGuardService.OnFirstExceptionHappened -= HandleFirstException;
		}

		private void HandleFirstException()
		{
			_gameStateMachine.Enter<ErrorHappenedState>();
		}
	}
}