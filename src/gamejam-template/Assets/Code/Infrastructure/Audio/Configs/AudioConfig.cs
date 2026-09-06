using UnityEngine;
using SF = UnityEngine.SerializeField;

namespace Code.Infrastructure.Audio.Configs
{
	[CreateAssetMenu(fileName = "AudioConfig", menuName = "Configs/Audio/AudioConfig")]
	public class AudioConfig : ScriptableObject
	{
		[SF] private AudioClip[] meowFallback;
		[SF] private AudioClip duckSqueak;
		[SF] private AudioClip duckThrow;
		[SF] private AudioClip duckLand;
		[SF] private AudioClip classLaugh;
		[SF] private AudioClip pencilSnap;
		[SF] private AudioClip pencilScratch;
		[SF] private AudioClip answerCopied;
		[SF] private AudioClip chalkScratch;
		[SF] private AudioClip teacherHmm;
		[SF] private AudioClip heartbeat;
		[SF] private AudioClip clockTick;
		[SF] private AudioClip schoolBell;
		[SF] private AudioClip teacherFootsteps;
		[SF] private AudioClip timeWarning;

		[SF] private AudioClip classroomMusic;
		[SF] private AudioClip classroomUrgentMusic;

		[SF] private AudioClip introPanelOne;
		[SF] private AudioClip introPanelTwo;
		[SF] private AudioClip introPanelThree;
		[SF] private AudioClip introPanelFour;

		[SF] private float microphoneDuckingMultiplier = 0.25f;
		[SF] private float duckingTransitionSeconds = 0.2f;
		[SF] private float heartbeatThreshold = 0.8f;

		public float MicrophoneDuckingMultiplier => microphoneDuckingMultiplier;
		public float DuckingTransitionSeconds => duckingTransitionSeconds;
		public float HeartbeatThreshold => heartbeatThreshold;

		public AudioClip GetSfx(SfxId id)
		{
			return id switch
			{
				SfxId.MeowFallback => meowFallback[Random.Range(0, meowFallback.Length)],
				SfxId.DuckSqueak => duckSqueak,
				SfxId.DuckThrow => duckThrow,
				SfxId.DuckLand => duckLand,
				SfxId.ClassLaugh => classLaugh,
				SfxId.PencilSnap => pencilSnap,
				SfxId.PencilScratch => pencilScratch,
				SfxId.AnswerCopied => answerCopied,
				SfxId.ChalkScratch => chalkScratch,
				SfxId.TeacherHmm => teacherHmm,
				SfxId.Heartbeat => heartbeat,
				SfxId.ClockTick => clockTick,
				SfxId.SchoolBell => schoolBell,
				SfxId.TeacherFootsteps => teacherFootsteps,
				SfxId.TimeWarning => timeWarning,
				_ => null
			};
		}

		public AudioClip[] GetSfxClips(SfxId id)
		{
			return id switch
			{
				SfxId.MeowFallback => meowFallback,
				SfxId.DuckSqueak => One(duckSqueak),
				SfxId.DuckThrow => One(duckThrow),
				SfxId.DuckLand => One(duckLand),
				SfxId.ClassLaugh => One(classLaugh),
				SfxId.PencilSnap => One(pencilSnap),
				SfxId.PencilScratch => One(pencilScratch),
				SfxId.AnswerCopied => One(answerCopied),
				SfxId.ChalkScratch => One(chalkScratch),
				SfxId.TeacherHmm => One(teacherHmm),
				SfxId.Heartbeat => One(heartbeat),
				SfxId.ClockTick => One(clockTick),
				SfxId.SchoolBell => One(schoolBell),
				SfxId.TeacherFootsteps => One(teacherFootsteps),
				SfxId.TimeWarning => One(timeWarning),
				_ => System.Array.Empty<AudioClip>()
			};
		}

		public AudioClip GetMusic(MusicId id)
		{
			return id switch
			{
				MusicId.Classroom => classroomMusic,
				MusicId.ClassroomUrgent => classroomUrgentMusic,
				_ => null
			};
		}

		public AudioClip GetVoiceOver(VoiceOverId id)
		{
			return id switch
			{
				VoiceOverId.IntroPanelOne => introPanelOne,
				VoiceOverId.IntroPanelTwo => introPanelTwo,
				VoiceOverId.IntroPanelThree => introPanelThree,
				VoiceOverId.IntroPanelFour => introPanelFour,
				_ => null
			};
		}

		private static AudioClip[] One(AudioClip clip) => new[] { clip };
	}
}
