namespace Code.Infrastructure.Audio.Services
{
	public interface IAudioService
	{
		void PlaySfx(SfxId id);
		void StartLoop(SfxId id);
		void StopLoop(SfxId id);
		void StopAllLoops();
		void PlayMusic(MusicId id);
		void StopMusic();
		void PlayVoiceOver(VoiceOverId id);
		void StopVoiceOver();
		void RefreshSettings();
	}
}
