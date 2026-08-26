---
name: create-scaffolding
description: Create full feature scaffolding — folders, TypeId enum, Config, ConfigsService, Factory, Feature, Installer. Mirrors the Tokens template.
---

Create a complete feature scaffolding for the feature described below.

Feature name: $ARGUMENTS

## Steps

### 1. Determine names

Given the feature name (e.g. `Asteroid`), derive:
- `{Feature}` — PascalCase singular (e.g. `Asteroid`)
- `{feature}` — camelCase singular (e.g. `asteroid`)
- `{feature_configs_label}` — snake_case with `_configs` suffix (e.g. `asteroid_configs`)
- Feature folder: `Assets/Code/Gameplay/{Feature}s/`

### 2. Create folder structure

Create the following folders under `Assets/Code/Gameplay/{Feature}s/`:
```
{Feature}s/
├── Configs/
├── Services/
├── Systems/
```

### 3. Generate `{Feature}Components.cs`

Create in the feature root folder. Contains a tag component marking entities of this feature type.

```csharp
using Entitas;

namespace Code.Gameplay.{Feature}s
{
	[Game] public class {Feature} : IComponent { }
}
```

### 4. Generate `{Feature}TypeId.cs`

Create in the feature root folder.

```csharp
namespace Code.Gameplay.{Feature}s
{
	public enum {Feature}TypeId
	{
		Unknown = 0,
	}
}
```

### 5. Generate `Configs/{Feature}Config.cs`

```csharp
using UnityEngine;

namespace Code.Gameplay.{Feature}s.Configs
{
	[CreateAssetMenu(fileName = nameof({Feature}Config), menuName = "Configs/" + nameof({Feature}Config), order = -1000)]
	public class {Feature}Config : ScriptableObject
	{
		public {Feature}TypeId {Feature}TypeId;
	}
}
```

### 6. Generate `Services/I{Feature}ConfigsService.cs`

```csharp
using Code.Gameplay.{Feature}s.Configs;
using Code.Infrastructure.ConfigsManagement;

namespace Code.Gameplay.{Feature}s.Services
{
	public interface I{Feature}ConfigsService : IConfigsService
	{
		{Feature}Config GetConfig({Feature}TypeId type);
	}
}
```

### 7. Generate `Services/{Feature}ConfigsService.cs`

```csharp
using System.Collections.Generic;
using System.Linq;
using Code.Gameplay.{Feature}s.Configs;
using Framework.AssetManagement;

namespace Code.Gameplay.{Feature}s.Services
{
	public class {Feature}ConfigsService : I{Feature}ConfigsService
	{
		private readonly IAssetsService _assets;

		private List<{Feature}Config> _{feature}Configs = new();
		private Dictionary<{Feature}TypeId, {Feature}Config> _{feature}ByTypeId;

		private const string {Feature}ConfigsLabel = "{feature_configs_label}";

		public {Feature}ConfigsService(IAssetsService assets)
		{
			_assets = assets;
		}

		public void LoadConfigs()
		{
			Load{Feature}Configs();
		}

		public {Feature}Config GetConfig({Feature}TypeId type)
		{
			{Feature}Config config = null;

			_{feature}ByTypeId?.TryGetValue(type, out config);

			return config;
		}

		private void Load{Feature}Configs()
		{
			_{feature}Configs = _assets
				.GetAssetsByLabel<{Feature}Config>({Feature}ConfigsLabel)
				.ToList();

			_{feature}ByTypeId = _{feature}Configs
				.ToDictionary(x => x.{Feature}TypeId, x => x);
		}
	}
}
```

### 8. Generate `Services/I{Feature}Factory.cs`

```csharp
namespace Code.Gameplay.{Feature}s.Services
{
	public interface I{Feature}Factory
	{
	}
}
```

### 9. Generate `Services/{Feature}Factory.cs`

```csharp
using Code.Infrastructure.EntityComponentSystem.Factories;
using Code.Infrastructure.EntityComponentSystem.Identifiers;

namespace Code.Gameplay.{Feature}s.Services
{
	public class {Feature}Factory : I{Feature}Factory
	{
		private IEntityFactory _entityFactory;
		private IIdentifierService _identifierService;

		public {Feature}Factory(
			IEntityFactory entityFactory,
			IIdentifierService identifierService)
		{
			_identifierService = identifierService;
			_entityFactory = entityFactory;
		}
	}
}
```

### 10. Generate `{Feature}Feature.cs`

```csharp
using Code.Infrastructure.EntityComponentSystem.Factories;

namespace Code.Gameplay.{Feature}s
{
	public sealed class {Feature}Feature : Feature
	{
		public {Feature}Feature(ISystemFactory systems)
		{
			//Add(systems.Create<T>());
		}
	}
}
```

### 11. Generate `{Feature}Installer.cs`

```csharp
using Code.Gameplay.{Feature}s.Services;
using Framework.Essentials.DependencyInjection;
using Zenject;

namespace Code.Gameplay.{Feature}s
{
	public class {Feature}Installer : PlainAbstractInstaller
	{
		public {Feature}Installer(DiContainer container) : base(container)
		{
		}

		public override void InstallBindings()
		{
			Container.BindInterfacesTo<{Feature}Factory>().AsSingle();
		}
	}
}
```

### 12. Register in GameplayCoreFeature

Find `GameplayCoreFeature.cs` and add:
```csharp
Add(systemFactory.Create<{Feature}Feature>());
```

Place it in logical order among existing features.

### 13. Register Installer

Find `GameplayInstaller.cs` and add the installer call in the `BindGameplayServices()` method:
```csharp
new {Feature}Installer(Container).InstallBindings();
```

### 14. Register ConfigsService in BootstrapInstaller

Find `BootstrapInstaller.cs` and add to the `BindConfigServices()` method:
```csharp
Container.BindInterfacesTo<{Feature}ConfigsService>().AsSingle();
```

Add the corresponding using directive:
```csharp
using Code.Gameplay.{Feature}s.Services;
```

## Rules

- Use **tabs** for indentation (size 4), never spaces.
- Use **explicit types** — never `var`.
- Private fields prefixed with `_`.
- Namespaces mirror folder paths: `namespace Code.Gameplay.{Feature}s`.
- One class/interface per file.
- Feature folder name is **plural** (e.g. `Tokens`, `Asteroids`).
- Class names are **singular** (e.g. `TokenFactory`, `AsteroidConfig`).
- The `Systems/` folder is created empty — systems are added later via `/create-system`.