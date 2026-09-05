using Entitas;
using Entitas.CodeGeneration.Attributes;

namespace Code.Gameplay.Progress
{
	[Game] public class ExamProgress : IComponent { }
	[Game, Watched] public class PlayerName : IComponent { public string Value; }
	[Game, Watched] public class IntroSeen : IComponent { }
	[Game, Watched] public class BestAnswers : IComponent { public int Value; }
	[Game, Watched] public class BestTimeSeconds : IComponent { public float Value; }

	[Game] public class BestResultRecorded : IComponent { }

	[Game] public class SetPlayerNameRequest : IComponent { public string Value; }
	[Game] public class MarkIntroSeenRequest : IComponent { }
}
