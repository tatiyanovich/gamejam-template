using Entitas;

namespace Code.Infrastructure.Profiling
{
	/// <summary>
	/// Drop-in replacement for <see cref="Feature"/> that wraps every directly added system
	/// in a <see cref="ProfiledSystem"/> so its per-phase execution time is recorded, and flushes
	/// the aggregated report to disk periodically and on tear down.
	/// Granularity equals the items added to the feature: when the children are themselves features
	/// (as in GameplayCoreFeature) the report is a per-feature breakdown.
	/// </summary>
	public abstract class ProfilingFeature : Feature
	{
		public override Systems Add(ISystem system)
		{
			return base.Add(new ProfiledSystem(system));
		}

		public override void Initialize()
		{
			SystemProfilingRecorder.Instance.Reset();
			base.Initialize();
		}

		public override void Execute()
		{
			base.Execute();
			SystemProfilingRecorder.Instance.OnFrameComplete();
		}

		public override void TearDown()
		{
			base.TearDown();
			SystemProfilingRecorder.Instance.WriteReport();
		}
	}
}
