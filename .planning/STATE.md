# Project Memory

## Project Reference

See: .planning/PROJECT.md (updated 2026-04-17)

**Core value:** Engaging and responsive AR interaction that blends digital lanterns seamlessly with the physical environment.
**Current focus:** Phase 1: Multi-Stage Lifecycle

## Active Context

### Phase 0: Initialization (Current)
- **Status**: Completed codebase mapping and project initialization.
- **Outcome**: Established roadmap for 3-stage gameplay and boost mechanics.
- **Git State**: `d62780b` (docs: initialize project)

### Key Learnings (from Mapping)
- Unity AR Foundation structure is standard.
- Object pooling is already present for lanterns.
- Singleton `GameManager` will be the primary point for stage logic.

## Decision Log

### 2026-04-17: Stage Transition Logic
Decision to use score thresholds for immediate progression.
Rationale: Rewards skill and prevents the game from feeling too slow for expert players.

### 2026-04-17: Boost Trigger Condition
Decision to trigger boost lantern at exactly 20s if threshold not met.
Rationale: Clear "last chance" window for catch-up.

## Next Steps
1. Run `/gsd-plan-phase 1` to start implementing the stage lifecycle.

---
*Last updated: 2026-04-17 after initialization*
