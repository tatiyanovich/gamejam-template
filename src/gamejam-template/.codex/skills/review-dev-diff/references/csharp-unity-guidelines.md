# C# and Unity guidelines

Read the repository's canonical rules before reviewing or editing C#:

- `../../../../../../.claude/rules/code-style.md`
- `../../../../../../.claude/rules/ecs-conventions.md`
- `../../../../../../.claude/rules/FolderStructure.md`
- `../../../../../../.claude/rules/view-reactive-queries.md` when a query or view changes

The non-negotiable review points are:

- tabs, explicit types, `_`-prefixed private fields, and `== false` instead of negation;
- one value per entity component, stateless systems, and matcher-driven filtering;
- views read through queries and write through requests or services;
- systems do not mutate save files during gameplay; snapshot refresh systems own persistence writes;
- system ordering is part of behavior, and fixed-step physics writes belong in fixed-update features;
- feature-based folders and existing folder names are preserved;
- generated Entitas code is regenerated, never hand-edited.

If a file already follows a stricter local convention, preserve it.
