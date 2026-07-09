# The Last Primordial — Project Brain (Context Tracker)

> **Last Updated:** 2026-07-10 | **Current Phase:** 0 (Pre-Production & Foundation)

---

## Project Status: Phase 0 COMPLETE ✅

### Completed Tasks

#### ✅ Task 0.1 — Rust + Bevy Project Setup
- Initialized Cargo project: `the_last_primordial`
- Engine: Bevy 0.19, Physics: bevy_rapier2d 0.34, Audio: bevy_kira_audio 0.26, Tilemap: bevy_ecs_ldtk 0.15
- Serialization: serde + serde_json + ron
- Dev profile optimized for Bevy performance (opt-level 1 for game, 3 for deps)
- Minimal main.rs: window "The Last Primordial", 1280x720, dark purple-black bg, 2D camera

#### ✅ Task 0.2 — Version Control
- Git repo initialized with `.gitignore` (Rust/Bevy/IDE/OS rules)
- Branch: `main` (initial), `develop` (integration)

#### ✅ Task 0.3 — Project Folder Structure
```
src/                    — Rust source code
assets/
  sprites/              — Character and object sprite sheets
  tilesets/             — LDtk tileset PNGs
  audio/
    music/              — OGG music tracks
    sfx/                — WAV sound effects
    ambient/            — OGG ambient loops
  dialogues/            — RON dialogue files
  cutscenes/            — RON cutscene scripts
  fonts/                — Game fonts
docs/                   — Design documents
project_brain/          — This context tracker
```

#### ✅ Task 0.4 — Game Design Document (GDD)
- Created `docs/GDD.md` — comprehensive living document
- Covers: game overview, full story summary, character roster, seven realm mechanics, UI/controls, game flow, tech specs
- Protagonist: Kael (internally "Christ" in lore)
- Three endings: Eternal Guardian / Age of Humanity / Broken Cycle

#### ✅ Task 0.5 — Art Direction & Style Guide
- Created `docs/art_style_guide.md`
- Tile size: 16×16, player: 32×48, bosses: 64×64 to 128×128
- Internal resolution: 320×180 scaled 4x to 1280×720
- 9 color palettes defined (Village + 7 Realms + The Core)
- Animation frame counts per character type
- VFX guidelines and parallax layer specs

#### ✅ Task 0.6 — Audio Direction
- Created `docs/audio_direction.md`
- 22 music tracks catalogued
- SFX categories: Combat (10), UI (7), Environment (7), Black Sword (4)
- 9 realm-specific ambience profiles
- Dynamic audio system: boss layering, story mixing, zone crossfading
- Sample libraries: Spitfire LABS, BBC Symphony (free tier)

#### ✅ Task 0.7 — Narrative Script
- Created `assets/dialogues/format_spec.md` — RON format specification
- Created 4 template dialogue files:
  - `nightmare_prologue.ron` — Opening sequence
  - `village_elder.ron` — Elder Maren with branching choices
  - `black_sword_discovery.ron` — Pivotal sword event with wielder voices
  - `chronar_meeting.ron` — First Primordial encounter with philosophical branching

#### ✅ Task 0.8 — Tool Selection
- Created `docs/tool_manifest.md`
- Art: Aseprite + LDtk
- Audio: Reaper/LMMS + jsfxr + Spitfire LABS
- Cutscenes: Custom Bevy system (RON-scripted)
- CI/CD: GitHub Actions (cargo check, clippy, test)
- Branch strategy: Git Flow (main, develop, feature/*)

---

## Key Technical Decisions

| Decision | Choice | Rationale |
|----------|--------|-----------|
| Engine | Bevy 0.19 (Rust) | ECS architecture, performance, cross-platform |
| Tile size | 16×16 px | Standard for pixel art, good detail level |
| Internal resolution | 320×180 | Clean 4x scale to 1280×720 |
| Dialogue format | RON | Native Rust serialization, readable |
| Audio format | OGG (music) / WAV (SFX) | Good compression / lossless quality |
| Physics | bevy_rapier2d | Official Bevy plugin, well-maintained |
| Level editor | LDtk | Best 2D level editor, native Bevy support |

---

## Next Phase: Phase 1 — Core Engine & Vertical Slice

### Upcoming Tasks (Sequential)
- 1.1 — Player Movement System (walk, run, jump, dash, ledge grab)
- 1.2 — Camera System (smooth-follow, dead zone, parallax)
- 1.3 — Combat System (light/heavy attack, dodge, parry)
- 1.4 — Enemy AI (patrol, detect, chase, attack)
- 1.5 — Tilemap & Level Loading (LDtk integration)

### Upcoming Tasks (Parallel)
- 1.6 — UI System (HUD, menus)
- 1.7 — Dialogue System (RON parsing, text box, typewriter)
- 1.8 — Save/Load System
- 1.9 — Sprite Sheet Pipeline
- 1.10 — Audio Engine Integration

---

## File Inventory

| File | Purpose | Status |
|------|---------|--------|
| `Cargo.toml` | Project manifest | ✅ |
| `src/main.rs` | Entry point | ✅ |
| `.gitignore` | Git ignore rules | ✅ |
| `docs/GDD.md` | Game Design Document | ✅ |
| `docs/art_style_guide.md` | Art direction | ✅ |
| `docs/audio_direction.md` | Audio direction | ✅ |
| `docs/tool_manifest.md` | Tool manifest | ✅ |
| `assets/dialogues/format_spec.md` | Dialogue format spec | ✅ |
| `assets/dialogues/nightmare_prologue.ron` | Nightmare opening | ✅ |
| `assets/dialogues/village_elder.ron` | Elder conversation | ✅ |
| `assets/dialogues/black_sword_discovery.ron` | Sword event | ✅ |
| `assets/dialogues/chronar_meeting.ron` | First Primordial meeting | ✅ |
| `project_brain/context.md` | This file | ✅ |

---

*Updated after each phase completion.*
