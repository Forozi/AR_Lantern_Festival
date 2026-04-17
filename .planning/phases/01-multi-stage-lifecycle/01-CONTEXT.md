# Phase 1: Multi-Stage Lifecycle - Context

**Gathered:** 2026-04-17
**Status:** Ready for planning

<domain>
## Phase Boundary

This phase implements the transition from a single-session AR experience to a structured 3-stage gameplay cycle. It covers the state management for stages, timer resets, score thresholds for skipping, and the "intermission" countdown sequence.

</domain>

<decisions>
## Implementation Decisions

### Stage Progression
- **Structure**: 3 discrete stages (Stage 1, 2, and 3).
- **Stage Skipping**: If the player reaches a score threshold of **50 points** within a stage, they immediately advance to the next intermission/stage.
- **Timer Logic**: Each stage begins with a fresh **30-second timer**. Skipping a stage effectively "saves" time for the overall session but does not carry over time; the next stage always starts at 30s.
- **Finality**: Stage 3 always runs to the end of its 30-second timer (or until threshold) to conclude the game session.

### Difficulty Scaling
- **Speed Multipliers**: 
  - Stage 1: 1.0x (Base)
  - Stage 2: 1.5x speed increase
  - Stage 3: 2.5x speed increase
- **Application**: The multiplier should affect lantern movement speed and potentially spawn frequency (at the agent's discretion for feel).

### Transition & Intermission
- **Countdown Sequence**: Between stages, a visual countdown (e.g., 3-2-1) is displayed via UI.
- **Intermission State**: During this countdown, all gameplay models must be cleared/hidden, and the `SpawnerSystem` must be inactive.
- **Game Over**: Triggered when the timer hits zero on Stage 3 (or if the player fails to advance/complete the cycle).

### the agent's Discretion
- **UI Placement**: Determining the cleanest way to display "Stage Skip" and "Intermission" messages without cluttering the AR view.
- **Threshold Tweakability**: The internal architecture should allow for easy adjustment of the 50-point threshold as gameplay testing progresses.

</decisions>

<canonical_refs>
## Canonical References

**Downstream agents MUST read these before planning or implementing.**

### Core Gameplay Logic
- `Assets/Scripts/GameManager.cs` — Primary state machine and scoring logic.
- `Assets/Scripts/SpawnerSystem.cs` — Spawning logic and difficulty parameter handling.
- `Assets/Scripts/LanternBehaviour.cs` — Individual lantern movement and interaction events.

### AR Framework
- `Assets/Scripts/ARPlacementManager.cs` — Entry point that triggers the first stage.

</canonical_refs>

<specifics>
## Specific Ideas
- Introduce a `CurrentStage` enum or integer in `GameManager` to track progression.
- Create a `StageIntermission` state that handles the 3-2-1 countdown logic before enabling the spawner again.

</specifics>

<deferred>
## Deferred Ideas
- Boost Trigger Mechanic (Deferred to Phase 2)
- Global Boost State Transformations (Deferred to Phase 2)
- Stage-specific Spawning Patterns (Deferred to Phase 2)

</deferred>

---

*Phase: 01-multi-stage-lifecycle*
*Context gathered: 2026-04-17 via discussion*
