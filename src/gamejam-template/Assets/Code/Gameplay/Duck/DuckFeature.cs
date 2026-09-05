using Code.Gameplay.Duck.Systems;
using Code.Infrastructure.EntityComponentSystem.Factories;

namespace Code.Gameplay.Duck
{
	public sealed class DuckFeature : Feature
	{
		public DuckFeature(ISystemFactory systemFactory)
		{
			Add(systemFactory.Create<InitializeDuckSystem>());

			Add(systemFactory.Create<TickDuckStateSystem>());

			Add(systemFactory.Create<CreateThrowDuckRequestOnKeyPressedSystem>());
			Add(systemFactory.Create<ThrowDuckByRequestSystem>());
			Add(systemFactory.Create<DistractTeacherByDuckSystem>());

			Add(systemFactory.Create<LandThrownDuckSystem>());
			Add(systemFactory.Create<ConfiscateDuckOnThirdThrowSystem>());
			Add(systemFactory.Create<PickUpDuckSystem>());
			Add(systemFactory.Create<ReturnDuckSystem>());
		}
	}
}
