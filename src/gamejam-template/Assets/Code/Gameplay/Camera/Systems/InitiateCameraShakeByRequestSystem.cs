using System.Collections.Generic;
using Code.Gameplay.Camera.Configs;
using Code.Gameplay.Camera.Services;
using Code.Infrastructure.EntityComponentSystem.Systems;
using Entitas;
using UnityEngine;

namespace Code.Gameplay.Camera.Systems
{
	public sealed class InitiateCameraShakeByRequestSystem : RequestHandlerSystem<GameEntity>
	{
		private readonly IGroup<GameEntity> _cameras;
		private readonly List<GameEntity> _buffer = new(4);
		private readonly ICameraConfigsService _configs;

		public InitiateCameraShakeByRequestSystem(GameContext game, ICameraConfigsService configs)
			: base(game.GetGroup(GameMatcher
				.AllOf(
					GameMatcher.CameraShakeRequest,
					GameMatcher.CameraShakeScale)))
		{
			_configs = configs;
			_cameras = game.GetGroup(GameMatcher
				.AllOf(
					GameMatcher.Camera,
					GameMatcher.CameraView,
					GameMatcher.CameraShakeAnimator));
		}

		protected override void OnExecute(IGroup<GameEntity> requests)
		{
			foreach (GameEntity request in requests)
			{
				ShakeConfig config = _configs.GetShakeConfig(request.CameraShakeRequest);

				if (config == null)
				{
					Debug.LogError($"Camera shake config for type {request.CameraShakeRequest} was not found.");
					continue;
				}

				foreach (GameEntity camera in _cameras.GetEntities(_buffer))
				{
					if (camera.Camera.isActiveAndEnabled == false)
						continue;

					camera.CameraShakeAnimator.Play(config, request.CameraShakeScale);
				}
			}
		}
	}
}
