---
name: create-factory
description: Create an entity factory — interface + implementation + DI registration. Factories encapsulate entity creation with fully valid entities in one call.
---

Create a factory for the entity described below.

Entity/feature name: $ARGUMENTS

## Steps

### 1. Find the feature folder

Search for `Assets/Code/Gameplay/` and locate the feature folder. Factories go in the `Services/` subfolder.

### 2. Read existing components

Read the feature's `{Feature}Components.cs` to understand what components the entity has.

### 3. Create the interface

Create `I{Feature}Factory.cs` in the feature's `Services/` folder.

```csharp
namespace Code.Gameplay.{Feature}.Services
{
	public interface I{Feature}Factory
	{
		GameEntity Create{Entity}({params});
	}
}
```

### 4. Create the implementation

Create `{Feature}Factory.cs` in the feature's `Services/` folder.

```csharp
using Code.Infrastructure.EntityComponentSystem.Extensions;
using Code.Infrastructure.EntityComponentSystem.Factories;
using Code.Infrastructure.EntityComponentSystem.Identifiers;

namespace Code.Gameplay.{Feature}.Services
{
	public class {Feature}Factory : I{Feature}Factory
	{
		private readonly IEntityFactory _entityFactory;
		private readonly IIdentifierService _identifierService;

		public {Feature}Factory(
			IEntityFactory entityFactory,
			IIdentifierService identifierService)
		{
			_entityFactory = entityFactory;
			_identifierService = identifierService;
		}

		public GameEntity Create{Entity}({params})
		{
			return _entityFactory.Game()
				.AddId(_identifierService.Next())
				.With(x => x.is{TagComponent} = true)
				.Add{Component1}(value1)
				.Add{Component2}(value2);
		}
	}
}
```

**For saveable entities (created from snapshot):**
```csharp
public GameEntity Create{Entity}({Entity}Snapshot snapshot)
{
	return _entityFactory.Game()
		.AddId(snapshot.Id)
		.With(x => x.is{TagComponent} = true)
		.Add{Component}(snapshot.{Field});
}

public {Entity}Snapshot CreateDefaultSnapshot()
{
	return new {Entity}Snapshot(
		id: 0,
		field: defaultValue);
}
```

### 5. Register in GameplayInstaller

Find `GameplayInstaller.cs` and add to the `BindFactories()` method:

```csharp
Container.BindInterfacesTo<{Feature}Factory>().AsSingle();
```

## Rules

- Use **tabs** for indentation (size 4), never spaces.
- Use **explicit types** — never `var`.
- Private fields prefixed with `_`.
- Factories are the ONLY place that creates entities with specific components.
- Factory methods create **fully valid** entities in one call.
- Saveable entities MUST always be created from their Snapshot.
- Factories own request creation for their feature.
- May use configs and identifier services. Must NOT depend on Views.
- Use `.With(x => x.is{Flag} = true)` for tag components.
- Use `.With(x => x.Add{Component}(value), when: condition)` for conditional components.
- Namespace: `Code.Gameplay.{Feature}.Services`.
