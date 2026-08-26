# Reference patterns — the yardsticks

Distilled from the features that are actually well-built. Load this instead of dumping the
reference features; that costs ~95 KB of source, this costs ~8 KB.

**This file describes shapes, never current behaviour.** It says "Allow/Forbid looks like this",
it never says "system X currently does Y". If a finding depends on what a specific file does
*right now* — dead matcher, ordering, save round-trip — open that file. A doc that drifts from
code produces confidently wrong findings, which cost more than missed ones.

`code-style.md` and `ecs-conventions.md` auto-load on `**/*.cs`, so component shape, system
statelessness, `ContainsEntity` over `== null`, matcher-over-`hasX`, config null-checks and naming
are already in context. Not repeated here.

---

## Tier 1 — trust these as yardsticks

| Feature | Yardstick for |
|---|---|
| `Gameplay/Pullable/` | the complete shape: gate flags, phased Feature, entity state lifecycle, teardown, reactive query |
| `Gameplay/Collectables/` | the same at minimum size — 8 files, nothing extra |
| `Gameplay/Targeting/` | extensions vs systems, one matcher per intent |
| `Gameplay/Stats/` | a service reading groups instead of a system |
| `Gameplay/Skills/` | extensibility — adding a payload type without editing existing code |

## Tier 2 — good structure, known limits. Do not cite as duplication yardsticks

| Feature | Good for | Limit |
|---|---|---|
| `Gameplay/Effects/` | `IEffect` payload model, `Processed` cleanup | `EffectFactory.Create` is a switch; one `Process*` system per effect type — both grow linearly |
| `Gameplay/Abilities/` | trigger dispatch, config→entity build | `ApplyTrigger` is a switch; the reactive and charge activation systems overlap heavily |
| `Gameplay/Highlight/` | `EntityComponentProvider` view shape, feedback-on-event | small surface, not a full-feature example |

---

## Gate flags — Allow / Forbid

A `CanBeX` flag is maintained by one `AllowX` system plus one `ForbidXOnY` per reason.
Never a single `RefreshX` that computes the whole truth.

```csharp
// Feature order: Allow first, every Forbid after it
Add(systems.Create<AllowPullSystem>());
Add(systems.Create<ForbidPullOnCollectedSystem>());
```

`AllowX` matches the base capability and sets the flag on. Each `ForbidXOnY` matches
`AllOf(Base, CanBeX, Reason)` and sets it off. Adding a new reason = adding one system,
touching nothing.

The flag component carries `[FlagPrefix("")]` so it reads `entity.CanBePulled`, not `entity.isCanBePulled`.

## Feature files — grouped by phase

Blank lines separate phases. A flat block of 6+ `Add(...)` calls is a finding.

```csharp
Add(systems.Create<AllowPullSystem>());
Add(systems.Create<ForbidPullOnCollectedSystem>());

Add(systems.Create<SetPullableTargetSystem>());
Add(systems.Create<TickPullDelaySystem>());

Add(systems.Create<StartPullingMotionSystem>());
Add(systems.Create<MovePullableToTargetSystem>());
Add(systems.Create<StopPullingSystem>());
```

Canonical order: gate flags → targeting/selection → state transitions → per-frame work →
scoring/effects → cleanup. Infrastructure first, cleanup last.

## Entity state lifecycle — Start / Tick / Stop

A stateful process on an entity is three systems, not one. Transient components exist only while
the process runs and are removed on exit with `SafeRemoveX`.

```csharp
// Start: seed the transient components, raise the marker
pullable.ReplacePullStartPosition(pullable.WorldPosition);
pullable.ReplacePullElapsed(0f);
pullable.isPulling = true;

// Stop: lower the marker, remove everything the process owned
pullable.isPulling = false;
pullable.SafeRemoveTargetId();
pullable.SafeRemovePullStartPosition();
pullable.SafeRemovePullElapsed();
```

