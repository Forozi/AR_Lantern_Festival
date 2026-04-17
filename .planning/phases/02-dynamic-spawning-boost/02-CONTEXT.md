# Phase 2: Dynamic Spawning & Boost - Context

**Gathered:** 2026-04-17
**Status:** Ready for planning

<domain>
## Phase Boundary

This phase addresses the implementation of the special "Boost" lantern type, the trigger logic (random + catch-up), and the high-intensity "Boost State" spawning patterns. It also covers the logic for transforming active play into the specialized boost scoring mode.

</domain>

<decisions>
## Implementation Decisions

### Boost Trigger Logic
- **Availability**: Maximum **one Boost per stage**.
- **Random Spawn Chance**: 
  - Stage 1: 1.0% chance per spawn.
  - Stage 2: 0.8% chance per spawn.
  - Stage 3: 0.6% chance per spawn.
- **Guarantee (Catch-up)**: If precisely **20 seconds** have passed in a stage, the stage score is below 50, and no boost has triggered yet, a Boost lantern is guaranteed to spawn.
- **Visuals**: Boost lanterns are **Yellow** (Material/Color shift from base red/purple).

### Boost State Mechanics
- **Transition**: When the Boost trigger is shot:
  1. All currently active lanterns are "popped" immediately (0 points awarded).
  2. The game enters the **Boost State** for exactly **10 seconds**.
- **Boost Spawning Pattern**:
  - Immediate spawn of multiple "rings" or rows of lanterns.
  - Stage 1: 2 rows stacked vertically.
  - Stage 2: 3 rows stacked vertically.
  - Stage 3: 4 rows stacked vertically.
- **Scoring**: During Boost State, all lanterns are of the Boost type.
  - Hits: **10 points**.
  - Escaped (Height threshold): **0 points**.

### Post-Boost Logic
- **Duration**: 10 seconds (Fixed).
- **Threshold Completion**: If the player reaches the stage threshold during the 10-second Boost State, the game **does not** advance immediately. The player continues to play for the full 10 seconds to maximize points.
- **Outcome A**: Once the 10 seconds expire, if the stage threshold has been met, the game advances to the next intermission/stage.
- **Outcome B**: If 10 seconds expire and the threshold is NOT met, the game reverts to the previous stage's spawning mechanism for the remaining time of the 30s session.

### Stage Thresholds & Game Over
- **Stage 1**: 50 points. Failure to reach this by 30s results in **Game Over**.
- **Stage 2**: Stage 1 Score + 50 points. Failure to reach this by 30s results in **Game Over**.
- **Stage 3**: No skip threshold. The game ends naturally when the 30s timer expires.


### the agent's Discretion
- **Pooling Expansion**: Ensure the object pool for lanterns can handle the increased capacity required for stacked 4-row rings in Stage 3 (up to 48 lanterns simultaneously).
- **Spawn Interval**: Determining the optimal gap between rows (vertical offset) so the rings look distinct but readable in AR.

</decisions>

<canonical_refs>
## Canonical References

**Downstream agents MUST read these before planning or implementing.**

### Logic Controllers
- `Assets/Scripts/GameManager.cs` — For handling the 10s state timer and the "Once Per Stage" flag.
- `Assets/Scripts/SpawnerSystem.cs` — For the stacking row logic and random chance calculations.
- `Assets/Scripts/LanternBehaviour.cs` — For the Yellow visual state and the specific boost scoring event.

</canonical_refs>

<specifics>
## Specific Ideas
- Use a boolean `_hasBoostTriggeredThisStage` in `GameManager` (reset on `AdvanceStage`).
- Implement `TriggerBoostMode()` in `SpawnerSystem` that overrides the standard wave timer while active.

</specifics>

<deferred>
## Deferred Ideas
- UI Stage Clear notification polish (Deferred to Phase 3).
- UI "Score to Next" readout (Deferred to Phase 3).

</deferred>

---

*Phase: 02-dynamic-spawning-boost*
*Context gathered: 2026-04-17 via discussion*
