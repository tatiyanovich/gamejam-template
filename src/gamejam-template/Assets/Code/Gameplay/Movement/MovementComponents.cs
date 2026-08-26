using Entitas;
using Entitas.CodeGeneration.Attributes;
using UnityEngine;

namespace Code.Gameplay.Movement
{
	[Game] public class RigidbodyVelocityMovement : IComponent { }
	[Game] public class RigidbodyVelocity2DMovement : IComponent { }
	[Game] public class RigidbodyInterpolatedMovement : IComponent { }
	[Game] public class RigidbodyPositionMovement : IComponent { }
	[Game] public class TransformMovement : IComponent { }
	[Game] public class TransformLateMovement : IComponent { }
	[Game] public class KinematicMovement : IComponent { }

	[Game] public class MovementDirection : IComponent { public Vector3 Value; }
	[Game, Watched] public class LookDirection : IComponent { public Vector3 Value; }
	[Game, Watched] public class MovementSpeed : IComponent { public float Value; }
	[Game] public class MaxMovementSpeed : IComponent { public float Value; }
	[Game] public class MinMovementSpeed : IComponent { public float Value; }
	[Game] public class MovementAcceleration : IComponent { public float Value; }
	[Game] public class RotationSpeed : IComponent { public float Value; }
	[Game] public class RotationSharpness : IComponent { public float Value; }
	[Game] public class MaxTurnRate : IComponent { public float Value; }
	[Game] public class TurnAcceleration : IComponent { public float Value; }
	[Game] public class TurnSpeed : IComponent { public float Value; }
	[Game] public class TurnSpeedReference : IComponent { public float Value; }
	[Game] public class MinTurnFactor : IComponent { public float Value; }
	[Game] public class Heading : IComponent { public float Value; }
	[Game] public class MovementSlowDownModifier : IComponent { public float Value; }
	[Game] public class MovementSpeedUpModifier : IComponent { public float Value; }
	[Game] public class MovementSpeedMultiplier : IComponent { public float Value; }
	[Game] public class RotationSpeedMultiplier : IComponent { public float Value; }
	[Game] public class MovementStep : IComponent { public Vector3 Value; }

	[Game] public class TargetVelocity : IComponent { public Vector3 Value; }

	[Game, Watched] public class Velocity : IComponent { public Vector3 Value; }

	[Game] public class Moving : IComponent { }
	[Game] public class CanMove : IComponent { }

	[Game] public class SmoothFollowMovement : IComponent { }
	[Game] public class FollowOffset : IComponent { public Vector3 Value; }
	[Game] public class FollowSmoothSpeed : IComponent { public float Value; }

	[Game] public class Impulse : IComponent { public Vector2 Value; }
}