The Stop system's exit condition is a group membership test, not a null check:
`if (_pullers.ContainsEntity(puller)) continue;`

## Reactive queries — including the "ended" edge

Progress comes from the live group. The *end* of a process is `[Watched] XChanged` **plus**
`NoneOf(X)` — that matches exactly the frame the flag went down.

```csharp
_pulling = game.GetGroup(GameMatcher
    .AllOf(
        GameMatcher.Pulling,
        GameMatcher.Id,
        GameMatcher.PullProgress));

_pullEnded = game.GetGroup(GameMatcher
    .AllOf(
        GameMatcher.Id,
        GameMatcher.PullingChanged)
    .NoneOf(
        GameMatcher.Pulling));
```

Query implements `IReactiveQuery`, is registered with `BindInterfacesTo<>()`, and is driven by
`NotifyQueryChangesSystem`. Views subscribe to the C# events. Nothing polls the context per frame.
`[Watched]` only where a query actually consumes the change.

## Extensibility — registry over switch

When a new variant should not require editing existing code, use the builder-registry shape:

```csharp
public interface ISkillBuilder
{
    bool CanBuild(SkillPayloadConfig config);
    bool IsReady(GameEntity owner);
    void Build(GameEntity owner, SkillRequest request);
}
```

Builders are collected by `List<ISkillBuilder>` injection; the dispatching system finds the first
that `CanBuild` and throws a named exception when none does. Adding a payload type = one new class
plus one DI line.

A `switch` over payload types in a factory is acceptable only while the set is closed and small.
If the branch under review adds a third or fourth case to an existing switch, that is the moment to
call it — but say it as "this switch is now the extension point that should be a registry", not as
"the existing code is wrong".

## Layering

- Views read via Queries, write via Requests or service calls. They never touch entities directly.
- Domain services never touch `View.gameObject`, `SetActive`, or `Transform`. Visibility is domain
  state (`Hidden`, maintained by `EntityView`) consumed by a system.
- Game-flow orchestration — fade, suspend session, switch camera, change branch — belongs in a
  system or a state, not in a `WindowBase` method.
- Systems never write the save file outside `RefreshSnapshotsFeature`.
- No `Camera.main`, `FindObjectOfType`, or statics inside queries and services.

## Data value types

Values passed between layers are `readonly struct` with a constructor:

```csharp
public readonly struct SkillRequest
{
    public readonly SkillTypeId Source;
    public readonly SkillPayloadConfig Config;
    public readonly int Level;

    public SkillRequest(SkillTypeId source, SkillPayloadConfig config, int level) { ... }
}
```

Mutable structs with public fields filled by object-initializers are off-style. A mutable class used
as scratch state inside a service is worse — it leaks the service's working set into a `Data/` type.

A service must not return a reusable internal `List` as its public result.

## Config services

- Catalog keyed by an id → `GetConfig(TypeId id)`.
- Single global config → a property named `Config`.
- Never `XConfigsService.XConfig` — the stutter reads as a namespace mistake.
- Configs are loaded at bootstrap and never null-checked.

## Factories

Factories **create**. `IXFactory.Create...` returns a fully valid entity in one call, with
`AddId(_identifierService.Next())` first. A factory method that mutates an entity someone else
created is misnamed — that belongs in `XExtensions` or in the system that owns the transition.

## Feedback on an event

Flash, punch, shake and floating text already have homes: the `Highlight` feature
(`HighlightAnimator` + `HighlightConfig` + a `StartHighlightByRequestSystem`), the `Vfx` feature,
and `ICameraFactory.CreateShakeRequest`. A new hand-rolled `Update()` that decays a `_flash` float
is duplication — check these first.

## Naming

- One concept, one word, across components, systems and data types. Two vocabularies for one entity
  (packet / stone / batch) is a finding on its own.
- Grep new names against the whole codebase for near-duplicates that mean different things.
- Enum members get a trailing comment when the name alone is ambiguous.
