# Coding Conventions

**Analysis Date:** 2026-04-17

## Code Style

### Formatting
- **Indentation**: 4 spaces (standard C# Unity).
- **Line Endings**: CRLF/LF.
- **Braces**: New line for opening and closing braces (Allman style).

### Naming
- **Classes**: `PascalCase` (`GameManager`).
- **Interfaces**: `IPascalCase` (e.g., `ILanternInteractable` if it existed).
- **Private Fields**: `_camelCase` (`_isInitialized`).
- **Public Properties**: `PascalCase` (`Score`).
- **Methods**: `PascalCase` (`StartGame`).
- **Variables**: `camelCase` (`spawnRadius`).

## Common Patterns

### Singleton Pattern
Used for world-level systems like `GameManager`.
```csharp
public static GameManager Instance { get; private set; }
void Awake() {
    if (Instance == null) Instance = this;
    else Destroy(gameObject);
}
```

### Event Pattern
Used for cross-system communication to avoid tight coupling.
```csharp
public delegate void LanternAction(LanternBehaviour lantern);
public static event LanternAction OnLanternHit;
```

### Dependency Injection (via Inspector)
References are primarily assigned through the Unity Editor using `[SerializeField] private` fields.

## Error Handling
- **Messaging**: Primarily `Debug.Log` and `Debug.LogError`.
- **Validation**: Minimal use of `Asserts`. Most systems rely on null checks before interaction.

## Tooling
- **Static Analysis**: Standard VS/Rider Roslyn analyzers.
- **Project Structure**: Standard Unity "Assets-centric" organization.

---

*Conventions analysis: 2026-04-17*
