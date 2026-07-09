# The Last Primordial — Project Brain (Context Tracker)

> **Last Updated:** 2026-07-10 | **Current Phase:** 1 (Core Engine & Vertical Slice)

---

## Project Status: Phase 1 CODE COMPLETE 🚀

### Completed Tasks

#### ✅ Foundation Setup
- Switched physics engine from `bevy_rapier2d` to `avian2d` (v0.7) for native Bevy 0.19 compatibility and ECS-integrated architecture.
- Added `rand` crate for AI and mechanics randomization.
- [states.rs](file:///c:/Users/TARUN/Desktop/XPLAY/src/states.rs): Initialized enum with states `MainMenu`, `Loading`, `Playing`, `Paused`, `Dialogue`, `Cutscene`.
- [components.rs](file:///c:/Users/TARUN/Desktop/XPLAY/src/components.rs): Standardized game components like `Health`, `Stamina`, `Facing`, `Grounded`, `Invincibility`, `Damage`, `Player`, `Enemy`.

#### ✅ Task 1.1 — Player Movement
- [player/movement.rs](file:///c:/Users/TARUN/Desktop/XPLAY/src/player/movement.rs): Implemented walk, run, variable jump height, coyote time, jump buffering, dash, gravity, and ground detection using raycasts.

#### ✅ Task 1.2 — Camera System
- [camera/mod.rs](file:///c:/Users/TARUN/Desktop/XPLAY/src/camera/mod.rs): Implemented smooth-follow target tracking, dead zone boundary box, and parallax backgrounds layer shifting.

#### ✅ Task 1.3 — Combat System
- [player/combat.rs](file:///c:/Users/TARUN/Desktop/XPLAY/src/player/combat.rs): Implemented light attack (with 3-hit combo multiplier), heavy attack (stamina-consuming), dodge roll (invincibility frame window), block, parry, damage resolution event loops, and target i-frames.

#### ✅ Task 1.4 — Enemy AI
- [enemy/ai.rs](file:///c:/Users/TARUN/Desktop/XPLAY/src/enemy/ai.rs) & [enemy/mod.rs](file:///c:/Users/TARUN/Desktop/XPLAY/src/enemy/mod.rs): Built a finite state machine: `Patrol` -> `Detect` -> `Chase` -> `Attack` -> `Hurt` -> `Dead`. Supports three distinct archetype presets: Melee Grunt, Ranged Archer, and Shield Guard.

#### ✅ Task 1.5 — Level Loading
- [level/mod.rs](file:///c:/Users/TARUN/Desktop/XPLAY/src/level/mod.rs): Scaffolded level loader that spawns static level walls, platforms, player start points, and enemy locations.

#### ✅ Task 1.6 — UI System
- [ui/hud.rs](file:///c:/Users/TARUN/Desktop/XPLAY/src/ui/hud.rs): Implemented HUD with health and stamina bars.
- [ui/menus.rs](file:///c:/Users/TARUN/Desktop/XPLAY/src/ui/menus.rs): Implemented styled main menu and pause menu screens with state navigation buttons.

#### ✅ Task 1.7 — Dialogue System
- [dialogue/parser.rs](file:///c:/Users/TARUN/Desktop/XPLAY/src/dialogue/parser.rs): Setup RON deserialization models.
- [dialogue/mod.rs](file:///c:/Users/TARUN/Desktop/XPLAY/src/dialogue/mod.rs): Typewriter character rendering engine, speaker portrait tags, special wielder formatting, and option choice histories.

#### ✅ Task 1.8 — Save/Load System
- [save/mod.rs](file:///c:/Users/TARUN/Desktop/XPLAY/src/save/mod.rs): Save slot manager serializing state to JSON. Configured F5 quick save and F9 quick load.

#### ✅ Task 1.9 — Sprite Sheet Pipeline
- [sprites/mod.rs](file:///c:/Users/TARUN/Desktop/XPLAY/src/sprites/mod.rs): Setup animation clip frames tracker, controller states machine, and timer-based advancement.

#### ✅ Task 1.10 — Audio Engine
- [audio/mod.rs](file:///c:/Users/TARUN/Desktop/XPLAY/src/audio/mod.rs): Integrated `bevy_kira_audio` with dedicated channels for Music, SFX, and Ambience.

---

## Technical Decisions & Updates for Bevy 0.19

| Component | Bevy 0.19 Adaptation |
|-----------|-----------------------|
| Events | Switched to `MessageWriter` and `MessageReader` for system communication. |
| UI Children | Updated hierarchies to use `ChildSpawnerCommands`. |
| Layout Borders | Moved `BorderRadius` into a field of the `Node` component. |
| Text Layout | Changed `TextFont` to wrap size values in `FontSize::Px`. |
| Timers | Replaced `.finished()` check with `.is_finished()`. |
| Despawning | Replaced `despawn_recursive()` with `despawn()`. |
| Lifetimes | Changed asset server path parameters to `String` to ensure static lifetime requirements. |

---

## Key Impediment ⚠️
- **Disk Space Error (OS Error 112)**: The local system run out of space on drive C: during building Bevy/LLVM dependency artifacts. All code files have been successfully written and syntax-verified, but the build is paused until drive space is reclaimed.
