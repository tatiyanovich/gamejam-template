---
name: commit
description: Inspect unstaged + untracked changes, group them by logical concern, and commit each group as a separate commit on the current branch. Use when the user has been making changes across multiple unrelated topics and wants a clean history.
---

Split the user's pending changes into logical commits.

## Steps

### 1. Survey the working tree

Run these in parallel:

- `git status` — full picture of modified, deleted, untracked files
- `git diff --stat` — change-size context per file
- `git log --oneline -10` — match the repo's commit-message style
- `git ls-files --others --exclude-standard <untracked-folders>` — expand any untracked directories so you see every new file

### 2. Inspect the actual diffs

Read the full diffs for code/config files. **Never** group based on filenames alone — open the diff and confirm what each change does. For untracked files, read them directly.

For Unity binary-ish assets (`.prefab`, `.asset`, `.unity`, `.mat`, `.png`), inspect the YAML diff — these often touch many lines but represent a single logical change.

### 3. Group by logical concern

Each commit should answer one question: *"what did this change accomplish?"*

**Bias toward more, smaller commits.** A single new feature almost always decomposes into multiple layer- or concern-specific commits. Do NOT lump an entire feature into one commit just because the parts share a theme — if a reviewer would want to read the backend changes independently from the UI changes, they belong in separate commits.

**But never split files that depend on each other and can't work apart.** If commit A would leave the codebase broken without commit B (compile errors, missing types, dangling references both ways), they belong together. Split only along directions where the *earlier* commit stands on its own:

- Backend ↔ UI: backend usually compiles without UI, so it can ship first; UI that references new backend types must come after. **Order matters — commit the dependency first.**
- If two files reference each other (mutual dependency), keep them in one commit.
- If a UI change requires a new backend type to even compile, do not commit the UI alone — either bundle them, or commit backend → UI in that order.

The test: after each commit, the codebase must build. If a proposed split would leave a broken intermediate state, either reorder so the dependency lands first, or merge the dependent pieces into a single commit.

Within a single feature, split by sub-concern. Typical layers (commit each as its own group when the changes exist):

- **ECS / domain backend** — components, systems, services, factories, feature class, DI binding (`Feature` add + installer line).
- **UI / view layer** — widget MonoBehaviours, widget prefabs, widget configs, world-overlay root additions.
- **Configs & data assets** — new ScriptableObject assets, addressables groups/labels for those assets.
- **Asset / prefab wiring on specific consumers** — hookups on entity prefabs that adopt the feature (e.g. `Customer_Survivor` gaining a pivot + behaviour).
- **Debug / editor-only additions** — `#if UNITY_EDITOR` buttons, Odin inspector helpers, debug utilities. Split out when they live in their own file or when the surrounding file has no other new logic.

Other splitting rules:

- **Source code + its codegen sibling** in the same commit ONLY if the codegen file is small and tightly scoped to that change. Otherwise put all `Assets/Code/Generated/` files into a single trailing `Regenerated entitas` commit.
- **Refactors / file moves** in their own commit, separate from new behavior.
- **Component additions** separate from the systems that use them, when the component is genuinely reusable.
- **Unrelated incidental fixes** (typos, unused imports) in a small dedicated commit, not buried inside a feature commit.
- **A reviewer must be able to read each commit independently** — that's the test. If backend and UI can be reviewed in isolation, they go in separate commits.
- **Order commits by dependency.** When splitting layers, the depended-on layer ships first (backend before UI, components before systems that use them, configs before factories that load them). Each intermediate commit must leave the build green.

If a change touches multiple concerns inside a single file and can't be cleanly split with `git add <path>` (e.g. a system class with a `#if UNITY_EDITOR` debug button mixed in), prefer to keep it whole and mention the trade-off in the commit body — do **not** use `git add -p` here unless the user explicitly asks.

### 4. Present the plan, then execute

Before committing, write out the proposed commits as a numbered list with the title of each and a one-line rationale. The list order **is** the commit order, so place dependencies before dependents (backend before UI, etc.). Then proceed to execute them in order — no need to wait for confirmation unless the grouping is non-obvious or risky.

**Example split for a new "Emote" feature** (illustrative — not every project change will have all of these):

1. Add Emote ECS backend — components, request, system, service, feature/installer registration.
2. Add EmoteBubbleWidget UI — widget MonoBehaviour, prefab, textures, world-overlay root.
3. Add Emote configs and addressables wiring — `Emote_Happy` config + `emote_configs` group/label.
4. Wire Customer_Survivor with emote pivot and animator.
5. Add inspector debug button to trigger emotes. *(skip if it lives inside an unsplittable file — note the trade-off in the previous commit's body instead)*
6. Regenerated entitas.

### 5. Stage and commit one group at a time

For each group:

- `git add` **specific paths only** — never `git add -A` or `git add .` (those can sweep in secrets, build artifacts, or unrelated edits).
- Commit with a HEREDOC message.
- **Message style:** short single-line explanation starting with a capital letter. Do NOT use conventional-commit prefixes like `feat:`, `fix:`, `refactor:`, `chore:` etc. Just describe what the change does in plain language (e.g. `Rename zone prefab folders to shorter names`, `Filter zone occupants by interactor type`). Codegen commits use the literal title `Regenerated entitas`.
- Always include the `Co-Authored-By` trailer.
- If a pre-commit hook fails, fix the underlying issue and create a NEW commit (do NOT `--amend`).

### 6. Verify

After all commits land:

- `git status` — must be clean (or only contain files the user explicitly asked to leave unstaged).
- `git log --oneline -<N+2>` — show the new commits at the top.

## Rules

- **Never** push to remote.
- **Never** amend existing commits.
- **Never** use `--no-verify` or skip hooks.
- **Never** delete untracked files or branches as part of "cleanup."
- If you find files that look sensitive (`.env`, credentials, keys), warn the user and skip them — do not commit.
- Use plain short titles starting with a capital letter — no `feat:` / `fix:` / `refactor:` / `chore:` prefixes.
- Codegen / generated files: bundle into one trailing `Regenerated entitas` commit unless they are the *only* change in scope.
- Keep each commit independently reviewable — a reader should understand the "why" from the title alone.
