using Entitas;
using Entitas.CodeGeneration.Attributes;
using UnityEngine;

namespace Code.Infrastructure.EntityComponentSystem
{
	[Game, Watched] public class Id : IComponent { [PrimaryEntityIndex] public int Value; }
    [Game, Watched] public class Name : IComponent { public string Value; }
    
    [Game, Watched] public class WorldRotation : IComponent { public Quaternion Value; }
    [Game, Watched] public class WorldPosition : IComponent { public Vector3 Value; }
    [Game, Watched] public class LossyScale : IComponent { public Vector3 Value; }
    
    [Game, Watched] public class LocalRotation : IComponent { public Quaternion Value; }
    [Game, Watched] public class LocalPosition : IComponent { public Vector3 Value; }
    
    [Game, Watched] public class Ready : IComponent { }
}