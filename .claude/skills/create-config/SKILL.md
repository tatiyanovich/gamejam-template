---
name: create-config
description: Create a ScriptableObject config class. Configs hold serialized data with property getters — no methods, no logic, no lookups.
---

Create a config for the feature described below.

Config description: $ARGUMENTS

## Steps

### 1. Find the feature folder

Search for `Assets/Code/Gameplay/` and locate the feature folder. Create a `Configs/` subfolder if it doesn't exist.

### 2. Generate the config class

Create `{ConfigName}.cs` in the feature's `Configs/` folder.

```csharp
using UnityEngine;

namespace Code.Gameplay.{Feature}.Configs
{
	[CreateAssetMenu(fileName = "{ConfigName}", menuName = "Configs/{Feature}/{ConfigName}")]
	public class {ConfigName} : ScriptableObject
	{
		[SerializeField] private {type} {fieldName} = {defaultValue};

		public {Type} {PropertyName} => {fieldName};
	}
}
```

### 3. Add to ConfigsService if needed

If the project uses a centralized `IConfigsService`, read it and add a property for this config. If the feature warrants its own config service, create `I{Feature}ConfigsService` following the service pattern.

### 4. Place the ScriptableObject asset

The actual `.asset` file should be created in Unity Editor at:
`Assets/AddressableResources/Configs/{Feature}/{ConfigName}.asset`

Remind the user to:
1. Create the ScriptableObject asset in Unity (Right-click > Create > Configs > ...)
2. Add it to the appropriate Addressables group
3. Register it in the config loading pipeline

## Rules

- Use **tabs** for indentation (size 4), never spaces.
- All fields are `[SerializeField] private` with public property getters.
- **No methods, no logic, no lookups** — derived values go into a config service.
- Read-only after loading. Runtime state belongs on entities, not configs.
- `[CreateAssetMenu]` attribute is required with `fileName` and `menuName`.
- Config classes go in `{Feature}/Configs/`, assets in `AddressableResources/Configs/`.
- Namespace: `Code.Gameplay.{Feature}.Configs`.
