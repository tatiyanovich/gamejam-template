---
paths:
  - "**/*.cs"
description: ECS modelling rules — component shape, system state, how systems differ, naming
---

## ECS Conventions (enforced)

### Entity components hold exactly one value

A `[Game]` component that lives on an **entity** is a flag or holds **one** value. Never declare such a component with several public fields, and never wrap a multi-field struct in one to get around it.

An entity is composed from those components — the shape `EffectFactory` uses:

```csharp
_entityFactory.Game()
	.AddId(_identifierService.Next())
	.With(x => x.isEffect = true)
	.With(x => x.isDamageEffect = true)
	.AddEffectValue(value)
	.AddProducerId(producerId)
	.AddTargetId(targetId);
```

**Why:** with one value per component, what a consumer needs becomes expressible in its matcher instead of an `if` inside the loop. `ProcessDamageEffectSystem` requires `TargetId` in `AllOf(...)`, so an ability activation that has no target never enters the group — no null/zero checks anywhere.

### Events are single payload components

An **event is the exception** and does not follow the rule above. It is one named `XEvent` component carrying its own fields:

```csharp
[Game]
public class KillEvent : IComponent
{
	public int KillerId;
	public int DeadEntityId;
	public Vector3 Position;
	public float Overkill;
}

_entityFactory.Event()
	.AddKillEvent(producerId, target.Id, position, overkill);
```

Consumed as `GetEvents(GameMatcher.AllOf(GameMatcher.KillEvent))` — **always exactly one component in that matcher**. `GameMatcher.AllOf(params IMatcher[])` accepts only single-index matchers, so a composed event throws `MatcherException` at runtime.

**Never modify `EventGroupExtensions` to make a wider matcher fit.** Infrastructure bending to accommodate a new shape means the shape is wrong — a composed event forces consumers to type-switch inside the loop, exactly what matchers exist to remove.

If a payload must be *dispatched on* rather than read, it is not an event: unfold it into one entity per item, each carrying its own marker component, and let a system per marker match it.

`DeathEvent` and `DamageEvent` are the in-repo reference for the event shape. Check them before inventing a new one; if nothing equivalent exists, the construct is probably wrong.

### Systems hold no state

A system's fields may only be: the context, injected services/factories, `IGroup<GameEntity>` groups, and plain `List<GameEntity>` scratch buffers.

No `HashSet`, no counters, no cached values — **even if cleared at the start of `Execute()`**. If an algorithm needs "already handled" bookkeeping, check the result buffer itself (`_nearbyBuffer.Contains(candidate)`); if data must survive between frames, it belongs on an entity as a component.

### Differences between systems live in matchers, not in code

Gameplay features never declare their own abstract/base systems — `RequestHandlerSystem<T>` is infrastructure and the only base a gameplay system may inherit.

When two systems would share a loop and differ by a rule, express the rule as a component and write two plain systems whose only difference is the matcher:

```csharp
// SetNearestTargetSystem
_targets = AllOf(CurrentHP, Alive, WorldPosition, Id)

// SetDamagedTargetSystem
_damagedTargets = AllOf(Damaged, Alive, WorldPosition, Id)
```

If the rule isn't queryable yet, add the marker component plus a small `Mark*` system that maintains it (`Damaged` + `MarkDamagedSystem`, mirroring `MarkLifeStateSystem`). Priority between such systems comes from their **order in the Feature**, each carrying `NoneOf(...)` on the result component so the first success wins and the generic one is the fallback.

### Naming: verb + object, one action per class

- **Verb first, never a noun phrase:** `ApplyProjectileHitSystem`, not `HitProjectileSystem`. `SteerHomingProjectileSystem`, not `HomingProjectileSystem`. `SetTargetSystem` / `RemoveInvalidTargetSystem` / `AimWeaponSystem`.
- **Plainest word the team already uses.** No industry jargon that needs explaining — `ModuleSlot`, not `Hardpoint`.
- **One system does one thing.** If the name needs "and", split it: `SyncModuleToSlotSystem` became `UpdateModulePositionSystem` + `UpdateModuleAimDirectionSystem`.
- **`...On<Event>System` must name the actual event component** — `DestructOnDeathSystem` ← `DeathEvent`.
- **`Request` means a command**: the payload placed on a request entity (`AddPlayVfxRequest(new VfxSpawnRequest(...))`) or a "create me this" factory argument. Never a bag of query parameters — pass the entity and read its components instead.

### No speculative settings

Every field on a config must be demanded by a mechanic that already exists or is being written now. If nothing reads it, it isn't written — a jam has no room for options nobody uses.

Two mechanisms doing the same job is the same violation: "collect everything in radius" and "collect the N nearest" are one config with `maxTargets: 0` meaning everyone — not two code paths.
