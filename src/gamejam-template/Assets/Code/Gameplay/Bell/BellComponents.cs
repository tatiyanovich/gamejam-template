using Entitas;
using Entitas.CodeGeneration.Attributes;

namespace Code.Gameplay.Bell
{
	[Game, Watched] public class BellAnnounced : IComponent { }

	[Game] public class BellAnnouncementEvent : IComponent { }
}
