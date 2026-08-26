---
name: create-query
description: Create a Query — interface + implementation + DI registration. Queries provide read-only access to ECS state for Views and services. Optionally reactive.
---

Create a query for the feature described below.

Feature/query description: $ARGUMENTS

## Steps

### 1. Find the feature folder

Search for `Assets/Code/Gameplay/` and locate the feature folder. Queries go in the `Queries/` subfolder.

### 2. Read existing components

Read the feature's components to understand what data the query exposes.

### 3. Create the interface

Create `I{Feature}Query.cs` in the feature's `Queries/` folder.

**Basic query:**
```csharp
namespace Code.Gameplay.{Feature}.Queries
{
	public interface I{Feature}Query
	{
		{ReturnType} Get{Data}();
		bool Is{Condition}();
	}
}
```

**Reactive query (add events for push notifications):**
```csharp
using System;

namespace Code.Gameplay.{Feature}.Queries
{
	public interface I{Feature}Query
	{
		{ReturnType} Get{Data}();
		event Action<{EventArgType}> On{What}Changed;
	}
}
```

### 4. Create the implementation

Create `{Feature}Query.cs` in the feature's `Queries/` folder.

**Basic query:**
```csharp
using Entitas;

namespace Code.Gameplay.{Feature}.Queries
{
	public class {Feature}Query : I{Feature}Query
	{
		private readonly IGroup<GameEntity> _{groupName};

		public {Feature}Query(GameContext game)
		{
			_{groupName} = game.GetGroup(GameMatcher
				.AllOf(
					GameMatcher.{Component1},
					GameMatcher.{Component2}));
		}

		public {ReturnType} Get{Data}()
		{
			foreach (GameEntity entity in _{groupName})
			{
				return entity.{ComponentAccess};
			}

			return default;
		}
	}
}
```

**Reactive query (implements IReactiveQuery):**
```csharp
using System;
using Entitas;

namespace Code.Gameplay.{Feature}.Queries
{
	public class {Feature}Query : I{Feature}Query, IReactiveQuery
	{
		private readonly IGroup<GameEntity> _{groupName};
		private readonly IGroup<GameEntity> _{changedGroup};

		public event Action<{EventArgType}> On{What}Changed;

		public {Feature}Query(GameContext game)
		{
			_{groupName} = game.GetGroup(GameMatcher
				.AllOf(
					GameMatcher.{Component1},
					GameMatcher.{Component2}));

			_{changedGroup} = game.GetGroup(GameMatcher
				.AllOf(
					GameMatcher.{Component1},
					GameMatcher.{WatchedComponent}Changed));
		}

		public {ReturnType} Get{Data}()
		{
			foreach (GameEntity entity in _{groupName})
			{
				return entity.{ComponentAccess};
			}

			return default;
		}

		public void ReactToChanges()
		{
			_{changedGroup}
				.EvaluateFirst(entity => On{What}Changed?.Invoke(entity.{ComponentAccess}));
		}
	}
}
```

### 5. Register in GameplayInstaller

Find `GameplayInstaller.cs` and add to the `BindQueries()` method:

```csharp
Container.BindInterfacesTo<{Feature}Query>().AsSingle();
```

Using `BindInterfacesTo` ensures `IReactiveQuery` implementations are auto-collected by `NotifyQueryChangesSystem`.

## Rules

- Use **tabs** for indentation (size 4), never spaces.
- Use **explicit types** — never `var`.
- Private fields prefixed with `_`.
- Interface MUST NOT expose write methods or ECS internals (matchers, groups).
- Prefer small per-feature queries over one big query class.
- `IReactiveQuery` goes on the **class**, NOT the public interface.
- `ReactToChanges()` fires C# events only — no state mutations.
- Events on query interface use `On{X}` naming.
- Use reactive queries only for infrequent state changes. Views poll for per-frame data.
- Namespace: `Code.Gameplay.{Feature}.Queries`.
