# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

A game-jam starter: Unity 6 (6000.3.6f1) ECS project on Entitas with a three-layer architecture
(View / Domain / Storage). Zenject for DI, UniTask for async, Addressables for assets, DOTween for
tweens. Everything here is infrastructure plus one deliberately trivial gameplay slice — drive a drill
over an endless field, burn fuel, and the distance record survives a restart. Build the actual game
on top; don't preserve the sample.

The Unity project lives in `src/gamejam-template/`. The `Jenny/` code-gen tooling lives at the repo root.

## Code Generation

Entitas code is generated via Jenny. Config: `Jenny/JennyRoslyn.properties`. Output: `Assets/Code/Generated/`.

**To regenerate:** click the **Generate** button in the Unity main toolbar, or run `Jenny-Gen.bat`
(Windows) / `Jenny-Gen` (macOS/Linux) **from inside the `Jenny/` folder** — the script resolves
`.\Jenny\Jenny.Generator.Cli.dll` and `JennyRoslyn.properties` relative to its own directory, so it
fails from the repo root. Regenerate after adding/removing/renaming any `[Game]`/`[Input]` component
so the generated context API stays in sync. Do NOT manually edit files under `Assets/Code/Generated/`.

Jenny reads the file list from `Assembly-CSharp.csproj`. After pulling changes or adding files, let
Unity refresh that csproj **before** running Jenny-Gen — a stale csproj makes it silently drop components.

Two ECS contexts exist: **Game** and **Input**. Add a context by appending it to
`Entitas.CodeGeneration.Plugins.Contexts` in the properties file and regenerating.

The `[Watched]` attribute generates change-tracking flags (`GameMatcher.XChanged`) for reactive queries.

## Testing

> There is no `Assets/Code/Tests/` folder yet — this is the intended pattern once tests are added.

- **Framework:** NUnit + FluentAssertions + NSubstitute
- **Location:** `Assets/Code/Tests/EditMode/`
- **Run tests:** Unity Test Runner (Window > Testing > Test Runner) — edit-mode tests only
- **Pattern:** Arrange-Act-Assert. Always call `_game.DestroyAllEntities()` in `[TearDown]`.

## Architecture Quick Reference

### Layers
- **View** — MonoBehaviours, UI, VFX. Reads domain via Queries; writes via Requests or service calls. Never modifies entities directly.
- **Domain** — ECS systems, factories, queries, services, state machine. All game rules live here.
- **Storage** — Snapshots (DTOs) and ISaveLoadService. No gameplay logic.

### Key Patterns

**Entities are created through factories** using `IEntityFactory`:
- `_entityFactory.Game()` — regular entity
- `_entityFactory.Event()` — tagged `isEvent`, auto-destroyed after one frame by `EventsCleanupSystem`
- `_entityFactory.Request()` — tagged `isRequest`, must be destroyed by its handler system
- `_entityFactory.Input()` — input-context entity

**Requests** (many-to-one): created from Views or systems, handled by a single system. Use the
`RequestHandlerSystem` base class for automatic cleanup; `OrphanRequestDetectionSystem` catches leaks.

**Events** (one-to-many): created by one system, consumed by many. Get a `Ready` flag next frame so
every consumer sees them. Consume with `game.GetEvents(matcher)` — exactly one component per matcher.

**Queries** (read API): read-only access to ECS state, mostly for Views. Implement `IReactiveQuery`
for push notifications via C# events; `NotifyQueryChangesSystem` drives the reactive cycle.
`DrillingQuery` is the reference example.

**Core loop**: the game is a graph of nodes (`LoopNodeId`). `StartLaunch` (menu) runs as a single
pipeline; every other node runs as a **session branch** so several can tick at once. Views never
enter states — they create requests (`ICoreLoopRequestFactory`) which `CoreLoopFeature` handles.

**State machine**: `EntryPoint` enters `BootstrapState` → `LoadProgressState` → `MigrateProgressState`
→ `PrepareAssetsState` → `ResolveLoopEntryState` → `LoadLoopSceneState` → `PrepareLoopSceneState` →
`RunLoopSceneState` (→ `SessionsRunningState` for session nodes). Side states: `ErrorHappenedState`
(entered by `ExceptionCatchService` on uncaught exceptions) and `RebootAppState` →
`CleanupAllContextsState` → back to `BootstrapState`. States implement `IState` plus optional
`IEnter`, `IExit`/`IEndOfFrameExit`, `IExecutable`, `IFixedExecutable`, `ILateExecutable`.

**System ordering matters**: systems run in the order they are added to a Feature.
`GlobalLoopInfraHeadFeature` runs before gameplay, `GlobalLoopInfraTailFeature` after it — put
gameplay features in `GameplayCoreFeature`.

### DI Registration

- `BootstrapInstaller` (Boot scene) — everything that must survive a scene swap: state machine, UI, storage, configs, feature installers
- `EcsInstaller` — ECS contexts, entity/system factories, identifier service
- `GameplayInstaller` (Boot scene) — per-scene factories and debug actions
- MonoBehaviours use `[Inject] private void Construct(...)`. Plain C# classes use constructor injection.
- Queries registered with `BindInterfacesTo<>()` so `IReactiveQuery` implementations are auto-collected.

### UI

Windows live in `Assets/Code/UI/` and extend `WindowBase` (`Assets/Plugins/Framework/UI`). To add one:
write the class, create a `WindowConfig` asset labelled `window_configs` pointing at the prefab, add
its GUID to `Addresses.UI`, and register a `WindowDefinition` in `BootstrapState.AddWindowDefinitions`.

### Save/Load

Saveable state always enters the world from its snapshot. Systems MUST NOT write to the save file
during gameplay — all snapshot mutation happens in `RefreshSnapshotsFeature`, triggered by
`ISaveLoadService.OnSaveRequested`. `AutoSaveSystem` requests a save every 5s;
`SaveProgressByRequestSystem` performs it. `RefreshDrillRunSystem` is the reference example.

## Auto-loaded Rules

Path-scoped rules in `.claude/rules/` load automatically when you touch matching files — read them before editing:
- `code-style.md` — C# conventions (`**/*.cs`): tabs, explicit types, `_` field prefix, `== false` over `!`, member ordering, stateless systems.
- `ecs-conventions.md` — ECS modelling (`**/*.cs`): one value per component, no state in systems, differences in matchers not base classes, naming, no speculative config fields.
- `FolderStructure.md` — feature-based layout under `Assets/Code/`; never invent folder names.
- `Addressables Guidelines.md` — asset grouping/naming for `AddressableResources/` (groups PascalCase, members snake_case with type suffixes like `_prefab`, `_config`).
- `Unity MCP Editor Work.md` — script Editor changes (prefabs, ScriptableObject fields, Addressables entries, Play Mode) via Unity MCP `RunCommand` instead of handing the designer a manual checklist.

## Scaffolding Skills

Prefer the `create-*` skills (e.g. `create-feature`, `create-system`, `create-components`,
`create-factory`, `create-query`, `create-request`, `create-event`, `create-config`, `create-service`,
`create-snapshot`, `create-state`, `create-scaffolding`) when adding new ECS building blocks — they
place files, follow naming, and wire up DI/Feature registration for you.
