using Entitas;

namespace Code.Gameplay.Collisions
{
    [Game] public class Collision : IComponent { }
    
    [Game] public class TriggerEnter : IComponent { }
    [Game] public class TriggerExit : IComponent { }
    [Game] public class TriggerStay : IComponent { }
    
    [Game] public class CollisionEnter : IComponent { }
    [Game] public class CollisionExit : IComponent { }
    [Game] public class CollisionStay : IComponent { }
    
    [Game] public class ColliderOwnerId : IComponent { public int Value; }
    [Game] public class CollidedId : IComponent { public int Value; }
}