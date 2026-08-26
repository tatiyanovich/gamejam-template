using Code.Gameplay.Vfx.Services;
using Code.Infrastructure.EntityComponentSystem.Systems;
using Entitas;

namespace Code.Gameplay.Vfx.Systems
{
	public sealed class PlayVfxByRequestSystem : RequestHandlerSystem<GameEntity>
	{
		private readonly IVfxFactory _vfxFactory;

		public PlayVfxByRequestSystem(GameContext gameContext, IVfxFactory vfxFactory)
			: base(gameContext.GetGroup(GameMatcher
				.AllOf(
					GameMatcher.PlayVfxRequest)))
		{
			_vfxFactory = vfxFactory;
		}

		protected override void OnExecute(IGroup<GameEntity> requests)
		{
			foreach (GameEntity request in requests)
			{
				_vfxFactory.Create(request.playVfxRequest.Request);
			}
		}
	}
}
