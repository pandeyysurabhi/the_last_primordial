# The Last Primordial — Audio Direction Document

> **Version:** 0.1.0 | **Last Updated:** 2026-07-10

---

## 1. Audio Philosophy

The Last Primordial uses audio as an emotional narrative tool, not just background decoration. Each realm has its own sonic identity. Music evolves with the story, becoming more complex and layered as Kael's understanding deepens.

**Core Principles:**
- Music should tell the story even without dialogue
- Silence is as powerful as sound (Realm of Death)
- Each realm must be instantly recognizable by its audio alone
- Audio transitions should feel seamless — never jarring

---

## 2. Technical Specifications

| Property | Specification |
|----------|--------------|
| **Music Format** | OGG Vorbis, 44.1kHz, stereo |
| **SFX Format** | WAV, 44.1kHz, mono |
| **Ambient Format** | OGG Vorbis, looping |
| **Audio Engine** | bevy_kira_audio |
| **Channels** | Music, SFX, Ambient, Dialogue |
| **Volume Control** | Per-channel, user-adjustable |
| **Crossfade Duration** | 2-3 seconds between zones |
| **Dynamic Layers** | Boss fights use layered system |

---

## 3. Music Track List

| # | Track Name | Scene | Mood | Priority |
|---|-----------|-------|------|----------|
| 1 | **Main Theme** | Title screen | Epic, mysterious | HIGH |
| 2 | **Kael's Nightmare** | Prologue nightmare | Distorted, fearful | HIGH |
| 3 | **Village of Memories** | Village hub (day) | Warm, nostalgic | HIGH |
| 4 | **Village Night** | Village hub (night) | Peaceful, reflective | MEDIUM |
| 5 | **Departure** | Leaving village | Bittersweet, determined | HIGH |
| 6 | **Echoes of Time** | Realm of Time (explore) | Ethereal, clockwork-like | HIGH |
| 7 | **Spirit of Time** | Boss: Time | Intense, rhythmic | HIGH |
| 8 | **Infinite Halls** | Realm of Space (explore) | Wonder, vertigo | HIGH |
| 9 | **Spirit of Space** | Boss: Space | Vast, geometric | HIGH |
| 10 | **Garden of Forever** | Realm of Life (explore) | Lush, overwhelming | HIGH |
| 11 | **Spirit of Life** | Boss: Life | Beautiful yet suffocating | HIGH |
| 12 | **White Silence** | Realm of Death (explore) | Peaceful, mournful | HIGH |
| 13 | **Spirit of Death** | Boss: Death | Gentle yet draining | HIGH |
| 14 | **Entropy's Dance** | Realm of Chaos (explore) | Unpredictable, playful | HIGH |
| 15 | **Spirit of Chaos** | Boss: Chaos | Glitchy, ever-changing | HIGH |
| 16 | **The First Light** | Realm of Creation (explore) | Hopeful, expansive | HIGH |
| 17 | **Spirit of Creation** | Boss: Creation | Overwhelming beauty | HIGH |
| 18 | **Dust and Flowers** | Realm of Destruction (explore) | Somber, resilient | HIGH |
| 19 | **Spirit of Destruction** | Boss: Destruction | Heavy, final | HIGH |
| 20 | **Heart of Reality** | The Core | Sacred, transcendent | HIGH |
| 21 | **The Final Choice** | Three endings | Emotionally open | HIGH |
| 22 | **Epilogue** | Post-credits | Quiet hope | MEDIUM |

---

## 4. SFX Categories

### Combat SFX
| Sound | Format | Notes |
|-------|--------|-------|
| Sword swing (light) | WAV | Quick, sharp |
| Sword swing (heavy) | WAV | Heavier, wind-up whoosh |
| Hit impact (flesh) | WAV | Dull thud |
| Hit impact (metal) | WAV | Clang/ring |
| Parry success | WAV | Bright metallic ring |
| Dodge whoosh | WAV | Air displacement |
| Player hurt | WAV | Sharp breath/grunt |
| Enemy hurt | WAV | Per-type variations |
| Enemy death | WAV | Per-type dissolve |
| Boss phase transition | WAV | Dramatic, realm-specific |

