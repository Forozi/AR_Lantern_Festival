# Roadmap: AR Lantern Festival

## Overview

Transitioning the existing 30-second session into a comprehensive 3-stage gameplay cycle with dynamic difficulty scaling, score-based progression, and a "Boost" catch-up mechanic for struggling players.

## Phases

- [x] **Phase 1: Multi-Stage Lifecycle** - Core framework for 3 stages, timers, and score-based skips.
- [x] **Phase 2: Dynamic Spawning & Boost** - Progressive speed ramping and the "Boost" transformation system.
- [ ] **Phase 3: UI & Feedback Systems** - Stage clear indicators and visual polish for game transitions.

## Phase Details

### Phase 1: Multi-Stage Lifecycle
**Goal**: Implement the structural framework for a 3-stage game session and score-based advancement.
**Depends on**: Nothing (Existing code serves as foundation)
**Requirements**: [CYCLE-01, CYCLE-02, CYCLE-03, MECH-04]
**Success Criteria**:
  1. Game runs for a maximum of 90 seconds (3 stages of 30s).
  2. Reaching a specific score (e.g., 50 per stage) immediately triggers the next stage.
  3. Lantern speed increases noticeably when transitioning from Stage 1 to Stage 2.
**Plans**: 2 plans

Plans:
- [ ] 01-01: Update `GameManager` for Stage-based lifecycle and score thresholds.
- [ ] 01-02: Implement `SpawnerSystem` difficulty scaling (speed multipliers).

### Phase 2: Dynamic Spawning & Boost
**Goal**: Integrate the "Boost" lantern and the global transformation logic.
**Depends on**: Phase 1
**Requirements**: [MECH-01, MECH-02, MECH-03, MECH-05]
**Success Criteria**:
  1. Boost lantern spawns exactly at the 20s mark if the score threshold hasn't been met.
  2. Hitting the Boost lantern transforms all active lanterns into "Boost" types.
  3. Boost hits award 10 points and ignore height thresholds for a limited time.
**Plans**: 2 plans

Plans:
- [ ] 02-01: Add Boost Lantern type and transformation logic to `LanternBehaviour`.
- [ ] 02-02: Implement the 20s catch-up spawn logic in `SpawnerSystem`.

### Phase 3: UI & Feedback Systems
**Goal**: Polish the player experience with clear visual cues for stage transitions.
**Depends on**: Phase 2
**Requirements**: [UI-01, UI-02, UI-03]
**Success Criteria**:
  1. "Stage 1 Cleared" / "Stage 2 Cleared" UI appears prominently.
  2. UI displays current stage number and "Score to Next" readout.
  3. Visual effect (e.g., color shift or flash) occurs when Boost State is active.
**Plans**: 1 plan

Plans:
- [ ] 03-01: Build and integrate the Stage Feedback UI components.

## Progress

**Execution Order:**
Phases execute in numeric order: 1 → 2 → 3

| Phase | Plans Complete | Status | Completed |
|-------|----------------|--------|-----------|
| 1. Multi-Stage Lifecycle | 2/2 | Complete | 2026-04-17 |
| 2. Dynamic Spawning & Boost | 2/2 | Complete | 2026-04-17 |
| 3. UI & Feedback Systems | 0/1 | Not started | - |

---
*Roadmap defined: 2026-04-17*
*Last updated: 2026-04-17 after initial definition*
