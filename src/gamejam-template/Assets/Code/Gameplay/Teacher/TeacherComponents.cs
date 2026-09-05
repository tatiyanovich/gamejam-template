using Entitas;
using Entitas.CodeGeneration.Attributes;

namespace Code.Gameplay.Teacher
{
	[Game] public class Teacher : IComponent { }
	[Game, Watched] public class TeacherAttentionComponent : IComponent { public TeacherAttention Value; }
	[Game] public class TeacherAttentionTimeLeft : IComponent { public float Value; }
	[Game, Watched] public class AlmostCaughtCount : IComponent { public int Value; }
}
