---
paths:
  - "**/Assets/Code/**"
description: Feature-based folder structure for the code directory
---

Folder structure is feature-based.

Assets/Code/
+-- Common/                    # Shared components and utilities
|   +-- CommonComponents.cs    # Like TargetId, ProducerId, Ready, etc.
|   +-- Extensions/            # Extension methods
|   +-- Providers/             # Component providers for views
|   +-- Utilities/             # Utility classes
|
+-- Gameplay/                  # All gameplay features
|   +-- Asteroids/             # Asteroid specific
|   |   +-- Providers
|   |   +-- Configs
|   |   +-- SelfInitializables
|   |   +-- Services
|   |   +-- Snapshots
|   |   +-- Systems
|   |   +-- AsteroidsComponents.cs
|   +-- Collisions/            # Collision detection and response
|   +-- Effects/               # Heal, Damage etc. effects system
|   +-- Input/                 # Input handling
|   +-- Lifetime/              # Entity lifetime management
|   +-- Movement/              # Movement
|   +-- Spaceship/             # Player spaceship
|   +-- Spawners/              # Generic spawner systems
|   +-- GameplayCoreFeature.cs # Main gameplay feature composition
|
+-- Infrastructure/            # Core infrastructure modules
|   +-- ConfigsManagement/     # Configs loading
|   +-- Installers/            # Zenject installers
|   +-- StateManagement/       # Game state machine and states
|   +-- EntryPoint.cs          # Application entry point
|
+-- UI/                        # UI systems and windows
|   +-- HUD/                   # Heads-up display
|   |   +-- Systems            # Specific to HUD systems
|   +-- Restarting/            # Restart functionality
|   +-- Systems/               # Common UI systems
|
+-- Storage/                   # Save/load and snapshots
|
+-- Generated/                 # Auto-generated Entitas code

## Never invent a folder name

Use only folder names that already exist in `Assets/Code`: `Systems`, `Services`, `Configs`, `Data`, `Queries`, `Providers`, `Behaviours`, `Snapshots`, `Extensions`, `Utilities`.

Serializable value types, structs and effect descriptors go in the feature's **`Data/`** folder (`Loot/Data`, `Vfx/Data`, `Effects/Data`) — not in a new category such as `Descriptors/`. Before creating a folder, check `find Assets/Code -type d` for a name that fits; if none does, the type usually belongs in an existing feature folder rather than a new one.

Renaming such a folder is not free: the namespace also appears in the `ns:` of every `[SerializeReference]` block inside `.asset` files, and those must be migrated with it.
