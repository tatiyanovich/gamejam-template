---
name: review-branch
description: Review a whole feature branch against the project's Entitas architecture — logic, SRP, naming, duplication, consistency, readability. Produces a scored review by category. Use when asked to review a branch, a feature, or "посмотри всю ветку".
---

Review a branch as a senior developer with 10+ years in Entitas on **this** architecture.
Not a generic C# review — every finding must be anchored in a project rule or an existing
feature that already solved the same problem.

Target: $ARGUMENTS (branch name, feature name, or empty → the current branch)

**Out of scope:** tests, documentation quality, `.meta` files, `Assets/Code/Generated/`,
`Assets/Plugins/`. Report only what changes the code.

---

## 1. Resolve the scope — do this first, do not skip

The session-start git snapshot **goes stale**. Always re-resolve:

```bash
git rev-parse --abbrev-ref HEAD
git branch -a
git merge-base origin/develop <branch>
git diff --name-status origin/develop...<branch> -- "*.cs" | grep -v Generated
```

Rules:
- Base is `origin/develop` unless the branch forked from `main` — check `merge-base` against both
  and take the one that yields the smaller, coherent diff.
- Use three-dot (`...`), not two-dot.
- If HEAD differs from what the user seems to mean, say so in one line and review the branch
  that is actually checked out.
- If the diff is >150 non-generated C# files, the branch is a merge of several features —
  ask which one, or review the newest feature folder only and say that you did.

State the resolved scope in the first line of the review: branch, base, file count, feature name.

## 2. Read it — in bulk, not file by file

```bash
cd <feature folder> && for f in $(find . -name "*.cs" | sort); do echo "=====FILE: $f"; cat "$f"; done
```

Then the non-feature part of the diff:

```bash
git diff origin/develop...<branch> -- "*.cs" ":(exclude)*<Feature>*" ":(exclude)*Generated*"
```

Read **every** system, component file, service, factory, query, config and view in the feature.
Skim nothing — SRP and duplication findings only appear when you have all of them in context.

## 3. Load the reference set

Read `references/patterns.md` (next to this file). It distils the yardstick features into ~8 KB
instead of ~95 KB of source, and names which features are Tier 1 (trust as yardsticks) vs Tier 2
(good structure, known limits — do not cite them as duplication yardsticks).

`code-style.md` and `ecs-conventions.md` auto-load on `**/*.cs`, so they are already in context.

**Open the real reference file — not the doc — whenever a finding depends on what the code
currently does**: a suspected dead matcher, system ordering, a save round-trip, whether a base class
attaches something. `patterns.md` describes shapes, and a doc that has drifted from code will make
you confidently wrong.

## 4. Checklist — walk all of it

### Layering
- View reads via Queries, writes via Requests or service calls. Never touches entities directly.
- Domain services must not reach into `View.gameObject` / `SetActive` / `Transform`. Visibility is
  domain state (`Hidden`) + a system (`HideOnDeathSystem` is the precedent).
- Game-flow orchestration (fade → suspend → switch camera → branch) belongs in a system or state,
  not in a `WindowBase`.
- Storage holds no gameplay logic. Systems never write the save file outside `RefreshSnapshotsFeature`.
- No `Camera.main`, no `FindObjectOfType`, no statics smuggled into queries/services.

### SRP
- One system = one verb + one object. If the name needs "and", split it.
- Count the responsibilities in every system's `Execute`. Three private methods doing unrelated jobs
  = three systems.
- Systems hold **no state**: only deps, groups, and `List<GameEntity>` scratch buffers.
  No `HashSet`, no counters, no cached "last frame" values — put remembered data on entities.
- No gameplay system base classes. Differences go in matchers (add a marker), not in inheritance.
- Factories create. A factory method that mutates an existing entity is misnamed and misplaced.
- Polling vs request: if a system computes something expensive every frame just to find out there is
  nothing to do, it should be request-driven.

### ECS modelling
- One value per component. No multi-field components, no struct blobs. Events are the exception —
  one named `XEvent` component carrying its fields.
- Never compose events; never touch `EventGroupExtensions`.
- Entity existence: `group.ContainsEntity(entity)`, **never** `entity == null`.
- Configs are never null-guarded — assume loaded at bootstrap, fail loud.
- Gate flags use `AllowX` + `ForbidXOnY` (Allow first), never a single Refresh.
- `NoneOf` is for describing the result set, not for separating two writers — order them in the Feature.
- `[Watched]` only where a reactive query actually consumes the change.
- `isPersistAcrossLoopNodes` on anything that must survive a loop-node transition.
- Reactive queries: `IReactiveQuery.ReactToChanges` + `[Watched] XChanged` matcher, driven by
  `NotifyQueryChangesSystem`. Never hand-subscribe to `group.OnEntityAdded`/`OnEntityRemoved`.

