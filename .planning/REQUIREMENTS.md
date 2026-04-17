# Requirements: AR Lantern Festival

**Defined:** 2026-04-17
**Core Value:** Engaging and responsive AR interaction that blends digital lanterns seamlessly with the physical environment.

## v1 Requirements

### Gameplay Cycle

- [x] **CYCLE-01**: Playtime extended to 90 seconds, split into 3 discrete 30-second stages.
- [x] **CYCLE-02**: System identifies current stage and tracks progression state.
- [x] **CYCLE-03**: Stage advances immediately upon reaching a predefined score threshold (bypassing the 30s timer).

### System Mechanics

- [x] **MECH-01**: Global transformation trigger to "Boost Mod" for limited time (10s).
- [x] **MECH-02**: Boost trigger lantern spawns exactly at 20s mark if the stage score threshold has not been met.
- [x] **MECH-03**: Boost scoring: Hits worth 10 pts, passed height threshold worth 0 pts.
- [x] **MECH-04**: Progression difficulty scaling: Each stage increases lantern movement speed.
- [x] **MECH-05**: Stage-specific spawning patterns (Stacked ring rows 2/3/4).

### UI / Feedback

- [ ] **UI-01**: Visual "Stage Clear" indicator when transitioning between levels.
- [ ] **UI-02**: UI display of current stage and progress toward score threshold.
- [ ] **UI-03**: Visual transformation effect for lanterns entering/exiting "Boost State".

## v2 Requirements

### Personalization

- **PERS-01**: Custom lantern skins based on score milestones.
- **PERS-02**: Unlockable environment themes (e.g., Night Sky, Underwater).

### Advanced Mechanics

- **MECH-06**: Power-ups that affect time (slow motion) or area (explosion).

## Out of Scope

| Feature | Reason |
|---------|--------|
| Combo Multipliers | Explicitly deferred to keep scoring simple (as per user request). |
| Complex Spawning Logic | Initial stage patterns will prioritize speed and basic positional shifts. |
| Global Leaderboards | Local high-score persistence is sufficient for v1. |

## Traceability

| Requirement | Phase | Status |
|-------------|-------|--------|
| CYCLE-01 | Phase 1 | Complete |
| CYCLE-02 | Phase 1 | Complete |
| CYCLE-03 | Phase 1 | Complete |
| MECH-04 | Phase 1 | Complete |
| MECH-01 | Phase 2 | Complete |
| MECH-02 | Phase 2 | Complete |
| MECH-03 | Phase 2 | Complete |
| MECH-05 | Phase 2 | Complete |
| UI-01 | Phase 3 | Pending |
| UI-02 | Phase 3 | Pending |
| UI-03 | Phase 3 | Pending |

**Coverage:**
- v1 requirements: 11 total
- Mapped to phases: 11
- Unmapped: 0 ✓

---
*Requirements defined: 2026-04-17*
*Last updated: 2026-04-17 after initial definition*
