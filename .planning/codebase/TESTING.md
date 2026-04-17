# Testing

**Analysis Date:** 2026-04-17

## Testing Landscape

Currently, the codebase does **not** have formal automated tests in the `Assets/` directory. However, the infrastructure for testing is present in the project configuration.

## Frameworks

**Unity Test Framework:**
- **Status:** Integrated via `com.unity.test-framework` 1.6.0.
- **Purpose:** Supports EditMode and PlayMode tests.

## Current Practices

- **Manual Testing**: Primary method of verification. Game loops and AR placement are tested directly on Android hardware or via the Unity XR Simulation environments.
- **Logging**: Extensive use of `Debug.Log` in `GameManager.cs` and `SpawnerSystem.cs` to verify state transitions and score calculations.

## Testable Components

The following components are prime candidates for future automated tests:
- **`GameManager`**: Scoring logic and high score persistence (EditMode or PlayMode).
- **`SpawnerSystem`**: Object pooling efficiency and spawn probability (PlayMode).
- **`LanternBehaviour`**: Movement math and boundary checks (EditMode).

---

*Testing analysis: 2026-04-17*