### Naming
- One concept, one word. Grep the feature for synonyms of the same entity (packet/stone/batch,
  target/seeker/enemy). Two vocabularies for one entity is a finding.
- Verb + object, plainest word, no jargon. `Request` = a command, not a noun bag.
- Check the new names against the whole codebase for collisions and for near-duplicates that mean
  different things (`Credits` vs `Money`, `Progress` vs `Progression`).
- Config service accessors: `GetConfig(id)` for catalogs, `Config` for a singleton config —
  never `XConfigsService.XConfig` stuttering.
- Enum members carry a trailing comment when the meaning is not obvious from the name.

### Duplication
- Does this feature reimplement something that already exists? Check Highlight (flash), Vfx,
  Cooldown, Targeting, Effects, ItemConfig (`ViewAsset` already maps id → prefab), Movement.
- Two mechanisms for one job is the cardinal sin — e.g. adding currency both via `CurrencyRequest`
  and via a direct service call.
- Repeated nested loops over a singleton entity (`foreach board`) across several systems.
- Copy-pasted `Update()` timers / lerps across MonoBehaviours.

### Feature file — the user always asks about this
- `XFeature.cs` must have **blank lines grouping systems by phase**, like `CollectablesFeature`,
  `TargetingFeature`, `EffectsFeature`. A flat block of 6+ `Add(...)` calls is a finding.
- Ordering must be defensible: infrastructure → gate flags → gameplay → scoring → cleanup.
- Sub-features (`XActivationFeature`, `InitializeXFeature`) when the pipeline has distinct stages.

### Data & configs
- `readonly struct` + constructor for value types passed around (`SkillRequest`, `SkillValue` are the
  yardstick). Mutable structs with public fields and object-initializers are off-style.
- No speculative config fields — every field must be demanded by a named mechanic.
- Config vs prefab split: balance in the config, anything eyeballed in the scene on the prefab.
- No magic numbers in systems. Consts named, or on the config.
- A service must not hand out a reusable internal `List` as its return value.

### Code style
- Tabs. Explicit types (no `var`). `_` field prefix. `== false` over `!`.
- Member order: consts/fields → ctor → public → private.
- Matchers fully expanded and indented, even for one component. Events fluent on the next line.
- `SF` alias for `SerializeField`. No `///` or `//` comments unless they explain a *why*.
- Folder layout per `FolderStructure.md` — `Data/Systems/Services/Configs/Queries/Behaviours/Snapshots`.
  Never an invented folder name.

### Correctness passes worth doing explicitly
- **Dead matchers**: for every `.AllOf(A, B)`, confirm something actually adds both. The classic
  miss is `AllOf(View)` on an entity built by a base class that never attaches an `EntityView`.
- **Dead guards**: `if (x.Count == 0) continue;` on a value that can never be 0.
- **Physics in update**: `Rigidbody2D` velocity writes must live in a fixed-update feature.
- **Save/load round-trip**: walk snapshot write → null-out → restore. Does completed state survive
  a restart? Can anything be earned twice?
- **System order**: does a consumer run before its producer in the Feature?
- **Doc vs code**: if the branch adds or touches `docs/features/<Feature>.md`, read it as a
  statement of *intent* and check the code against every concrete claim it makes. You are not
  reviewing the doc's quality — you are using it as a free list of assertions to falsify. A doc
  saying "this toggles the view of every board, slot and stone" is a testable claim, and the entity
  that turns out to have no `View` component is the finding. Report the discrepancy as a code bug,
  and note the stale line so the doc gets fixed with it.

## 5. Verify before you claim

Every finding must be one of:
- **verified** — you read the code path end to end (say so plainly), or
- **flagged** — you state the condition under which it breaks.

Never assert a bug you have not traced. If a claim depends on how a base class behaves, open the
base class. If it depends on ordering, open the Feature. Wrong findings cost more than missed ones.

Credit what is done well, concretely, with the same rigour — a review that is only negative is not
usable for prioritising.

## 6. Output

Write in the user's language (Russian if they wrote in Russian). Structure:

1. **Scope line** — branch, base, file count, what the feature is.
2. **Overall score `X.X / 10`** and one sentence on where the points went.
3. **Categories**, each with its own `— N/10`. Use only the categories that have something to say;
   typical set:
   - Архитектура и слои
   - SRP систем
   - ECS-моделирование
   - Нейминг и консистентность
   - Пробелы по смыслу в фиче
   - Дублирование
   - Сложная логика
   - Правила проекта
   - Save / Load
4. **Что чинить, по порядку** — a ranked list, cheapest-highest-impact first, each item one line.

Rules for the body:
- Anchor with `path/File.cs:line`. Line numbers must be real.
- Quote code only when the shape is the point (e.g. a flat Feature block). 8 lines max.
- Every "inconsistent" claim names the file it is inconsistent *with*.
- No filler, no restating what the code does. If a paragraph does not change what the user would do,
  cut it.
