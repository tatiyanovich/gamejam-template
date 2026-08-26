---
name: create-event
description: Create an Event — component + producer/consumer pattern. Events are one-to-many notifications auto-destroyed after one frame.
---

Create an Event for the scenario described below.

Event description: $ARGUMENTS

## Steps

### 1. Determine the feature

Identify which feature produces this event. Search for `Assets/Code/Gameplay/` and locate the feature folder.

### 2. Add the event component

Read the feature's `{Feature}Components.cs` and add the event component:

```csharp
[Game] public class {Name}Event : IComponent { public int {RelevantId}; }
```

Remind the user to run Jenny code generation after adding the component.

### 3. Show the producer pattern

The system that creates the event:

```csharp
_entityFactory.Event().Add{Name}Event(entityId);
```

Events get a `Ready` flag next frame (via `EventsReadySystem`) so ALL systems see them regardless of ordering.

### 4. Create consumer system(s)

Create the consumer in the appropriate feature's `Systems/` folder.

```csharp
using System.Collections.Generic;
using Entitas;

namespace Code.Gameplay.{ConsumerFeature}.Systems
{
	public class {Action}On{Event}System : IExecuteSystem
	{
		private readonly GameContext _game;
		private readonly IGroup<GameEntity> _{eventGroup};
		private readonly IGroup<GameEntity> _{targetGroup};

		public {Action}On{Event}System(GameContext game)
		{
			_game = game;

			_{eventGroup} = game.GetEvents(GameMatcher
				.AllOf(
					GameMatcher.{Name}Event));

			_{targetGroup} = game.GetGroup(GameMatcher
				.AllOf(
					GameMatcher.{TargetMatcher}));
		}

		public void Execute()
		{
			foreach (GameEntity eventEntity in _{eventGroup})
			{
				GameEntity target = _game.GetEntityWithId(eventEntity.{eventAccessor}.{RelevantId});

				if (_{targetGroup}.ContainsEntity(target))
				{
					// react to event
				}
			}
		}
	}
}
```

### 5. Register in Feature

Add consumer systems to the appropriate Feature class. Consumer systems must be ordered AFTER the producer.

## Rules

- Use **tabs** for indentation (size 4), never spaces.
- Create events with `_entityFactory.Event()` — tagged `isEvent`.
- Events get `Ready` flag next frame so ALL systems see them.
- Consume with `game.GetEvents(matcher)` which includes `Ready` + `Event` automatically.
- Do NOT manually destroy event entities — `EventsCleanupSystem` handles this.
- Event components are suffixed with `Event`.
- Consumer system naming: `{Action}On{Event}System`.
- Avoid events for high-frequency per-frame signals — use a component on an existing entity instead.
- Entity existence checks: use `group.ContainsEntity(entity)` instead of `entity != null`.
