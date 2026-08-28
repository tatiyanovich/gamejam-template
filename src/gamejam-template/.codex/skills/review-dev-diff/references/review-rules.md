# Review rules

## Scope

- Resolve the current branch and upstream with `git status --short --branch` and `git rev-parse`.
- Review committed branch changes, staged and unstaged changes, and relevant untracked files.
- Exclude generated code, `.meta` files, and third-party plugins from style findings, but inspect them for integration correctness.
- Read the complete feature or subsystem around a change; do not judge isolated lines without their feature order and DI wiring.

## Correctness passes

- Trace producer to consumer for every request, event, watched component, and reactive query.
- Confirm feature ordering, request cleanup, event cleanup, entity teardown, and loop-node persistence.
- Walk save/load as snapshot write, serialization, load, migration, and restore into ECS state.
- Walk windows as definition, Addressables config, prefab, layer, open/close ownership, and scene transition.
- Confirm ECS contexts, factories, systems, and generated lookup/components agree.
- Check async scene and Addressables handles for ownership and release.
- Check DI lifetime: cross-scene services belong in bootstrap scope; scene-only dependencies belong in scene scope.

## Findings

- A verified finding must be traced end to end and anchored with a real `path:line`.
- A conditional risk must state the condition that triggers it.
- Name the existing project rule or implementation pattern a finding violates.
- Prefer a small verified finding set over speculative warnings.
- Credit concrete strengths that materially reduce risk.

## Reference patterns

For deeper Entitas shape comparisons, read `../../../../../../.claude/skills/review-branch/references/patterns.md`. Open the real reference implementation whenever behavior, ordering, or lifecycle matters.
