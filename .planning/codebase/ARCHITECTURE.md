# Architecture

**Analysis Date:** 2026-04-17

## Overview

The codebase is a session-based AR game built using the **Singleton** and **Event-Driven** patterns. It focuses on a simple "spawn and interact" loop within an AR-enabled space.

## Pattern & Layers

### 1. Global State Management (Singletons)
- **`GameManager`**: Acts as the central authority for game state (Start, Score, Time, GameOver). It is a persistent Singleton throughout the game session.

### 2. System Decoupling (Events)
- **`LanternBehaviour`**: Uses static events (`OnLanternHit`, `OnLanternEscaped`) to notify systems about lantern-specific actions without direct coupling to `GameManager` or `SpawnerSystem`.

### 3. Resource Management (Object Pooling)
- **`SpawnerSystem`**: Uses `UnityEngine.Pool` to efficiently manage the lifecycle of `LanternBehaviour` instances, reducing garbage collection overhead during frequent spawning/despawning.

### 4. Hardware Abstraction (AR Foundation)
- **`ARPlacementManager`**: Handles the boundary between hardware (ARCore/AR Foundation) and game logic. It translates physical gestures and plane detection into a starting anchor for the game.

## Data Flow

1.  **Placement**: `ARPlacementManager` performs raycasts → User confirms placement → `spawnerSystem.SetAnchorAndInitialize()` is called.
2.  **Start**: `GameManager.StartGame()` is triggered → Timer begins.
3.  **Spawn Loop**: `SpawnerSystem` updates `_waveTimer` → `SpawnBatch()` retrieves lanterns from pool → Lanterns are initialized with types (Cursed/Blessing).
4.  **Interaction**: User interaction (Tap/Shoot) → `LanternBehaviour` triggers `OnLanternHit` → `GameManager` updates score.
5.  **Termination**: Timer reaches zero → `GameManager` enters `GameOver()` state → Time scale set to zero → Persistence via `PlayerPrefs`.

## Key Abstractions

- **`SpawnPoint`**: A internal class in `SpawnerSystem` that tracks spatial availability and occupancy.
- **`LanternType`**: Enum defining the behavior and scoring logic for different lantern types.

---

*Architecture analysis: 2026-04-17*
