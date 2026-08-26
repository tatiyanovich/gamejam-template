---
paths:
  - "**/AddressableResources/**"
description: Addressables asset organization, naming, and grouping rules
---

# Addressables Guidelines

All assets that should be built into Addressables bundles must be located in `Assets/AddressableResources` . Keep in mind that it's just the regular folder, when you put something in it it doesn't become an addressable.

Inside of the `AddressableResources` the main rule is to sort folders by features and usage (by usage means the assets that are often used together ideally should share one addressable group):

Specific feature folder itself also can contain a set of sub-feature folders inside:

Every feature should be a **separate** addressable group:

Addressable group should be named in [PascalCase](https://www.theserverside.com/definition/Pascal-case).
Group members should be named in [snake\_case](https://developer.mozilla.org/en-US/docs/Glossary/Snake_case).
**Every** feature folder should be included as a member of this group and named with the suffix “\_folder” in the end. It helps to avoid duplicate issues when a new asset is added in the folder, but not into the addressables. As a folder itself is an addressable, all the insides of the folder are addressables too.
If you need some asset available by the key you can enable the `Addressables` checkmark on it and it will become available by the key. By default the path will be used as a key, but you should rename it to a readable name.
For example: `Assets/AddressableResources/Visuals/PickUps/PickUp.prefab`
Should be renamed to: `pick_up_prefab`
Also don’t forget to move the new appeared addressable from `Default Local Group` into the group it belongs to:
Name all assets with its type as a suffix in the end. It helps to keep addressables readable and clean:
`Folder = _folder`
`Prefab = _prefab`
`Texture = _texture`
`Material = _material`
`ScriptableObject = _config`
`ParticleSystem = _vfx`
And so on.
If some assets are shared between several features you can extract it into separate “Shared” group.