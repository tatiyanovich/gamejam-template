using Entitas;
using Entitas.CodeGeneration.Attributes;
using UnityEngine;

namespace Code.Common
{
	[Game] public class TransformComponent : IComponent { public Transform Value; }
	[Game] public class RigidbodyComponent : IComponent { public Rigidbody Value; }
	[Game] public class Rigidbody2DComponent : IComponent { public Rigidbody2D Value; }
	[Game] public class ParticleSystemComponent : IComponent { public ParticleSystem Value; }
	[Game] public class MeshRendererComponent : IComponent { public MeshRenderer Value; }
	[Game] public class MeshFilterComponent : IComponent { public MeshFilter Value; }

	[Game] public class PreviousWorldPosition : IComponent { public Vector3 Value; }

	[Game] public class AimDirection : IComponent { public Vector3 Value; }

	[Game] public class TargetId : IComponent { public int Value; }
	[Game] public class ProducerId : IComponent { public int Value; }
	[Game] public class KillerId : IComponent { public int Value; }
	[Game] public class OwnerId : IComponent { [EntityIndex] public int Value; }
	
	[Game] public class SceneEntityGuid : IComponent { [EntityIndex] public string Value; }

	[Game, Watched] public class Processed : IComponent { }
	[Game, Watched] public class Started : IComponent { }
	[Game, Watched] public class Completed : IComponent { }
	
	[Game, Watched] public class Hidden : IComponent { }
	
	[Game] public class Full : IComponent { }
	[Game] public class Capacity : IComponent { public int Value; }
	[Game, Watched] public class Amount : IComponent { public int Value; }

	[Game] public class Occupied : IComponent { }
}
