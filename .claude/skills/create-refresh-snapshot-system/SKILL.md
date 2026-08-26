---
name: create-refresh-snapshot-system
description: Create a RefreshSnapshotsSystem for an ECS entity type. Generates the system, registers it in RefreshSnapshotsFeature, and adds the save file field if needed.
---

Create a RefreshSnapshotsSystem for the entity described below.

Entity/feature name: $ARGUMENTS

## Steps

### 1. Find the Snapshot class

Search for a `{EntityName}Snapshot` class under `Assets/Code/`. Read it to learn the snapshot's fields, constructor parameters, and namespace. If no snapshot class exists — stop and ask the user whether to create one first.

### 2. Find the ECS components

Search for the components file that defines this entity's components (e.g., `{Feature}Components.cs`). Identify the ECS context (`[Game]`, `[Input]`) and which components map to each snapshot field.

### 3. Find the GeneralSaveFile field

Find and read `GeneralSaveFile.cs`. Identify the field that stores this snapshot type. It will be either:
- A `List<{EntityName}Snapshot>` for collection-based entities
- A single `{EntityName}Snapshot` for singleton entities

If no field exists for this snapshot type, add it to `GeneralSaveFile` (with the appropriate `using` directive). Use `List<{EntityName}Snapshot>` by default unless the user specifies it's a singleton.

### 4. Generate the system

Create the file in the `Storage/Systems/` folder as `Refresh{EntityName}SnapshotsSystem.cs`.

Follow these patterns exactly:

**For collection-based entities** (the field is a `List<Snapshot>`):

```csharp
using System.Collections.Generic;
using {SnapshotNamespace};
using Code.Storage.SaveFiles;
using Entitas;
using Framework.Storage;

namespace Code.Storage.Systems
{
	public class Refresh{EntityName}SnapshotsSystem : IExecuteSystem
	{
		private readonly ISaveLoadService _saveLoadService;
		private readonly IGroup<{Context}Entity> _{entitiesVarName};
		private readonly List<{Context}Entity> _buffer = new(16);

		public Refresh{EntityName}SnapshotsSystem(
			{Context}Context {contextParam},
			ISaveLoadService saveLoadService)
		{
			_saveLoadService = saveLoadService;

			_{entitiesVarName} = {contextParam}.GetGroup({Context}Matcher
				.AllOf(
					{Context}Matcher.{TagComponent}));
		}

		public void Execute()
		{
			GeneralSaveFile saveFile = _saveLoadService.Get<GeneralSaveFile>();
			saveFile.{SaveFileField}.Clear();

			foreach ({Context}Entity {entityVar} in _{entitiesVarName}.GetEntities(_buffer))
			{
				{EntityName}Snapshot snapshot = new(
					{namedConstructorArgs});

				saveFile.{SaveFileField}.Add(snapshot);
			}
		}
	}
}
```

**For singleton entities** (the field is a single Snapshot):

```csharp
using {SnapshotNamespace};
using Code.Storage.SaveFiles;
using Entitas;
using Framework.Storage;

namespace Code.Storage.Systems
{
	public class Refresh{EntityName}SnapshotSystem : IExecuteSystem
	{
		private readonly ISaveLoadService _saveLoadService;
		private readonly IGroup<{Context}Entity> _{entitiesVarName};

		public Refresh{EntityName}SnapshotSystem(
			{Context}Context {contextParam},
			ISaveLoadService saveLoadService)
		{
			_saveLoadService = saveLoadService;

			_{entitiesVarName} = {contextParam}.GetGroup({Context}Matcher
				.AllOf(
					{Context}Matcher.{TagComponent}));
		}

		public void Execute()
		{
			GeneralSaveFile saveFile = _saveLoadService.Get<GeneralSaveFile>();

			foreach ({Context}Entity {entityVar} in _{entitiesVarName})
			{
				saveFile.{SaveFileField} = new {EntityName}Snapshot(
					{namedConstructorArgs});
			}
		}
	}
}
```

### 5. Register in RefreshSnapshotsFeature

Find `RefreshSnapshotsFeature.cs` and add:
```csharp
Add(systems.Create<Refresh{EntityName}SnapshotsSystem>());
```
Add the required `using Code.Storage.Systems;` if not already present.

## Rules

- Use **tabs** for indentation (size 4), never spaces.
- Use **explicit types** — never `var`.
- Private fields prefixed with `_`.
- Named arguments in constructor calls, each on its own line.
- Namespace must be `Code.Storage.Systems`.
- System class implements only `IExecuteSystem`.
- Map snapshot constructor parameter names to entity component accessors:
  - Value components: `entity.ComponentName` (e.g., `quest.Id`, `quest.CurrentProgress`)
  - Flag components: `entity.isFlag` (e.g., `quest.isActive`, `quest.isCompleted`)
- The group matcher should use the entity's primary tag component (e.g., `GameMatcher.ScoreHolder`, `GameMatcher.Pickup`).
- Follow all conventions from CLAUDE.md and CodingGuidelines.md.
