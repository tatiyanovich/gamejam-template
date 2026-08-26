---
name: create-snapshot
description: Create a Snapshot DTO for save/load. Snapshots are data-only classes stored in GeneralSaveFile, used to persist and restore entity state.
---

Create a snapshot for the entity described below.

Entity/feature name: $ARGUMENTS

## Steps

### 1. Find the feature components

Read the feature's `{Feature}Components.cs` to understand what data needs to be persisted.

### 2. Find the Storage folder

Search for `Assets/Code/Storage/` to find existing snapshots and the `GeneralSaveFile`.

### 3. Create the Snapshot class

Create `{Entity}Snapshot.cs` in the `Storage/Snapshots/` folder.

```csharp
using System;

namespace Code.Storage.Snapshots
{
	[Serializable]
	public class {Entity}Snapshot
	{
		public int Id;
		public {Type} {Field};

		public {Entity}Snapshot(
			int id,
			{type} {field})
		{
			Id = id;
			{Field} = {field};
		}
	}
}
```

### 4. Add to GeneralSaveFile

Find `GeneralSaveFile.cs` and add the field:

**For collection-based entities:**
```csharp
public List<{Entity}Snapshot> {Entities} = new();
```

**For singleton entities:**
```csharp
public {Entity}Snapshot {Entity};
```

### 5. Update the factory

Read the entity's factory and ensure it creates entities from snapshots:

```csharp
public GameEntity Create{Entity}({Entity}Snapshot snapshot)
{
	return _entityFactory.Game()
		.AddId(snapshot.Id)
		.With(x => x.is{Tag} = true)
		.Add{Component}(snapshot.{Field});
}

public {Entity}Snapshot CreateDefaultSnapshot()
{
	return new {Entity}Snapshot(
		id: 0,
		field: defaultValue);
}
```

### 6. Remind about RefreshSnapshotsSystem

Tell the user they also need a `RefreshSnapshotsSystem` to write entity state back to the save file. They can use `/create-refresh-snapshot-system {Entity}` to generate it.

## Rules

- Use **tabs** for indentation (size 4), never spaces.
- Snapshots are **data-only DTOs** — no methods, no logic.
- Must be `[Serializable]`.
- Constructor with named parameters for all fields.
- Public fields (not properties) for JSON serialization.
- Systems MUST NOT write to save file during gameplay — all writes happen in `RefreshSnapshotsFeature`.
- Entities that can be saved are ALWAYS created from their Snapshot.
- Namespace: `Code.Storage.Snapshots`.
