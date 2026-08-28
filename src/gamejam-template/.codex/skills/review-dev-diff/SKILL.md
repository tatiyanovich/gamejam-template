---
name: review-dev-diff
description: Review the current gamejam-template branch or working-tree diff against its upstream branch using the repository's Unity, Entitas, and C# conventions. Use for branch reviews and infrastructure audits; exclude generated code and third-party plugins from style findings.
---

# Review development diff

Resolve the scope from live Git state. Do not rely on an earlier status snapshot.

1. Read [references/csharp-unity-guidelines.md](references/csharp-unity-guidelines.md) and [references/review-rules.md](references/review-rules.md) completely.
2. Resolve the repository root, current branch, upstream branch, merge base, staged diff, unstaged diff, and untracked C# files.
3. Read every changed non-generated C# file. For an infrastructure audit, also trace each named subsystem end to end through its installers, assets/configs, state transitions, and cleanup.
4. Verify findings against the current implementation. Open base classes, feature ordering, DI bindings, and snapshot round trips before asserting a bug.
5. Report findings first, ordered by severity, with real `path:line` anchors. Distinguish verified defects from conditional risks. If no defects remain, say so and list the checks performed.

Treat `Assets/Code/Generated/`, `.meta` files, and `Assets/Plugins/` as out of style-review scope. Inspect them when needed to verify integration or generated-code consistency.
