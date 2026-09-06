using System.Threading;
using Cysharp.Threading.Tasks;

namespace Code.Infrastructure.Audio.Services
{
	public interface IAudioService
	{
		void PlaySfx(SfxId id);
		void StopSfx();
		void StartLoop(SfxId id);
		void StopLoop(SfxId id);
		void StopAllLoops();
		void PlayMusic(MusicId id);
		void StopMusic();
		UniTask PlayVoiceOver(VoiceOverId id, CancellationToken cancellationToken = default);
		void StopVoiceOver();
		void RefreshSettings();
	}
}
