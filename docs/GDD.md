# The Last Primordial — Game Design Document (Living GDD)

> **Version:** 0.1.0 | **Last Updated:** 2026-07-10 | **Status:** Phase 0 - Pre-Production

---

## 1. Game Overview

| Field | Detail |
|-------|--------|
| **Title** | The Last Primordial |
| **Genre** | 2D Action-Adventure / Narrative Puzzle |
| **Engine** | Bevy (Rust) |
| **Art Style** | Pixel Art + Hand-drawn elements |
| **Audio** | Orchestral + Pixel-style SFX + Realm-specific ambience |
| **Platforms** | PC (Steam) → Nintendo Switch → Mac / Linux |
| **Price** | $19.99 – $24.99 (premium buy-to-play) |
| **Team** | Surabhi Pandey, Jemin Morabiya, Tarun Ahuja |
| **Protagonist** | Kael (internally "Christ" in lore bible) |
| **Key Mechanic** | Seven Realms, each with unique gameplay rules |
| **Endings** | 3 (Eternal Guardian / Age of Humanity / Broken Cycle) |

---

## 2. Story Summary

### The Core
The Core is the foundation of reality — not a god, not conscious, simply the engine of existence. Seven Fundamental Laws emerged from it: Creation, Destruction, Time, Space, Life, Death, and Chaos. These laws are interdependent; altering one disrupts all others.

### Humanity's Rise and Fall
Seven kingdoms discovered the Core and used it to "improve" reality — curing diseases, controlling nature, seeking to eliminate death. Every modification, though well-intentioned, destabilized reality's balance. Reality began collapsing.

