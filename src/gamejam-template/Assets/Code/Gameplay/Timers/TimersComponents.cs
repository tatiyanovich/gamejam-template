using Entitas;

namespace Code.Gameplay.Timers
{
  [Game] public class Timer : IComponent { }
  [Game] public class Interval : IComponent { public float Value; }
  [Game] public class TimeLeft : IComponent { public float Value; } 
  [Game] public class TimeIsUp : IComponent { } 
  [Game] public class IntervalUp : IComponent { }
  [Game] public class Ticking : IComponent { }
  
  [Game] public class Cooldown : IComponent { public float Value; }
}