### UI SFX
| Sound | Format | Notes |
|-------|--------|-------|
| Menu navigate | WAV | Soft click |
| Menu select | WAV | Confirmation tone |
| Menu cancel | WAV | Soft back tone |
| Dialogue advance | WAV | Page turn or soft blip |
| Text typewriter | WAV | Per-character blip |
| Save game | WAV | Gentle chime |
| Item acquire | WAV | Discovery tone |

### Environment SFX
| Sound | Format | Notes |
|-------|--------|-------|
| Footsteps (grass) | WAV | Soft rustle |
| Footsteps (stone) | WAV | Hard tap |
| Footsteps (wood) | WAV | Creaky hollow |
| Door open/close | WAV | Material-dependent |
| Water splash | WAV | Interaction |
| Lever/switch | WAV | Mechanical click |
| Chest open | WAV | Creak + discovery chime |

### Black Sword SFX
| Sound | Format | Notes |
|-------|--------|-------|
| Sword hum (idle) | OGG | Constant subtle vibration |
| Voice activation | WAV | Sword glows, voices emerge |
| Memory flash | WAV | Bright, sharp, echoing |
| Wielder disagreement | WAV | Overlapping whispers |

---

## 5. Realm-Specific Ambience

| Realm | Ambient Sounds | Mood |
|-------|---------------|------|
| **Village** | Birdsong, wind, distant chatter, market sounds | Safe, nostalgic |
| **Time** | Ticking clocks, time distortion whooshes, reversed audio | Unsettling beauty |
| **Space** | Spatial echoes, gravitational hums, distant star tones | Cosmic wonder |
| **Life** | Dense insects, rustling leaves, water, layered birdsong | Overwhelming vitality |
| **Death** | Near-silence, distant wind, soft bell chimes | Peaceful emptiness |
| **Chaos** | Glitchy audio, random instrument bursts, overlapping fragments | Disorienting fun |
| **Creation** | Building sounds, crystalline growth, energetic hums | Bright expansion |
| **Destruction** | Crumbling stone, distant thunder, wind through ruins | Somber weight |
| **The Core** | Deep resonance, heartbeat-like pulse, crystalline tones | Sacred |

---

## 6. Dynamic Audio System

### Boss Fight Layering
Boss music uses a multi-layer system where instrument layers are added/removed based on boss phase:
- **Phase 1:** Base rhythm + melody
- **Phase 2:** Add percussion/drums
- **Phase 3:** Add choir/strings
- **Phase 4 (if applicable):** Full orchestra, maximum intensity

### Story Moment Mixing
- During dialogue: Fade music to 30% volume
- During memory flashes: All audio fades except heartbeat
- During Primordial meetings: Custom musical sting → ambient silence

### Zone Transitions
- Crossfade music over 2-3 seconds when entering new zones
- Ambient sounds blend gradually (1-2 second overlap)
- Boss arena entry: Music cuts, 1 second silence, then boss theme

---

## 7. Orchestral Sample Libraries (Recommended)

| Library | Use | Cost |
|---------|-----|------|
| Spitfire LABS | Ambient textures, pads | Free |
| BBC Symphony Discover | Full orchestral | Free |
| Spitfire Audio Solo Strings | Emotional melodies | Paid |
| ProjectSAM Free Orchestra | Brass and woodwinds | Free |
| Pianobook | Unique piano/keys | Free |

### SFX Generation Tools
| Tool | Use |
|------|-----|
| jsfxr / sfxr | Pixel-style combat and UI sounds |
| Freesound.org | Nature ambience, environmental sounds |
| Custom recording | Unique foley for special moments |

---

*Detailed per-track composition notes to be added during production.*