### The Seven Primordials
Seven ordinary humans volunteered to merge with the Fundamental Laws, stabilizing reality at the cost of their humanity. They became:
- **Chronar** — Guardian of Time (experiences all moments simultaneously)
- **Vastara** — Guardian of Space (constantly repairing reality's structure)
- **Elaria** — Guardian of Life (feels every living being's joy and suffering)
- **Morvex** — Guardian of Death (witnesses every death in existence)
- **Xael** — Guardian of Chaos (maintains unpredictability and free will)
- **Aetherion** — Guardian of Creation (ensures creation doesn't overwhelm)
- **Nihros** — Guardian of Destruction (ensures necessary endings occur)

### The Eighth Law — Choice
The Seven discovered an eighth law that cannot be permanently guarded: **Choice**. It manifests through "Choice Wielders" who inherit the Black Sword — a weapon containing the memories and philosophies of every previous wielder.

### Kael's Origin
The Last Choice Wielder (Kael's father) and an immortal woman (saved by Morvex millennia ago) broke the oldest law by having a child — Kael — who was *born* with the essence of Choice rather than *selected*. The Seven executed his parents (who accepted their fate willingly) but spared Kael, choosing to observe him.

### Kael's Journey
Kael grows up with fragmented nightmare memories. When the Black Sword finds him, he learns the Seven killed his parents. Believing them to be villains, he sets out for revenge. Through seven realms, each teaching a philosophical lesson, his certainty crumbles until he reaches the Core and faces the truth.

### Three Endings
1. **Eternal Guardian** — Kael becomes the eighth Primordial, binding Choice permanently
2. **Age of Humanity** — Kael frees the Seven, returning reality to humanity's hands
3. **Broken Cycle** — Kael destroys the Core, ending the cycle entirely

---

## 3. Characters

### Kael (Protagonist)
- Orphan raised in a village, haunted by nightmares
- Sprite: 32×48 px | 15+ animation states | 5 emotion portraits
- Begins seeking revenge, evolves toward seeking truth
- Carries the Black Sword and his mother's pendant

### The Seven Primordials
| Name | Law | Visual Direction | Key Question |
|------|-----|-----------------|--------------|
| Chronar | Time | Exhausted despite immortality, clock motifs | "Which moment made you believe revenge could heal grief?" |
| Vastara | Space | Translucent, stars within form | "If you could place every person where they belong... would anyone still be free?" |
| Elaria | Life | Surrounded by blooming flowers, warm green/gold | "Would you save every life... even if saving one meant condemning thousands?" |
| Morvex | Death | Emotionally distant, gentle eyes, pale palette | "Do you hate me because I brought death... or because death reached them at all?" |
| Xael | Chaos | Appearance shifts, multicolor glitch effects | Two identical doors → "The destination never mattered. Only the fact that you were free to choose." |
| Aetherion | Creation | Stars and light surround him, gold/white | "If you were given the power to create a perfect world... would you erase the imperfect one?" |
| Nihros | Destruction | Covered in scars, powerful build, dark reds/grays | "Tell me... what deserves to survive?" |

### Other Characters
- **Village NPCs** (8-10): 32×48 px, 3 states, 2 emotions
- **Village Elders** (2-3): 32×48 px, 3 states, 3 emotions
- **Past Choice Wielders** (5-8 shown): Portraits only, 2 emotions each
- **Kael's Father**: Sword Voice / Memory, 32×48 px, 3 states, 3 emotions
- **Kael's Mother**: Memory scenes, 32×48 px, 2 states, 2 emotions

---

## 4. The Seven Realms — Mechanic Specs

### Realm 1: Time (Chronar)
- **Mechanics:** Time acceleration, reversal, freeze, fracture zones
- **Enemies:** Age-shifting creatures, temporal echoes
- **Boss:** Spirit of Time — arena rewinds, ghostly echoes of player's actions
- **Ambience:** Ticking clocks, time distortion whooshes, reversed audio

### Realm 2: Space (Vastara)
- **Mechanics:** Impossible geometry, portals, perspective puzzles, gravity shifts
- **Enemies:** Spatial distortion entities
- **Boss:** Spirit of Space — infinite geometric prison
- **Ambience:** Spatial echoes, gravitational hums, distant star tones

### Realm 3: Life (Elaria)
- **Mechanics:** Living environment, symbiotic creatures, ecosystem puzzles, regenerating enemies
- **Enemies:** Creatures that evolve and regenerate
- **Boss:** Spirit of Life — refuses to let anything die
- **Ambience:** Dense nature sounds, insects, rustling leaves, birdsong

### Realm 4: Death (Morvex)
- **Mechanics:** Emotional encounters, helping souls, minimal combat, memory puzzles
- **Enemies:** Lingering spirits (some helpful, some deceptive)
- **Boss:** Spirit of Death — drains strength, patience-based fight
- **Ambience:** Near-silence, distant wind, soft bell chimes

### Realm 5: Chaos (Xael)
- **Mechanics:** Randomized patterns, shifting controls, evolving puzzles, NPCs from wrong timelines
- **Enemies:** Unpredictable pattern-shifting foes
- **Boss:** Spirit of Chaos — rules are prisons
- **Ambience:** Glitchy audio, random instrument bursts, overlapping fragments

### Realm 6: Creation (Aetherion)
- **Mechanics:** Constantly generating environments, evolving weapons, spawning enemies
- **Enemies:** Self-constructing entities
- **Boss:** Spirit of Creation — find the source, don't fight endlessly
- **Ambience:** Building sounds, crystalline growth tones, energetic hums

### Realm 7: Destruction (Nihros)
- **Mechanics:** Deteriorating weapons, collapsing platforms, resource management
- **Enemies:** Decay-inducing threats
- **Boss:** Spirit of Destruction — protect what remains
- **Ambience:** Crumbling stone, distant thunder, wind through ruins

---

## 5. UI & Controls

### HUD Elements
- Health bar (top-left)
- Stamina bar (below health)
- Black Sword dialogue indicator (bottom-right glow)
- Mother's pendant (inventory, subtle glow during key moments)

### Controls (Keyboard + Gamepad)
| Action | Keyboard | Gamepad |
|--------|----------|---------|
| Move | WASD / Arrow Keys | Left Stick |
| Jump | Space | A / Cross |
| Light Attack | Left Click / J | X / Square |
| Heavy Attack | Right Click / K | Y / Triangle |
| Dodge Roll | Shift | B / Circle |
| Block/Parry | Ctrl | LB / L1 |
| Interact/Talk | E | A / Cross |
| Pause Menu | Escape | Start |
| Inventory | Tab | Select |

### Menus
- **Main Menu:** New Game, Continue, Settings, Quit
- **Pause Menu:** Resume, Settings, Save, Load, Quit to Menu
- **Settings:** Volume (Music/SFX/Ambient/Dialogue), Resolution, Fullscreen, Controls
- **Dialogue Box:** Character portrait + name + text with typewriter effect + choice buttons

---

## 6. Game Flow

```
Nightmare Sequence (Prologue)
    ↓
Village Hub (Explore, talk to NPCs, learn about parents)
    ↓
Black Sword Discovery (Pivotal event)
    ↓
Departure (Overworld intro, father's first whisper)
    ↓
Seven Realms (Time → Space → Life → Death → Chaos → Creation → Destruction)
    ↓
[Critical Memory Scene between Realms 5 & 6]
    ↓
Seven Primordials Gathering
    ↓
The Core (Truth revelation, memory playback)
    ↓
Three Endings → Epilogues → Final Scene (child + black flower)
```

---

## 7. Technical Specifications

| Component | Technology |
|-----------|-----------|
| Engine | Bevy 0.19 (Rust) |
| Physics | bevy_rapier2d |
| Tilemap | LDtk + bevy_ecs_ldtk |
| Audio | bevy_kira_audio |
| UI | bevy_ui |
| Serialization | serde + RON |
| Art Pipeline | Aseprite → PNG sprite sheets |
| Level Design | LDtk → .ldtk files |
| Audio Formats | OGG Vorbis (music) / WAV (SFX) |
| Target FPS | 60fps on minimum spec |
| Resolution | 1280×720 default, scalable |

---

*This is a living document. Updated as development progresses.*
