using System;
using Code.Gameplay.Camera.Services;
using Code.Gameplay.Exam;
using Code.Gameplay.Greybox;
using Code.Gameplay.Greybox.Services;
using UnityEngine;
using Zenject;

namespace Code.Editor
{
	public class GreyboxPlaytestView : IDisposable
	{
		private readonly GameContext _game;
		private readonly GreyboxFeature _feature;
		private GameObject _art;
		private UnityEngine.Camera _camera;
		private float _originalCameraSize;

		public SceneContext Scene { get; }

		public GreyboxPlaytestView(SceneContext scene)
		{
			Scene = scene;
			_game = scene.Container.Resolve<GameContext>();
			DiContainer container = new(scene.Container);
			container.BindInterfacesTo<GreyboxBoardFactory>().AsSingle();
			_feature = new GreyboxFeature(new PlaytestSystemFactory(container));
			_feature.Initialize();
		}

		public void Update()
		{
			if (_art == null)
			{
				_art = GameObject.Find("CopycatArt");
				if (_art != null)
					_art.SetActive(false);
			}
			UnityEngine.Camera camera = Scene.Container.Resolve<ICameraQuery>().GetCamera();
			if (camera != null && camera != _camera)
			{
				_camera = camera;
				_originalCameraSize = camera.orthographicSize;
				camera.orthographicSize = 7.5f;
			}
			foreach (GameEntity board in _game.GetGroup(GameMatcher
				.AllOf(
					GameMatcher.GreyboxBoard)))
				board.GreyboxBoard.SetOutcome(ExamOutcome.None);
			_feature.Execute();
		}

		public void Dispose()
		{
			if (_art != null)
				_art.SetActive(true);
			if (_camera != null)
				_camera.orthographicSize = _originalCameraSize;
			foreach (GameEntity board in _game.GetGroup(GameMatcher
				.AllOf(
					GameMatcher.GreyboxBoard)).GetEntities())
			{
				if (board.GreyboxBoard != null)
					UnityEngine.Object.Destroy(board.GreyboxBoard.gameObject);
				board.Destroy();
			}
		}
	}
}
