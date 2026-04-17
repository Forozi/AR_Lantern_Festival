# Directory Structure

**Analysis Date:** 2026-04-17

## High-Level Layout

```text
.
├── Assets/                 # Primary source directory
│   ├── Materials/           # URP Materials for lanterns and environment
│   ├── Prefabs/            # Reusable game objects (Lanterns, Indicators)
│   ├── Scenes/             # Unity scene files (Main Game)
│   ├── Scripts/            # C# Source code
│   ├── Settings/           # URP Asset and quality settings
│   ├── Sprites/            # UI sprites and 2D assets
│   └── TextMesh Pro/       # TextMesh Pro resources
├── Packages/               # Package manager manifest and lockfiles
├── ProjectSettings/        # Unity project configuration
└── ...                     # Build and IDE artifacts (.sln, .csproj)
```

## Key Locations

### Core Logic
- `Assets/Scripts/GameManager.cs`: entry point for game state.
- `Assets/Scripts/SpawnerSystem.cs`: logic for lantern generation and pooling.
- `Assets/Scripts/ARPlacementManager.cs`: logic for AR plane detection and initialization.

### Data & Prefabs
- `Assets/Prefabs/Lantern.prefab`: The primary interactive object.
- `Assets/Prefabs/PlacementIndicator.prefab`: Visual feedback for AR placement.

### Graphics
- `Assets/Materials/CursedLantern.mat`: Specialized material for cursed lanterns.
- `Assets/Settings/URP_Asset.asset`: Defines the render pipeline quality and features.

## Naming Conventions

- **Scripts**: PascalCase (`SpawnerSystem.cs`).
- **Materials**: PascalCase (`LanternGlow.mat`).
- **Prefabs**: PascalCase (`Lantern.prefab`).
- **Internal Fields**: Underscore camelCase (`_spawnTimer`).
- **Public Properties**: PascalCase (`Score`).

---

*Structure analysis: 2026-04-17*
