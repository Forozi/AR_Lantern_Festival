# Technology Stack

**Analysis Date:** 2026-04-17

## Languages

**Primary:**
- C# 9.0+ - All application logic, Unity scripts, and system management.

## Runtime

**Environment:**
- Unity 2022.3/2023.x - Game engine and development environment.
- .NET Standard 2.1 - Scripting backend.

**Package Manager:**
- Unity Package Manager (UPM) - Handles dependencies and engine modules.
- manifest.json: `Packages/manifest.json` present.

## Frameworks

**Core:**
- Unity Engine - Core game engine.
- AR Foundation 6.2.1 - Multi-platform AR abstraction layer.
- Universal Render Pipeline (URP) 17.2.0 - High-performance graphics renderer.

**AR Subsystems:**
- ARCore 6.2.1 - Android-specific AR tracking and features.

**UI:**
- Unity UI (uGUI) 2.0.0 - Core UI system.
- TextMesh Pro - Advanced typography and text rendering.

**Input/Interaction:**
- Input System 1.14.2 - Action-based input handling.
- XR Interaction Toolkit 3.2.2 - VR/AR interaction patterns.

## Key Dependencies

**Critical:**
- `com.unity.xr.arfoundation` 6.2.1 - Enables plane detection, raycasting, and anchor management.
- `com.unity.xr.arcore` 6.2.1 - Provides the underlying AR tracking for Android devices.
- `com.unity.render-pipelines.universal` 17.2.0 - Used for visual effects like lantern glow and flickering.
- `com.unity.xr.interaction.toolkit` 3.2.2 - Handles user interaction with spawned lanterns.

**Infrastructure:**
- `UnityEngine.Pool` - Used for lantern object pooling in `SpawnerSystem.cs`.

## Configuration

**Build:**
- `ProjectSettings/` - Contains platform-specific settings, AR configuration, and Quality settings.
- `Packages/manifest.json` - Defines all package dependencies.

## Platform Requirements

**Development:**
- Windows/macOS with Unity Editor installed.
- Android SDK/NDK for Android builds.

**Production:**
- ARCore-compatible Android devices.
- Minimum Android API level required by ARCore 6.2.1.

---

*Stack analysis: 2026-04-17*
*Update after major dependency changes*
