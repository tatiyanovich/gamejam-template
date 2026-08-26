using Code.UI.Fade;
using Cysharp.Threading.Tasks;
using Framework.UI.UiManagement.Services;

namespace Code.Infrastructure.StateManagement.Sessions
{
	public class SessionRevealGate : ISessionRevealGate
	{
		private readonly IUiService _uiService;

		private int _pending;
		private float _nextRevealDelay;

		private const float RevealDuration = 1f;

		public SessionRevealGate(IUiService uiService)
		{
			_uiService = uiService;
		}

		public void RegisterPending() => _pending++;

		public void SetNextRevealDelay(float delay) => _nextRevealDelay = delay;

		public void NotifyReady()
		{
			if (_pending > 0)
				_pending--;

			if (_pending > 0)
				return;

			Reveal();
		}

		private void Reveal()
		{
			float delay = _nextRevealDelay;
			_nextRevealDelay = 0f;

			FadeWindow fadeWindow = _uiService.GetWindow<FadeWindow>();

			if (fadeWindow == null)
				return;

			fadeWindow.FadeOut(RevealDuration, delay).Forget();
		}
	}
}
