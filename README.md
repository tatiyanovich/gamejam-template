# Game Jam Template

A stripped-down Unity 6 + Entitas starter meant to be forked at the start of a jam. All the plumbing
you don't want to write under time pressure is already here and running; the gameplay is one trivial
slice you are expected to delete.

## What's in the box

| Area | What you get |
|---|---|
| ECS | Entitas + Jenny codegen, `Game` and `Input` contexts, factories, events, requests, reactive queries, destruct pipeline, system profiling |
| Boot flow | State machine from splash to gameplay: configs → save load → migrations → asset warm-up → scene load → running pipeline |
| Core loop | Node graph (menu / gameplay) with session branches, so multiple gameplay pipelines can tick at once |
| Save/Load | JSON save files, snapshot refresh pipeline, auto-save every 5s, versioned migrations |
| UI | Window/widget service with layers, history and side effects: loading screen with progress, fade, main menu, HUD, result popup, settings |
| Input | Axis + pointer input as ECS entities, on-screen joystick with keyboard fallback |
| Assets | Addressables-based asset service with labels and categories |
| Errors | Global exception catcher, log ring buffer, error screen, app reboot flow |
| Editor | Toolbar buttons: regenerate ECS code, wipe saves, jump between scenes, scale time |
| Agents | `.claude/` rules, 16 scaffolding skills and subagents tuned for this architecture |

## Getting started

1. Unity **6000.3.6f1** (URP, 2D). Open `src/gamejam-template/`.
2. Open the `Boot` scene and press Play. You should get loading → menu → gameplay.
3. Rename things: `Constants.GameName`, product name in Project Settings, the `src/gamejam-template/`
   folder (also update `Jenny/JennyRoslyn.properties` — it references that path twice).

## The sample gameplay slice

Drive around with WASD or the on-screen stick, touch a pickup, score goes up, score survives a restart.
It exists to prove the pipeline end to end: input → system → component → reactive query → HUD, plus
entity → snapshot → save file. Three files own it — `Assets/Code/Gameplay/Pickups/`,
`Assets/Code/Storage/Systems/RefreshScoreSystem.cs`, `Assets/Code/UI/Gameplay/GameplayWindow.cs`.
Delete them when your real gameplay lands.

## Adding a feature

Ask Claude Code to use the scaffolding skills — `create-feature`, `create-system`, `create-components`,
`create-query`, `create-request`, `create-event`, `create-config`, `create-service`, `create-factory`,
`create-snapshot`, `create-state`. They place files in the right folders and wire DI and feature
registration. `CLAUDE.md` has the architecture rundown; `.claude/rules/` has the conventions.

After adding or renaming any `[Game]`/`[Input]` component, regenerate the Entitas code: **Generate**
button in the Unity toolbar, or `./Jenny-Gen` from inside the `Jenny/` folder.

## Third-party

Bundled: Entitas, Zenject, UniTask, DOTween, NSubstitute, FluentAssertions, RequiredField, plus the
in-repo `Framework` layer (UI, storage, state machine, asset and scene services). No paid Asset Store
packages — add your own if you need them.
