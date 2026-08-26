using Code.Gameplay.Camera.Behaviours;
using Code.Gameplay.Camera.Scene;
using Entitas;
using UnityEngine;

namespace Code.Gameplay.Camera
{
	[Game] public class CameraComponent : IComponent { public UnityEngine.Camera Value; }
	[Game] public class CameraViewComponent : IComponent { public CameraView Value; }
	[Game] public class CameraShakeAnimatorComponent : IComponent { public CameraShakeAnimator Value; }
	[Game] public class CameraShakeRequest : IComponent { public CameraShakeTypeId Value; }
	[Game] public class CameraShakeScale : IComponent { public float Value; }
}
