# AR Lantern Festival

## What This Is

An immersive Augmented Reality (AR) game where players interact with lanterns in their physical space. It features a session-based gameplay loop where players shoot lanterns to score points while managing risks and chasing high scores.

## Core Value

Engaging and responsive AR interaction that blends digital lanterns seamlessly with the physical environment.

## Requirements

### Validated

- ✓ AR Plane Detection & Placement — existing
- ✓ Basic Lantern Spawning & Pooling — existing
- ✓ Two Lantern Types (Blessed/Cursed) — existing
- ✓ Scoring & High Score Persistence — existing
- ✓ Shooting/Tap Mechanic — existing

### Active

- [ ] 3-Stage Gameplay System — 90 seconds total playtime (3x 30s)
- [ ] Score-based Stage Advancing — Skip to next stage if threshold reached
- [ ] Boost Trigger Mechanic — Catch-up lantern appearing at 20s if threshold not met
- [ ] Global "Boost State" — All lanterns transform into boost types with new patterns
- [ ] Stage-based Spawning Patterns — Progressive difficulty (speed ramps) per stage
- [ ] Stage Clear UI Indicators — Clear visual feedback on player progress

### Out of Scope

- [ ] Multi-platform AR (Web/iOS) — focusing on Android (ARCore) for now
- [ ] Complex Combo Systems — prioritizing pure speed and accuracy over combos
- [ ] Multiplayer — focusing on single-player core loop first

## Context

- **Technical Stack**: Unity, C#, AR Foundation (ARCore), URP.
- **Current State**: Functional vertical slice with basic spawning and scoring. No "stages" or "boost" logic yet.
- **Feedback**: Need for more dynamic progression and "save-save" catch-up mechanics.

## Constraints

- **Hardware**: Must run smoothly on ARCore-compatible Android devices.
- **Performance**: High-frequency spawning must maintain 60 FPS in AR.
- **AR Interaction**: Reliant on environmental lighting and plane stability for consistent placement.

## Key Decisions

| Decision | Rationale | Outcome |
|----------|-----------|---------|
| Score-based Skip | Rewards player skill and keeps the game pacing tight. | — Pending |
| 20s Boost Trigger | Provides a "last chance" for struggling players to advance. | — Pending |
| Global Boost State | Simplifies visual communication and reinforces a "power-up" feeling. | — Pending |

## Evolution

This document evolves at phase transitions and milestone boundaries.

**After each phase transition** (via `/gsd-transition`):
1. Requirements invalidated? → Move to Out of Scope with reason
2. Requirements validated? → Move to Validated with phase reference
3. New requirements emerged? → Add to Active
4. Decisions to log? → Add to Key Decisions
5. "What This Is" still accurate? → Update if drifted

**After each milestone** (via `/gsd-complete-milestone`):
1. Full review of all sections
2. Core Value check — still the right priority?
3. Audit Out of Scope — reasons still valid?
4. Update Context with current state

---
*Last updated: 2026-04-17 after initialization*
