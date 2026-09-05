using Code.Gameplay.Meow.Systems;
using Code.Infrastructure.EntityComponentSystem.Factories;

namespace Code.Gameplay.Meow
{
	public sealed class MeowFeature : Feature
	{
		public MeowFeature(ISystemFactory systemFactory)
		{
			Add(systemFactory.Create<InitializeMeowSourceSystem>());

			Add(systemFactory.Create<SampleMicrophoneLevelSystem>());

			Add(systemFactory.Create<EmitMeowOnLoudMicrophoneSystem>());
			Add(systemFactory.Create<EmitMeowOnKeyPressedSystem>());
			Add(systemFactory.Create<RearmMeowOnQuietMicrophoneSystem>());
		}
	}
}
