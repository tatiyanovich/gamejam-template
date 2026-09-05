using System.Collections.Generic;
using Code.Common.Cooldown;
using Code.Gameplay.Meow.Services;
using Code.Infrastructure.EntityComponentSystem.Extensions;
using Code.Infrastructure.EntityComponentSystem.Factories;
using Entitas;

namespace Code.Gameplay.Meow.Systems
{
	public class EmitMeowOnKeyPressedSystem : IExecuteSystem
	{
		private readonly IEntityFactory _entityFactory;
		private readonly IMeowConfigsService _meowConfigsService;

		private readonly IGroup<GameEntity> _meowSources;
		private readonly IGroup<InputEntity> _meowKeyInputs;

		private readonly List<GameEntity> _buffer = new(1);

		public EmitMeowOnKeyPressedSystem(
			GameContext game,
			InputContext input,
			IEntityFactory entityFactory,
			IMeowConfigsService meowConfigsService)
		{
			_entityFactory = entityFactory;
			_meowConfigsService = meowConfigsService;

			_meowSources = game.GetGroup(GameMatcher
				.AllOf(
					GameMatcher.MeowSource)
				.NoneOf(
					GameMatcher.OnCooldown));

			_meowKeyInputs = input.GetGroup(InputMatcher
				.AllOf(
					InputMatcher.Input,
					InputMatcher.MeowKeyPressed));
		}

		public void Execute()
		{
			if (_meowKeyInputs.count == 0)
				return;

			foreach (GameEntity meowSource in _meowSources.GetEntities(_buffer))
			{
				meowSource.PutOnCooldown(_meowConfigsService.MeowConfig.CooldownSeconds);

				_entityFactory.Event()
					.With(x => x.isMeowEvent = true);
			}
		}
	}
}
