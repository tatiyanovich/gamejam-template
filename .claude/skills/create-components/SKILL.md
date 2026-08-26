---
name: create-components
description: Create ECS components file for a gameplay feature. Generates the components file with proper context attributes, naming, and placement.
---

Create ECS components for the feature described below.

Feature/entity name: $ARGUMENTS

## Steps

### 1. Determine feature location

Search for `Assets/Code/Gameplay/` in the project and find or create the feature folder matching the name.

### 2. Check for existing components file

Look for `{Feature}Components.cs` in the feature folder. If it exists, read it and add new components to it. If not, create it.

### 3. Generate the components file

Create `{Feature}Components.cs` in the feature folder.

Follow these patterns:

**Flag component (no fields):**
```csharp
[Game] public class Dead : IComponent { }
```

**Single-value component:**
```csharp
[Game] public class CurrentHP : IComponent { public float Value; }
```

**Multi-field component (use sparingly — prefer atomic):**
```csharp
[Game] public class AvailableWorkersComponent : IComponent
{
	public int RedCount;
	public int BlueCount;
}
```

**Request component:**
```csharp
[Game] public class SpawnEnemyRequest : IComponent { public EnemyTypeId Value; }
```

**Event component:**
```csharp
[Game] public class DeathEvent : IComponent { public int DeadEntityId; }
```

### Full file template

```csharp
using Entitas;

namespace Code.Gameplay.{Feature}
{
	[Game] public class {TagComponent} : IComponent { }
	// ... additional components
}
```

## Rules

- Use **tabs** for indentation (size 4), never spaces.
- Attribute with `[Game]` or `[Input]` context.
- Use `[Watched]` only when change tracking is needed for reactive queries.
- Prefer **atomic** components (single field or flag). 99% of regular components should be atomic.
- Use **tag components** for querying: `isPlayer`, `isEnemy`, `isDead`.
- If name collides with an enum (e.g. `EffectTypeId`), suffix the class with `Component`.
- Single-value components use `Value` as field name.
- One components file per feature — all components for the feature live in one file.
- Namespace mirrors folder path: `namespace Code.Gameplay.{Feature}`.
- After creating components, remind the user to run Jenny code generation.
