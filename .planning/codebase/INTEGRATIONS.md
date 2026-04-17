# External Integrations

**Analysis Date:** 2026-04-17

## AR Services

**ARCore (Google):**
- **Purpose:** Plane detection, motion tracking, and environmental understanding.
- **Integration:** Managed via `com.unity.xr.arcore` and `com.unity.xr.arfoundation`.
- **Primary Use:** Used in `ARPlacementManager.cs` to detect floor planes and place the game center anchor.

## Graphics & Rendering

**Universal Render Pipeline (URP):**
- **Purpose:** Post-processing, lighting, and optimized rendering.
- **Integration:** Configured in `Settings/URP_Asset`.
- **Primary Use:** Provides the visual foundation for the lantern's emissive materials and flickering effects.

## Local Persistence

**PlayerPrefs:**
- **Purpose:** Local data storage for high scores.
- **Integration:** Built-in Unity API.
- **Primary Use:** Used in `GameManager.cs` to save and load the `HighScore` (`PlayerPrefs.SetInt("HighScore", HighScore)`).

---

*Integration analysis: 2026-04-17*
