using System;
using Code.Gameplay.Bell.Queries;
using Code.Gameplay.Duck;
using Code.Gameplay.Duck.Queries;
using Code.Gameplay.Exam;
using Code.Gameplay.Exam.Queries;
using Code.Gameplay.Meow.Queries;
using Code.Gameplay.Suspicion.Queries;
using Code.Gameplay.Teacher;
using Code.Gameplay.Teacher.Queries;
using Zenject;

namespace Code.Infrastructure.Audio.Services
{
	public class GameplayAudioPresenter : IInitializable, IDisposable
	{
		private readonly IAudioService _audioService;
		private readonly IAudioConfigsService _audioConfigsService;
		private readonly IExamQuery _examQuery;
		private readonly IMeowQuery _meowQuery;
		private readonly IDuckQuery _duckQuery;
		private readonly ITeacherQuery _teacherQuery;
		private readonly ISuspicionQuery _suspicionQuery;
		private readonly IBellQuery _bellQuery;

		public GameplayAudioPresenter(
			IAudioService audioService,
			IAudioConfigsService audioConfigsService,
			IExamQuery examQuery,
			IMeowQuery meowQuery,
			IDuckQuery duckQuery,
			ITeacherQuery teacherQuery,
			ISuspicionQuery suspicionQuery,
			IBellQuery bellQuery)
		{
			_audioService = audioService;
			_audioConfigsService = audioConfigsService;
			_examQuery = examQuery;
			_meowQuery = meowQuery;
			_duckQuery = duckQuery;
			_teacherQuery = teacherQuery;
			_suspicionQuery = suspicionQuery;
			_bellQuery = bellQuery;
		}

		public void Initialize()
		{
			_examQuery.OnElapsedSecondsChanged += HandleElapsedSecondsChanged;
			_examQuery.OnAnswerProgressChanged += HandleAnswerProgressChanged;
			_examQuery.OnAnswerCopied += HandleAnswerCopied;
			_examQuery.OnWrongInput += HandleWrongInput;
			_examQuery.OnExamFinished += HandleExamFinished;
			_meowQuery.OnMeow += HandleMeow;
			_duckQuery.OnStateChanged += HandleDuckStateChanged;
			_teacherQuery.OnAttentionChanged += HandleTeacherAttentionChanged;
			_suspicionQuery.OnLevelChanged += HandleSuspicionLevelChanged;
			_bellQuery.OnAnnounced += HandleBellAnnounced;
		}

		public void Dispose()
		{
			_examQuery.OnElapsedSecondsChanged -= HandleElapsedSecondsChanged;
			_examQuery.OnAnswerProgressChanged -= HandleAnswerProgressChanged;
			_examQuery.OnAnswerCopied -= HandleAnswerCopied;
			_examQuery.OnWrongInput -= HandleWrongInput;
			_examQuery.OnExamFinished -= HandleExamFinished;
			_meowQuery.OnMeow -= HandleMeow;
			_duckQuery.OnStateChanged -= HandleDuckStateChanged;
			_teacherQuery.OnAttentionChanged -= HandleTeacherAttentionChanged;
			_suspicionQuery.OnLevelChanged -= HandleSuspicionLevelChanged;
			_bellQuery.OnAnnounced -= HandleBellAnnounced;
		}

		private void HandleElapsedSecondsChanged(float elapsedSeconds)
		{
			MusicId music = _bellQuery.IsAnnounced() ? MusicId.ClassroomUrgent : MusicId.Classroom;
			_audioService.PlayMusic(music);
		}

		private void HandleAnswerProgressChanged(int questionIndex, int progress, int length)
		{
			if (progress > 0)
				_audioService.PlaySfx(SfxId.PencilScratch);
		}

		private void HandleAnswerCopied(int questionIndex)
		{
			_audioService.PlaySfx(SfxId.AnswerCopied);
		}

		private void HandleWrongInput(int questionIndex)
		{
			_audioService.PlaySfx(SfxId.PencilSnap);
		}

		private void HandleExamFinished(ExamOutcome outcome)
		{
			_audioService.StopMusic();
			_audioService.StopAllLoops();

			if (outcome == ExamOutcome.BellRang)
				_audioService.PlaySfx(SfxId.SchoolBell);
		}

		private void HandleMeow(bool fromMicrophone)
		{
			if (fromMicrophone == false)
				_audioService.PlaySfx(SfxId.MeowFallback);
		}

		private void HandleDuckStateChanged(DuckState state)
		{
			switch (state)
			{
				case DuckState.Flying:
					_audioService.PlaySfx(SfxId.DuckThrow);
					break;
				case DuckState.OnFloor:
					_audioService.PlaySfx(SfxId.DuckLand);
					_audioService.PlaySfx(SfxId.DuckSqueak);
					_audioService.PlaySfx(SfxId.ClassLaugh);
					break;
			}
		}

		private void HandleTeacherAttentionChanged(TeacherAttention attention)
		{
			switch (attention)
			{
				case TeacherAttention.Writing:
					_audioService.StopLoop(SfxId.TeacherFootsteps);
					_audioService.PlaySfx(SfxId.ChalkScratch);
					break;
				case TeacherAttention.Turning:
				case TeacherAttention.Alerted:
					_audioService.PlaySfx(SfxId.TeacherHmm);
					break;
				case TeacherAttention.Distracted:
					_audioService.StartLoop(SfxId.TeacherFootsteps);
					break;
			}
		}

		private void HandleSuspicionLevelChanged(float level)
		{
			float normalized = level / _suspicionQuery.GetMaximumLevel();
			if (normalized >= _audioConfigsService.AudioConfig.HeartbeatThreshold)
				_audioService.StartLoop(SfxId.Heartbeat);
			else
				_audioService.StopLoop(SfxId.Heartbeat);
		}

		private void HandleBellAnnounced()
		{
			_audioService.PlayMusic(MusicId.ClassroomUrgent);
			_audioService.PlaySfx(SfxId.TimeWarning);
		}
	}
}
