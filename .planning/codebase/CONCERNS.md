# Technical Concerns

**Analysis Date:** 2026-04-17

## Technical Debt

### 1. Global State Coupling
The `GameManager` singleton is accessed frequently by multiple systems. While convenient, this creates a central point of failure and makes unit testing individual components difficult.

### 2. Static Event Cleanup
`LanternBehaviour` uses static events. If those events aren't properly unsubscribed from (especially in cases of scene reloads or object destruction not covered by standard lifecycle), it could lead to memory leaks or "ghost" logic executions.

## Known Risks & Fragile Areas

### 1. AR Plane Reliability
The `ARPlacementManager` relies on `TrackableType.PlaneWithinPolygon`. In environments with low detail or reflective floors, plane detection may fail or be inconsistent, preventing the game from starting.

### 2. Object Life Management
While object pooling is implemented, the `ClearAllLanterns` method in `SpawnerSystem.cs` manually releases lanterns to the pool. If any external reference holds onto a lantern after it is released, it could cause unexpected behavior when that lantern is reused.

## Performance Concerns

### 1. AR Foundation Overheads
Running both Plane Detection and high-frequency spawning can be intensive for older mobile devices. AR Plane detection is disabled after placement, which is a good mitigation.

### 2. Draw Calls
Frequent spawning of lanterns with emissive materials and flickering logic may increase draw calls significantly if URP batching (SRP Batcher) is not configured correctly.

---

*Concerns analysis: 2026-04-17*
