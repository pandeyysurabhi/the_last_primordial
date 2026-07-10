# The Last Primordial — Tool & Asset Pipeline Manifest

> **Version:** 0.2.0 | **Last Updated:** 2026-07-10 | **Engine:** Godot 4.3 (.NET/C#)

---

## 1. Core Development Tools

| Purpose | Tool | Version | Notes |
|---------|------|---------|-------|
| **Game Engine** | Godot | 4.3.stable.mono | 2D rendering, physics, audio, UI |
| **Language** | C# | .NET 8.0 | Via Godot.NET.Sdk 4.3.0 |
| **Build System** | dotnet CLI | 9.0+ | `dotnet build`, `dotnet test` |
| **IDE** | VS Code + C# Dev Kit | Latest | Primary development environment |
| **IDE (Alternative)** | JetBrains Rider | Latest | Full Godot C# integration |

---

## 2. Art Pipeline

| Purpose | Tool | Format | Notes |
|---------|------|--------|-------|
| **Pixel Art & Animation** | Aseprite | PNG + JSON | Sprite sheets, animation tags, export automation |
| **Level Design** | Godot TileMap Editor | Built-in | Tile-based level design, collision layers |
| **Level Design (Optional)** | LDtk (Level Designer Toolkit) | .ldtk | Via Godot LDtk plugin if needed |
| **Concept Art** | Krita / Photoshop | PNG | Hand-drawn elements, Core art |
| **Reference Boards** | Pinterest / PureRef | — | Mood boards, visual references |

### Aseprite Export Settings
- **Format:** PNG sprite sheet (horizontal strip)
- **Metadata:** JSON (for frame data, tags, slices)
- **Naming:** `<character>_<animation>.png` (e.g., `kael_idle.png`)
- **Tags:** One tag per animation state (idle, walk, run, etc.)
- **Import:** Godot auto-imports PNGs → configure as SpriteFrames resource

### Godot TileMap Configuration
- **Tile Size:** 16×16 px
- **Layers:** Collision, Entity, Decoration, Background
- **Tileset:** One per realm + village
- **Physics Layers:** Ground, Walls, Platforms (one-way), Hazards

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
| **Integration** | Godot AudioStreamPlayer | OGG/WAV | Built-in audio system |

### Audio Export Settings
- **Music:** OGG Vorbis, 44.1kHz, stereo, ~192kbps
- **SFX:** WAV, 44.1kHz, mono, 16-bit
- **Ambient:** OGG Vorbis, looping, stereo

### Audio Bus Layout (Godot)
```
Master
├── Music      (for background music, crossfade via Tween)
├── SFX        (for combat, UI, interactions)
├── Ambient    (for environmental loops)
└── Dialogue   (for voice/text blips, sword voices)
```

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
| **Dialogue Data** | JSON (.json files) | Parsed by System.Text.Json in C# |
| **Cutscene Scripts** | JSON (.json files) | Sequential step-based scripting |
| **Save Data** | JSON | Serialized game state via System.Text.Json |
| **Configuration** | Godot Resources (.tres) | Game settings, constants |
| **Serialization** | System.Text.Json | .NET JSON serialization framework |

---

## 5. Physics & Gameplay

| Purpose | Tool | Notes |
|---------|------|-------|
| **2D Physics** | Godot built-in | CharacterBody2D, RigidBody2D, Area2D, RayCast2D |
| **Collision System** | Godot physics layers | Hitbox/hurtbox via Area2D with collision layers |
| **Tilemap Rendering** | Godot TileMap | Built-in tile rendering and collision |
| **UI Framework** | Godot Control nodes | CanvasLayer + Control tree (VBoxContainer, HBoxContainer, etc.) |
| **Animation** | Godot AnimationPlayer | Sprite animations, tweens, cutscene timing |
| **Shaders** | Godot Shader Language | Chromatic aberration, vignette, realm effects |

---

## 6. Version Control & CI/CD

| Purpose | Tool | Notes |
|---------|------|-------|
| **Version Control** | Git | Local + remote |
| **Remote Repository** | GitHub | Branch protection, PR reviews |
| **CI Pipeline** | GitHub Actions | `dotnet build`, `dotnet test` |
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
| **Profiling** | Godot built-in profiler | Frame-level performance analysis (Debugger → Profiler) |
| **Debug Rendering** | Godot debug draw | Collision shape visualization (Debug → Visible Collision Shapes) |
| **Logging** | GD.Print / GD.PushWarning | Built-in logging, visible in Output panel |
| **Hot Reloading** | Godot asset hot-reload | Live asset updates during development |
| **Remote Inspector** | Godot Scene Tree (Remote) | Live node inspection during play |

---

## 8. Cutscene System

The cutscene system is **custom-built in Godot/C#** (not a third-party tool).

**Approach:** In-engine scripted sequences using JSON data files.
- No pre-rendered video — keeps file size small and visual consistency
- Player input disabled during cutscenes (with skip support)
- Supports: camera movement (Tween), entity spawning, dialogue, audio cues, shader effects, fades

**Types:**
1. Scripted Gameplay (camera + entities on script)
2. Illustrated Stills (full-screen art + text via TextureRect)
3. Dialogue Cutscene (portraits + text boxes via Control nodes)
4. Memory Sequence (screen effects via ShaderMaterial + replay)
5. Environmental Transition (camera pan via Tween + title card)

---

## 9. Project Structure

```
TheLastPrimordial/
├── project.godot              # Godot project file
├── TheLastPrimordial.csproj   # C# project
├── TheLastPrimordial.sln      # .NET solution
├── main.tscn                  # Root scene
├── src/                       # C# source code
│   ├── Main.cs
│   ├── Autoloads/             # Singletons (GameManager, AudioManager, etc.)
│   ├── Core/                  # Core systems (StateMachine, Events)
│   ├── Player/                # Player scripts
│   ├── Enemies/               # Enemy AI
│   ├── NPCs/                  # NPC interactions
│   ├── UI/                    # UI controllers
│   ├── Dialogue/              # Dialogue system
│   ├── Combat/                # Combat system
│   ├── Camera/                # Camera system
│   ├── SaveLoad/              # Save/Load system
│   └── Cutscenes/             # Cutscene system
├── scenes/                    # Godot scene files (.tscn)
│   ├── levels/
│   ├── characters/
│   ├── ui/
│   ├── effects/
│   └── cutscenes/
├── assets/                    # Game assets
│   ├── sprites/
│   ├── audio/
│   ├── dialogues/             # JSON dialogue data
│   ├── cutscenes/             # JSON cutscene data
│   ├── fonts/
│   └── shaders/               # Custom .gdshader files
├── resources/                 # Godot resources (.tres)
│   ├── themes/
│   ├── materials/
│   └── data/
└── docs/                      # Design documents
```

---

*This manifest will be updated as new tools are adopted or versions change.*
