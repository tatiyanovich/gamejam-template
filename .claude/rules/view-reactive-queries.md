---
paths:
  - "**/Behaviours/**/*.cs"
  - "**/Queries/**/*.cs"
  - "**/Views/**/*.cs"
description: How Views/animators react to ECS state — via reactive Query events, not per-frame systems poking the view
---

## Views react to state changes via Query events — never poll them from a system

A View (animator, VFX behaviour, UI widget, window) must be **driven by an event it subscribes to**, not by a system that reaches into the view every frame and calls a setter. When ECS state changes, a reactive **Query** raises a C# event; the View subscribes to that event and updates itself.

**Do not** write a per-frame `IExecuteSystem` that iterates entities and calls a method on their view component:

```csharp
// ANTI-PATTERN — PushDistanceToHudSystem
public void Execute()
{
    foreach (GameEntity run in _runs.GetEntities(_buffer))
        _hud.SetDistance(run.DrilledDistance);   // system pokes the view every frame
}
```

This runs every frame regardless of change, couples a domain system to a specific view type, and puts view logic in the system pipeline.

### The pattern (reference: `DrillingQuery` + `GameplayWindow`)

**1. Mark the component `[Watched]` and implement `IReactiveQuery`.** Codegen emits an `XChanged` flag, raised whenever the component is added or removed (tag components) or its value is reassigned; `GameWatchedCleanupSystems` clears it in the frame's infra tail. Match on `GameMatcher.XChanged` and fire the event from `ReactToChanges()`, which `NotifyQueryChangesSystem` drives.

This applies to tag components too — `[Watched]` on a tag covers add and remove both. Never hand-subscribe to `group.OnEntityAdded`/`OnEntityRemoved`: that fires mid-pipeline the instant any system mutates the entity, rather than at the one defined point in the frame, and it forces the query to be `IDisposable`.

```csharp
public class DrillingQuery : IDrillingQuery, IReactiveQuery
{
    private readonly IGroup<GameEntity> _runs;
    private readonly IGroup<GameEntity> _changedRuns;

    public event Action<float> OnDistanceChanged;

    public DrillingQuery(GameContext game)
    {
        _runs = game.GetGroup(GameMatcher
            .AllOf(
                GameMatcher.DrillRun,
                GameMatcher.DrilledDistance));

        _changedRuns = game.GetGroup(GameMatcher
            .AllOf(
                GameMatcher.DrillRun,
                GameMatcher.DrilledDistance,
                GameMatcher.DrilledDistanceChanged));
    }

    public void ReactToChanges()
    {
        foreach (GameEntity run in _changedRuns)
            OnDistanceChanged?.Invoke(run.DrilledDistance);
    }

    public float GetDistance()
    {
        foreach (GameEntity run in _runs)
            return run.DrilledDistance;

        return 0f;
    }
}
```

Expose a getter alongside the event: a view that opens mid-session needs the current value once, then lives off events.

**2. Register the query** with `BindInterfacesTo<>().AsSingle()` (feature installer or `BootstrapInstaller`) so the `IReactiveQuery` is auto-collected and the interface is injectable into views.

**3. A window subscribes in `OnOpen`, unsubscribes in `OnClose`, and seeds itself from the getter:**

```csharp
protected override UniTask OnOpen(CancellationToken cancellationToken = default)
{
    _drillingQuery.OnDistanceChanged += HandleDistanceChanged;

    HandleDistanceChanged(_drillingQuery.GetDistance());

    return base.OnOpen(cancellationToken);
}

protected override UniTask OnClose(CancellationToken cancellationToken = default)
{
    _drillingQuery.OnDistanceChanged -= HandleDistanceChanged;

    return base.OnClose(cancellationToken);
}

private void HandleDistanceChanged(float distance)
{
    distanceText.text = $"{Mathf.FloorToInt(distance)} M";
}
```

**4. An entity view** (a behaviour on the entity's prefab) subscribes in `RegisterComponents()`, unsubscribes in `UnregisterComponents()`, and filters to its own entity, since the query event fires for every changed entity:

```csharp
public override void RegisterComponents()
{
    Entity.AddDrillGlowAnimator(this);
    _fuelQuery.OnFuelEmptied += HandleFuelEmptied;
}

public override void UnregisterComponents()
{
    Entity.SafeRemoveDrillGlowAnimator();
    _fuelQuery.OnFuelEmptied -= HandleFuelEmptied;
}

private void HandleFuelEmptied(GameEntity drill)
{
    if (drill != Entity)
        return;

    PlayGlow();
}
```

### Rule of thumb

- **State → view sync belongs in a Query event, not a system.** If you find yourself writing a system whose only job is to push a component value onto a view component each frame, replace it with a reactive Query the view subscribes to.
- **Per-frame `Update()` inside the view is fine** for smoothing/interpolation toward a target the event set. The *trigger* is the event; the *tween* is local.
- Subscribe in `OnOpen`/`RegisterComponents`, unsubscribe in `OnClose`/`UnregisterComponents` — symmetric, no leaks.
