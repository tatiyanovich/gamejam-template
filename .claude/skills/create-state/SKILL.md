---
name: create-state
description: Create a game state for the state machine. States implement IState plus optional IEnter, IExit, IExecutable, IFixedExecutable, ILateExecutable.
---

Create a game state as described below.

State description: $ARGUMENTS

## Steps

### 1. Read existing states

Search for the `StateManagement/States/` folder under `Assets/Code/Infrastructure/` to understand the current state flow and patterns.

### 2. Determine interfaces

Based on the state's responsibility, choose which interfaces to implement:

| Interface | Purpose |
|-----------|---------|
| `IState` | Required for all states |
| `IEnter` | Has `Enter()` — runs once when entering the state |
| `IExit` | Has `Exit()` — runs once when leaving the state |
| `IExecutable` | Has `Execute()` — called every frame (Update) |
| `IFixedExecutable` | Has `FixedExecute()` — called every fixed update |
| `ILateExecutable` | Has `LateExecute()` — called every late update |

### 3. Generate the state file

Create `{StateName}.cs` in the `StateManagement/States/` folder.

**Simple async state (like PrepareGameplayState):**
```csharp
using Cysharp.Threading.Tasks;
using Framework.StateManagement;

namespace Code.Infrastructure.StateManagement.States
{
	public class {StateName} : IState, IEnter
	{
		private readonly IGameStateMachine _gameStateMachine;

		public {StateName}(IGameStateMachine gameStateMachine)
		{
			_gameStateMachine = gameStateMachine;
		}

		public void Enter()
		{
			Prepare().Forget();
		}

		private async UniTask Prepare()
		{
			// async setup work
			_gameStateMachine.Enter<{NextState}>();
		}
	}
}
```

**Gameplay state (with features and update loops):**
```csharp
using Code.Infrastructure.EntityComponentSystem.Factories;
using Framework.StateManagement;

namespace Code.Infrastructure.StateManagement.States
{
	public class {StateName} : IState, IEnter, IExecutable, IExit
	{
		private readonly ISystemFactory _systemFactory;
		private {Feature}Feature _{featureName};

		public {StateName}(ISystemFactory systemFactory)
		{
			_systemFactory = systemFactory;
		}

		public void Enter()
		{
			_{featureName} = _systemFactory.Create<{Feature}Feature>();
			_{featureName}.Initialize();
		}

		public void Execute()
		{
			_{featureName}.Execute();
			_{featureName}.Cleanup();
		}

		public void Exit()
		{
			_{featureName}.TearDown();
			_{featureName} = null;
		}
	}
}
```

### 4. Register the state

Find where states are registered with the state machine and add the new state registration.

### 5. Wire the transition

Update the previous state to transition to this new state, or update this state to transition to the next state.

## Rules

- Use **tabs** for indentation (size 4), never spaces.
- Use **explicit types** — never `var`.
- Private fields prefixed with `_`.
- States implement `IState` plus optional lifecycle interfaces.
- State flow: `BootstrapState` -> `LoadProgressState` -> `MigrateProgressState` -> `PrepareGameplayState` -> `GameplayState`.
- For async entry, use `Enter()` calling `Prepare().Forget()` with `UniTask`.
- Gameplay states own Features and call `Initialize()`, `Execute()`, `Cleanup()`, `TearDown()`.
- Namespace: `Code.Infrastructure.StateManagement.States`.
