# COPYCAT playtest tools

Keep these tools for the animation/UI regression pass. Editor code lives in
`Assets/Code/Editor/*Playtest*`, `GameplayRegression.cs` and `Playtest*.cs`;
it is excluded from player builds. No production config is changed by the tools.

## Interactive recording

1. Open `Assets/Scenes/Boot.unity`, enter Play Mode, click START and wait for the exam.
2. Select `COPYCAT/QA/Record gameplay`. Recordings go to `Temp/Playtest/<UTC timestamp>/`.
3. Select `COPYCAT/QA/Use keyboard meow` to stop microphone capture for this Play Mode session.
   M still uses the actual game's input pipeline. `Use microphone` restores capture.
4. `Use diagnostic view` temporarily hides `CopycatArt` and enables the existing greybox.
   `Use game art` restores the art and original camera size, including after scene reloads.
5. Play using Space (hold to lean), M, Q, arrows/WASD, 1–4 and letters. Esc restarts.
6. `Capture Game View` captures the full Game View including overlay canvases.
   Automatic `frame-*.png` images capture only the gameplay camera, paired with `frame-*.txt`.
   `states.txt` records ECS/input snapshots approximately every 0.2 seconds.
7. Stop recording or exit Play Mode. Tools never start recording automatically.

For automation, write a **new unique line** to `Temp/Playtest/command.txt`, e.g.
`record run-1`, `keyboard run-1`, `greybox run-1`, `art run-2`, `capture run-3`,
`gameview run-3`, `snapshot run-3`, `focus run-3`, `end run-3`, `microphone run-3`.
Commands are processed one at a time; wait for the editor between writes.
An existing command is ignored on entering Play Mode. Latest snapshot: `state.txt`.
Recorder failures: `error.txt` (check its timestamp; old files are not deleted).

Raw recordings are temporary/ignored by Git. Copy selected **game-only** evidence outside
`Temp` when attaching a bug; never commit desktop screenshots containing private windows.
Recording renders extra frames and can affect performance; do not use it to benchmark FPS.

## Real macOS keyboard player

Run from `src/gamejam-template` with Swift and Node installed:

```sh
swiftc tools/playtest/input-macos.swift -o Temp/Playtest/input-macos
pgrep -alf '/Unity.app/Contents/MacOS/Unity'
node tools/playtest/player.cjs UNITY_PID
```

Start recording first, mute the microphone using the menu, restart the attempt,
then give Unity's Game View focus. The host app/terminal needs macOS Accessibility access.
The helper requires the specified PID to be Unity and frontmost. It stops on focus loss;
avoid clicking elsewhere during a run. Do not approve unrelated GUI prompts mid-run.
It sends OS key-down/up events, **not ECS flag writes**. Individual actions:

```sh
Temp/Playtest/input-macos UNITY_PID down:46 wait:0.12 up:46
Temp/Playtest/input-macos UNITY_PID down:49 down:126 wait:0.12 up:126 up:49
```

The adaptive player reads the recorded ECS state as an oracle, and its answer keycodes
match the current 12-question deck. This tests the input pipeline and mechanics;
it does **not** prove that a human can read the final art or that animations are correct.
When the deck changes, update the answer sequences. Inspect paired images separately.
Exit code 0 means Passed; 1 means a failed attempt/error. It does not tune difficulty.

## Repeatable gameplay checks

In the loaded exam: `COPYCAT/QA/Test gameplay boundaries`.
`Temp/Playtest/regression.txt` must contain 14 PASS lines, no FAIL and DONE.
The tests run production systems with isolated Game/Input contexts and a controlled clock;
they do not replace the live run or write saves. This is boundary coverage, not keyboard coverage.
Do not mistake a NOT RUN report (exam not loaded yet) for a passing test.

`COPYCAT/QA/Test error window rendering` also runs outside Play Mode. It instantiates
the error prefab, renders a long marked-up log as bounded plain text, checks short logs,
and destroys the instance. `Temp/Playtest/error-window.txt`: two PASS lines and DONE.

## Leaderboard fixture

```sh
node tools/playtest/leaderboard-fixture.cjs
node ../../tools/leaderboard/test.js
```

Keep the first process running, then select `COPYCAT/QA/Test leaderboard` in Play Mode.
`Temp/Playtest/network.txt` must contain 50 PASS lines, no FAIL and DONE.
The fixture binds only localhost:18764, executes the repository's real `Code.gs`
against an in-memory sheet, and resets **only that in-memory sheet** on each test.
The Unity service uses a transient config; production URL/assets are never overwritten.
Stop the fixture with Ctrl-C when done.

Coverage: 20 posts, name cleanup, top ten/ranking/sorting, offline service flag,
HTTP 503, malformed JSON/contracts, 5-second timeout, cancellation and recovery.
This does not validate real Google deployment permissions, network disconnection at OS level,
or the unfinished Report Card integration. Never point this fixture runner at the live sheet.

## Animation pass

Record using **Use game art**, not diagnostic view. Check pose/state agreement, telegraph
timing, paw occlusion, paper readability while leaning, duck flight/pickup/return/confiscation,
HUD, final results and repeated restarts. Use full Game View captures for overlays.
Repeat the isolated regression and keyboard player, but also complete a visually driven pass.
