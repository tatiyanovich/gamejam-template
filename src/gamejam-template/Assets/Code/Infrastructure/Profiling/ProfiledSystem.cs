using System.Diagnostics;
using Entitas;

namespace Code.Infrastructure.Profiling
{
	/// <summary>
	/// Decorator that measures how long the wrapped system spends in each Entitas phase
	/// and forwards the timings to <see cref="SystemProfilingRecorder"/>.
	/// Only the phases the inner system actually implements are timed and recorded.
	/// </summary>
	public sealed class ProfiledSystem : IInitializeSystem, IExecuteSystem, ICleanupSystem, ITearDownSystem
	{
		private readonly string _systemName;
		private readonly IInitializeSystem _initializeSystem;
		private readonly IExecuteSystem _executeSystem;
		private readonly ICleanupSystem _cleanupSystem;
		private readonly ITearDownSystem _tearDownSystem;

		public ProfiledSystem(ISystem system)
		{
			_systemName = system.GetType().Name;
			_initializeSystem = system as IInitializeSystem;
			_executeSystem = system as IExecuteSystem;
			_cleanupSystem = system as ICleanupSystem;
			_tearDownSystem = system as ITearDownSystem;
		}

		public void Initialize()
		{
			if (_initializeSystem == null)
				return;

			if (SystemProfilingRecorder.Enabled == false)
			{
				_initializeSystem.Initialize();
				return;
			}

			long startTimestamp = Stopwatch.GetTimestamp();
			_initializeSystem.Initialize();
			SystemProfilingRecorder.Instance.Record(_systemName, ProfilePhase.Initialize, Stopwatch.GetTimestamp() - startTimestamp);
		}

		public void Execute()
		{
			if (_executeSystem == null)
				return;

			if (SystemProfilingRecorder.Enabled == false)
			{
				_executeSystem.Execute();
				return;
			}

			long startTimestamp = Stopwatch.GetTimestamp();
			_executeSystem.Execute();
			SystemProfilingRecorder.Instance.Record(_systemName, ProfilePhase.Execute, Stopwatch.GetTimestamp() - startTimestamp);
		}

		public void Cleanup()
		{
			if (_cleanupSystem == null)
				return;

			if (SystemProfilingRecorder.Enabled == false)
			{
				_cleanupSystem.Cleanup();
				return;
			}

			long startTimestamp = Stopwatch.GetTimestamp();
			_cleanupSystem.Cleanup();
			SystemProfilingRecorder.Instance.Record(_systemName, ProfilePhase.Cleanup, Stopwatch.GetTimestamp() - startTimestamp);
		}

		public void TearDown()
		{
			if (_tearDownSystem == null)
				return;

			if (SystemProfilingRecorder.Enabled == false)
			{
				_tearDownSystem.TearDown();
				return;
			}

			long startTimestamp = Stopwatch.GetTimestamp();
			_tearDownSystem.TearDown();
			SystemProfilingRecorder.Instance.Record(_systemName, ProfilePhase.TearDown, Stopwatch.GetTimestamp() - startTimestamp);
		}
	}
}
