using Code.Gameplay.Vfx.Data;

namespace Code.Gameplay.Vfx.Services
{
	public interface IVfxFactory
	{
		GameEntity Create(VfxSpawnRequest request);
	}
}
