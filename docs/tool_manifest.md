# The Last Primordial — Tool & Asset Pipeline Manifest

> **Version:** 0.1.0 | **Last Updated:** 2026-07-10

---

## 1. Core Development Tools

| Purpose | Tool | Version | Notes |
|---------|------|---------|-------|
| **Game Engine** | Bevy (Rust) | 0.19.0 | ECS architecture, 2D rendering |
| **Language** | Rust | 1.96+ | Memory-safe, high performance |
| **Build System** | Cargo | 1.96+ | Rust's package manager |
| **IDE** | VS Code + rust-analyzer | Latest | Primary development environment |

---

## 2. Art Pipeline

| Purpose | Tool | Format | Notes |
|---------|------|--------|-------|
| **Pixel Art & Animation** | Aseprite | PNG + JSON | Sprite sheets, animation tags, export automation |
| **Level Design** | LDtk (Level Designer Toolkit) | .ldtk | Tile-based level editor, entity placement |
| **Concept Art** | Krita / Photoshop | PNG | Hand-drawn elements, Core art |
| **Reference Boards** | Pinterest / PureRef | — | Mood boards, visual references |

### Aseprite Export Settings
- **Format:** PNG sprite sheet (horizontal strip)
- **Metadata:** JSON (for frame data, tags, slices)
- **Naming:** `<character>_<animation>.png` (e.g., `kael_idle.png`)
- **Tags:** One tag per animation state (idle, walk, run, etc.)

### LDtk Configuration
- **Tile Size:** 16×16 px
- **Layers:** Collision, Entity, Decoration, Background
- **Tileset:** One per realm + village
- **Integration:** bevy_ecs_ldtk crate (v0.15)

---

## 3. Audio Pipeline

| Purpose | Tool | Format | Notes |
|---------|------|--------|-------|
| **DAW (Primary)** | Reaper | — | Orchestral composition, mixing |
| **DAW (Alternative)** | LMMS | — | Free, cross-platform |
| **SFX Generation** | jsfxr / sfxr | WAV | Pixel-style game sounds |
| **Audio Library** | Spitfire LABS | — | Free orchestral samples |
| **Audio Library** | BBC Symphony Discover | — | Free full orchestra |
| **Audio Editing** | Audacity | — | Trimming, normalization |
| **Integration** | bevy_kira_audio | OGG/WAV | Bevy audio plugin |

### Audio Export Settings
- **Music:** OGG Vorbis, 44.1kHz, stereo, ~192kbps
- **SFX:** WAV, 44.1kHz, mono, 16-bit
- **Ambient:** OGG Vorbis, looping, stereo

### Audio File Organization
```
assets/audio/
├── music/
│   ├── village_day.ogg
│   ├── realm_time_explore.ogg
│   └── boss_spirit_time.ogg
├── sfx/
│   ├── sword_swing_01.wav
│   ├── footstep_grass_01.wav
│   └── ui_select.wav
└── ambient/
    ├── forest_birds.ogg
    ├── village_marketplace.ogg
    └── realm_time_clocks.ogg
```

---

## 4. Data & Scripting

| Purpose | Tool/Format | Notes |
|---------|-------------|-------|
| **Dialogue Data** | RON (.ron files) | Rusty Object Notation, native Rust parsing |
| **Cutscene Scripts** | RON (.ron files) | Sequential step-based scripting |
| **Save Data** | JSON / RON | Serialized game state |
| **Configuration** | RON | Game settings, constants |
| **Serialization** | serde + serde_json + ron | Rust serialization framework |

---

## 5. Physics & Gameplay

| Purpose | Tool | Notes |
|---------|------|-------|
| **2D Physics** | bevy_rapier2d (v0.34) | Collision, hitbox/hurtbox, raycasts |
| **Tilemap Rendering** | bevy_ecs_tilemap | Via bevy_ecs_ldtk |
| **UI Framework** | bevy_ui | Built-in Bevy UI system |

---

## 6. Version Control & CI/CD

| Purpose | Tool | Notes |
|---------|------|-------|
| **Version Control** | Git | Local + remote |
| **Remote Repository** | GitHub | Branch protection, PR reviews |
| **CI Pipeline** | GitHub Actions | cargo check, clippy, test |
| **Branch Strategy** | Git Flow | main, develop, feature/* |

### Branch Rules
- `main` — Production-ready, tagged releases only
- `develop` — Integration branch, all features merge here
- `feature/<name>` — Individual feature branches
- PRs required for merges to `main` and `develop`

---

## 7. Performance & Debugging

| Purpose | Tool | Notes |
|---------|------|-------|
| **Profiling** | Tracy | Frame-level performance analysis |
| **Debug Rendering** | bevy_rapier2d debug-render | Collision shape visualization |
| **Logging** | bevy::log (tracing) | Built-in structured logging |
| **Hot Reloading** | Bevy asset hot-reload | Live asset updates during development |

---

## 8. Cutscene System

The cutscene system is **custom-built in Bevy** (not a third-party tool).

**Approach:** In-engine scripted sequences using RON files.
- No pre-rendered video — keeps file size small and visual consistency
- Player input disabled during cutscenes (with skip support)
- Supports: camera movement, entity spawning, dialogue, audio cues, shader effects, fades

**Types:**
1. Scripted Gameplay (camera + entities on script)
2. Illustrated Stills (full-screen art + text)
3. Dialogue Cutscene (portraits + text boxes)
4. Memory Sequence (screen effects + replay)
5. Environmental Transition (camera pan + title card)

---

*This manifest will be updated as new tools are adopted or versions change.*
