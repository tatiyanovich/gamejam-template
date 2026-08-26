---
name: create-request
description: Create a Request — component + RequestHandlerSystem + registration. Requests are many-to-one fire-and-forget commands.
---

Create a Request for the action described below.

Request description: $ARGUMENTS

## Steps

### 1. Determine the feature

Identify which feature this request belongs to. Search for `Assets/Code/Gameplay/` and locate the feature folder.

### 2. Add the request component

Read the feature's `{Feature}Components.cs` file and add the request component:

**Flag request (no data):**
```csharp
[Game] public class RestartGameRequest : IComponent { }
```

**Data request:**
```csharp
[Game] public class SpawnEnemyRequest : IComponent { public EnemyTypeId Value; }
```

Remind the user to run Jenny code generation after adding the component.

### 3. Create the handler system

Create the handler in the feature's `Systems/` folder.

```csharp
using Entitas;
using Code.Infrastructure.EntityComponentSystem.Systems;

namespace Code.Gameplay.{Feature}.Systems
{
	public class {HandlerName} : RequestHandlerSystem<GameEntity>
	{
		public {HandlerName}(GameContext game)
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
				// handle the request
			}
		}
	}
}
```

### 4. Register in Feature

Add the handler system to the feature's Feature class:
```csharp
Add(systemFactory.Create<{HandlerName}>());
```

### 5. Show how to create the request

Provide the caller pattern:

**From a system:**
```csharp
_entityFactory.Request()
	.With(x => x.is{RequestComponent} = true);
```

**From a view (MonoBehaviour):**
```csharp
_entityFactory.Request()
	.Add{RequestComponent}(value);
```

**From a factory method (preferred for cross-feature):**
```csharp
public void Create{Action}Request({params})
{
	_entityFactory.Request()
		.Add{RequestComponent}(value);
}
```

## Rules

- Use **tabs** for indentation (size 4), never spaces.
- Create requests with `_entityFactory.Request()` (NOT `.Game()`). Tags entity with `isRequest`.
- Handler MUST destroy request entities — `RequestHandlerSystem` does this automatically.
- Do NOT use `RequestHandlerSystem` when system also implements `IInitializeSystem`.
- Request components are suffixed with `Request`.
- One handler system per request type.
- If multiple places need to create the same request, put a helper method on the relevant factory.
