---
description: Keep docs/ in sync with every action, decision and design change on the COPYCAT jam project
---

# Docs maintenance (always on)

`docs/` is the team's shared memory for the COPYCAT jam. Three people work in parallel without sleep;
if it is not written down, it did not happen. After **every** unit of work, update the docs **in the same turn**,
before reporting back to the user.

## What to update, when

| When you… | Update |
|---|---|
| finish a task, create/delete/rename files, generate assets, change a scene/prefab, run a build | append one line to `docs/PROJECT.md` **§9 Work Log** |
| make or change a decision (tech, design, scope, tooling) | add `D-NN` to `docs/PROJECT.md` **§4 Decisions** with a one-line *why*; never delete old ones, mark them `(отменено D-XX)` |
| change any gameplay number, rule, text, mechanic, content row | edit the matching section of `docs/GDD.md`; keep the tables as the single source of truth |
| start / finish / cut a task | flip its checkbox in `docs/PLAN.md` (`[ ]` → `[~]` → `[x]` / `[-]`); if you cut, add the reason to §4 «Линия отреза» |
| add an asset or change the art/audio pipeline | update the asset list or pipeline section in `docs/ART_BIBLE.md` |
| notice a bug you are not fixing right now | add a row to `docs/BUGS.md` |
| add a new window, feature folder, config or save-file field | update the mapping table in `docs/PROJECT.md` **§5** |

## Format

- Work Log line: `- YYYY-MM-DD HH:MM · Кто · Что сделано · Где/результат` (local time, 24h). One line per action, append-only, newest at the bottom.
- Docs are in Russian; all in-game strings quoted in docs are English, exactly as they appear in the game.
- Keep entries to one line. Details belong in code, configs or the GDD section, not in the log.
- Do not rewrite history or reorder sections. Do not touch `docs/prototype/`.

## What NOT to log

Reading files, searching, thinking, failed attempts that left no trace. Log outcomes, not activity.
