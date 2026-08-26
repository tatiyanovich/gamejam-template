---
name: create-system
description: Create an ECS system. Supports IExecuteSystem, IInitializeSystem, ICleanupSystem, and RequestHandlerSystem. Generates the system file and registers it in the Feature.
---

Create an ECS system as described below.

System description: $ARGUMENTS

## Steps

### 1. Determine system type

Based on the description, choose the appropriate system type:

| Base | When to use |
|------|-------------|
| `IExecuteSystem` | Regular per-frame logic (most systems) |
| `IInitializeSystem` | One-time setup: spawning entities, restoring from save |
| `ICleanupSystem` | Resetting boolean flags after they've been consumed |
| `RequestHandlerSystem` | Handling one-shot request entities (auto-destroys them) |

### 2. Find the feature folder

Search for `Assets/Code/Gameplay/` and locate the feature this system belongs to. Systems go in the `Systems/` subfolder.

### 3. Generate the system file

Create `{SystemName}.cs` in the feature's `Systems/` folder.

**IExecuteSystem template:**
```csharp
using System.Collections.Generic;
using Entitas;

namespace Code.Gameplay.{Feature}.Systems
{
	public class {SystemName} : IExecuteSystem
	{
		private readonly IGroup<GameEntity> _{groupName};
		private readonly List<GameEntity> _buffer = new({bufferSize});

		public {SystemName}(GameContext game)
		{
			_{groupName} = game.GetGroup(GameMatcher
				.AllOf(
					GameMatcher.{Component1},
					GameMatcher.{Component2}));
		}

		public void Execute()
		{
			foreach (GameEntity {entityVar} in _{groupName}.GetEntities(_buffer))
			{
				// logic here
			}
		}
	}
}
```

**IInitializeSystem template:**
```csharp
using Entitas;

namespace Code.Gameplay.{Feature}.Systems
{
	public class {SystemName} : IInitializeSystem
	{
		public {SystemName}(GameContext game)
		{
		}

		public void Initialize()
		{
			// one-time setup
		}
	}
}
```

**ICleanupSystem template:**
```csharp
using System.Collections.Generic;
using Entitas;

namespace Code.Gameplay.{Feature}.Systems
{
	public class {SystemName} : ICleanupSystem
	{
		private readonly IGroup<GameEntity> _{groupName};
		private readonly List<GameEntity> _buffer = new({bufferSize});

		public {SystemName}(GameContext game)
		{
			_{groupName} = game.GetGroup(GameMatcher
				.AllOf(
					GameMatcher.{FlagComponent}));
		}

		public void Cleanup()
		{
			foreach (GameEntity entity in _{groupName}.GetEntities(_buffer))
			{
				entity.is{FlagComponent} = false;
			}
		}
	}
}
```

**RequestHandlerSystem template:**
```csharp
using Entitas;
using Code.Infrastructure.EntityComponentSystem.Systems;

namespace Code.Gameplay.{Feature}.Systems
{
	public class {SystemName} : RequestHandlerSystem<GameEntity>
	{
		public {SystemName}(GameContext game)
			: base(game.GetGroup(GameMatcher
				.AllOf(
					GameMatcher.Request,
					GameMatcher.{RequestComponent})))
		{
		}

		protected override void OnExecute(IGroup<GameEntity> requests)
		{
			foreach (GameEntity request in requests)
			{
				// handle request
			}
		}
	}
}
```

### 4. Register in the Feature

Find the appropriate Feature class and add:
```csharp
Add(systemFactory.Create<{SystemName}>());
```

Respect ordering: producers before consumers, state changes before view updates, cleanup last.

## Rules

- Use **tabs** for indentation (size 4), never spaces.
- Use **explicit types** — never `var`.
- Private fields prefixed with `_`.
- Systems must be **stateless** — no mutable instance fields. Only `readonly` groups, buffers, services, factories.
- **Single responsibility** — one short sentence describes what the system does.
- **1-2 groups** per system is ideal. **3 is the hard maximum.**
- Pre-allocate buffers: `private readonly List<GameEntity> _buffer = new(N);`
- Buffering (`GetEntities(_buffer)`) is only required when the loop body causes entities to enter or leave the iterated group.
- Entity existence checks: use `group.ContainsEntity(entity)` instead of `entity != null`.
- **Combine existence + flag/component checks into a group.** When the loop body would otherwise do `if (entity == null) continue;` followed by `if (entity.isFoo == false) continue;` (or `entity.hasFoo == false`), instead define a group whose matcher includes `Foo` and replace both guards with a single `if (_fooGroup.ContainsEntity(entity) == false) continue;`. `ContainsEntity` returns false for null, so the null check is folded in. Do this even if the group is "passive" (only used for lookups, never iterated). Example: looking up an entity by id and checking `isAlive` → build a group of `AllOf(Id, Alive)` and use `ContainsEntity`.
- Namespace mirrors folder: `namespace Code.Gameplay.{Feature}.Systems`.
- Follow naming convention: `[Subject][Action]System` (e.g., `MarkInCombatSystem`, `ProcessDamageEffectSystem`).
