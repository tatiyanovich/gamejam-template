# Unity MCP Editor Work

This project has the Unity MCP server connected (`com.unity.ai.assistant` package). When the work requires changes that would otherwise need a human in the Unity Editor — **default to scripting those changes via Unity MCP instead of dumping a checklist on the designer**.

## When this applies

The Editor side of the project has a lot of structured artifacts. All of these are scriptable via MCP:

- Creating a new prefab from scratch (`PrefabUtility.SaveAsPrefabAsset` on a constructed GameObject)
- Modifying an existing prefab (`PrefabUtility.LoadPrefabContents` → mutate → `SaveAsPrefabAsset` → `UnloadPrefabContents`)
- Wiring a `[SerializeField]` on a ScriptableObject or component (`SerializedObject` + `FindProperty` + `ApplyModifiedPropertiesWithoutUndo` + `EditorUtility.SetDirty`)
- Setting an `AssetReferenceGameObject` field — that's `prefab.m_AssetGUID` inside the SO, settable via SerializedProperty
- Adding a prefab/SO to Addressables (`AddressableAssetSettingsDefaultObject.Settings.CreateOrMoveEntry(...)`, set `entry.address`)
- Toggling Play Mode (`mcp__unity-mcp__Unity_ManageEditor` with action `Play`/`Stop`)
- Reading the console (`mcp__unity-mcp__Unity_GetConsoleLogs` filtered by `error`)

**If you find yourself writing instructions like "now in the Editor, do X, Y, Z" — stop. Script it instead.** The exception is genuinely creative/visual work — material tweaks, animation curves, layout authoring — that's still on a human.

## RunCommand cheatsheet

The skeleton:

```csharp
using UnityEngine;
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;

internal class CommandScript : IRunCommand
{
    public void Execute(ExecutionResult result)
    {
        // ... your work ...
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        result.Log("Done.");
    }
}
```

The class **MUST** be named `CommandScript`, **MUST** be `internal`, and **MUST** implement `IRunCommand` — the harness rejects any other shape.

### Wire a SerializedField on a ScriptableObject

```csharp
ScriptableObject so = AssetDatabase.LoadAssetAtPath<ScriptableObject>(path);
SerializedObject serialized = new(so);
SerializedProperty assetGuidProp = serialized
    .FindProperty("prefab")
    .FindPropertyRelative("m_AssetGUID");
assetGuidProp.stringValue = AssetDatabase.AssetPathToGUID(targetPath);
serialized.ApplyModifiedPropertiesWithoutUndo();
EditorUtility.SetDirty(so);
```

### Build a new prefab with a child prefab nested in it

```csharp
GameObject prefabAsset = AssetDatabase.LoadAssetAtPath<GameObject>(childPrefabPath);

GameObject root = new GameObject(
    "RootName",
    typeof(RectTransform),
    typeof(MyWindowOrComponent));

RectTransform rect = (RectTransform)root.transform;
rect.anchorMin = Vector2.zero;
rect.anchorMax = Vector2.one;
rect.sizeDelta = Vector2.zero;

GameObject child = (GameObject)PrefabUtility.InstantiatePrefab(prefabAsset, root.transform);

PrefabUtility.SaveAsPrefabAsset(root, outputPath);
Object.DestroyImmediate(root);
```

### Remove a child from an existing prefab

```csharp
GameObject contents = PrefabUtility.LoadPrefabContents(prefabPath);
try
{
    Component target = contents.GetComponentInChildren<TheChildComponentType>(includeInactive: true);
    if (target != null)
        Object.DestroyImmediate(target.gameObject);

    PrefabUtility.SaveAsPrefabAsset(contents, prefabPath);
}
finally
{
    PrefabUtility.UnloadPrefabContents(contents);
}
```

### Register a prefab in Addressables (reusing an existing group)

```csharp
AddressableAssetSettings settings = AddressableAssetSettingsDefaultObject.Settings;
string newPrefabGuid = AssetDatabase.AssetPathToGUID(newPrefabPath);

// Reuse the group of a sibling addressable if you want them grouped together
AddressableAssetGroup group = settings
    .FindAssetEntry(AssetDatabase.AssetPathToGUID(siblingPath))?
    .parentGroup ?? settings.DefaultGroup;

AddressableAssetEntry entry = settings.CreateOrMoveEntry(newPrefabGuid, group);
entry.address = "human_readable_address";
settings.SetDirty(AddressableAssetSettings.ModificationEvent.EntryMoved, entry, true);
```

### Never write `using System.Reflection`

`RunCommand` dies with `UNEXPECTED_ERROR: Object reference not set to an instance of an object` — before compiling —
whenever the script contains a `using System.Reflection;` directive. Reflection itself works fine; only the directive is fatal.
Spell the types out instead:

```csharp
entityType.GetMethod("HandleStart",
    System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);

foreach (System.Reflection.FieldInfo field in component.GetType().GetFields())
```

### Reading live ECS state in Play Mode

The sandbox references `Assembly-CSharp` and Zenject but **not** the Entitas assembly, so a `GameContext`/`GameEntity`
typed variable fails with `CS0012`. Resolve into `object` and go through reflection:

```csharp
Zenject.SceneContext[] contexts = UnityEngine.Object.FindObjectsByType<Zenject.SceneContext>(FindObjectsSortMode.None);
object game = contexts[0].Container.TryResolve<GameContext>();

IEnumerable entities = (IEnumerable)game.GetType().GetMethod("GetEntities", Type.EmptyTypes).Invoke(game, null);

foreach (object entity in entities)
{
    Type entityType = entity.GetType();
    bool isExamRun = (bool)entityType.GetProperty("isExamRun").GetValue(entity);
    entityType.GetMethod("ReplaceAnswerProgress").Invoke(entity, new object[] { 3 });
}
```

To drive gameplay over several frames, subscribe a static method to `EditorApplication.update` and unsubscribe it from
inside on a terminal condition. It ticks slowly while the Editor is unfocused — budget a few seconds per tick.

### Resolve a project type by name (when the MCP-compiled CommandScript can't `using` your assembly)

```csharp
Type type = AppDomain.CurrentDomain.GetAssemblies()
    .SelectMany(a => { try { return a.GetTypes(); } catch { return Array.Empty<Type>(); } })
    .FirstOrDefault(t => t.FullName == "Code.UI.Joystick.JoystickWindow");
```

The RunCommand sandbox can't easily reference custom assemblies via `using`, so reflection is the reliable path for `typeof`.

## After mutating

Always end your script with:

```csharp
AssetDatabase.SaveAssets();
AssetDatabase.Refresh();
```

…and verify with `mcp__unity-mcp__Unity_GetConsoleLogs` filtered by `error` to catch broken references / compilation failures from your changes.

## When Play Mode is involved

Use `mcp__unity-mcp__Unity_ManageEditor` with `Action: GetState` to check `IsPlaying` / `IsCompiling`. Asset changes don't pick up while Play Mode is running — Stop it first with `Action: Stop, WaitForCompletion: true` if you need fresh compiles to land.

## What NOT to script

- Visual authoring (mesh tweaks, material edits, animation curves, lighting probes) — humans only.
- Anything that's a one-off design decision where the script would be longer than the manual click sequence.
- Importing external assets (textures, models) — those need pipeline settings a designer should approve.
