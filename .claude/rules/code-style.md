---
paths:
  - "**/*.cs"
description: C# code style rules enforced across the project
---

## Code Style (enforced)

- **Tabs** (size 4), not spaces. Max 120 chars per line.
- **Explicit types** — never use `var`.
- **Private fields** prefixed with `_` (except `[SerializeField]` fields).
- **No abbreviations** (use `Position` not `Pos`, `Rotation` not `Rot`).
- **Conditions:** use `== false` instead of `!`.
- **Events:** named `OnX`, handlers named `HandleX`. No lambda event handlers.
- **Max 3 method params** — extract to struct (suffixed `Request`/`Response`/`Dto`) if more. Exception: constructors and `Construct` methods.
- **Named arguments** each on own line.
- **Class member order:** serialized fields → internal variables → injected fields → constants → properties → events → constructor → lifecycle → subscribe/unsubscribe → setup/cleanup → update/execute → internal methods → handlers.
- **Group fields by role with a blank line** between groups. In systems the order is: context (`GameContext`/`InputContext`) → injected services/factories → ECS groups (`IGroup<>`) → scratch buffers (`List<>`/etc.). Example:
  ```csharp
  private readonly GameContext _game;

  private readonly IEntityFactory _entityFactory;
  private readonly IPlayerConfigsService _playerConfigsService;

  private readonly IGroup<GameEntity> _players;
  private readonly IGroup<GameEntity> _runs;
  private readonly IGroup<GameEntity> _damageables;

  private readonly List<GameEntity> _buffer = new(4);
  ```
- **Systems must be stateless** — no instance fields that change over time.
- **Events — fluent on the next line.** `_entityFactory.Event()` on its own line, `.AddXEvent(...)` indented on the next line. Never `_entityFactory.Event().AddXEvent(...)` on one line.
- **Matchers — always fully expanded and indented, even for a single component.** `GameMatcher` on the `GetGroup(` line, `.AllOf(`/`.NoneOf(` each on their own line, each component on its own line (see `AccumulateDrilledDistanceSystem`). Never the compact `GameMatcher.AllOf(GameMatcher.X)` or `.AllOf(GameMatcher.X)` form.
- **Filter with matchers, not `hasX`** — never guard iteration with `if (entity.hasFoo)`. Add `GameMatcher.Foo` to the group's `AllOf(...)` (or `NoneOf(...)`) so the group only ever contains valid entities. Access `entity.Foo` directly inside the loop. Same principle as favoring `group.ContainsEntity` over `== null`: express presence/absence in the query, not in per-entity branches.
- **One class/interface per file** (exception: generic overloads with same name).
- **Namespaces** mirror folder paths: `namespace Code.Gameplay.Effects.Systems`.
- **Don't null-check configs** — assume ScriptableObject configs and their fields (loaded via `IConfigsService`/Addressables) are set up; skip defensive null-checks and warning logs. A `NullReferenceException` is the intended signal that a config is missing or mislabeled.
