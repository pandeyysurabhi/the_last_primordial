# The Last Primordial — Art Direction & Style Guide

> **Version:** 0.2.0 | **Last Updated:** 2026-07-10 | **Engine:** Godot 4.3 (.NET/C#)

---

## 1. Visual Identity

**Overall Style:** Pixel art with hand-drawn elements for special moments (cutscenes, the Core).
The game should feel like a beautifully illustrated storybook rendered in pixels — detailed, atmospheric, and emotionally evocative.

**Influences:**
- Celeste (tight pixel art, emotional storytelling)
- Hyper Light Drifter (atmospheric, color-driven worlds)
- Hollow Knight (interconnected world, atmospheric depth)
- Owlboy (detailed pixel art, rich environments)

---

## 2. Technical Specifications

| Property | Value |
|----------|-------|
| **Tile Size** | 16×16 px |
| **Player Sprite** | 32×48 px |
| **NPC Sprites** | 32×48 px |
| **Enemy Sprites** | 32×32 to 48×48 px |
| **Boss Sprites** | 64×64 to 128×128 px |
| **Primordial Sprites** | 48×64 px |
| **Portrait Size** | 128×128 px |
| **Resolution** | 320×180 internal (scaled to 1280×720) |
| **Scale Factor** | 4x |
| **Sprite Format** | PNG (horizontal strip or grid) |
| **Metadata** | JSON (Aseprite export) → imported as Godot SpriteFrames |

### Godot Import Settings (Critical for Pixel Art)
| Setting | Value | Location |
|---------|-------|----------|
| **Texture Filter** | Nearest | Project Settings → Rendering → Textures |
| **Texture Repeat** | Disabled | Per-sprite import preset |
| **Mipmaps** | Off | Import dock → uncheck "Generate Mipmaps" |
| **2D Pixel Snap** | Enabled | Project Settings → Rendering → 2D |
| **Stretch Mode** | Viewport | Project Settings → Display → Window → Stretch |
| **Stretch Aspect** | Keep | Project Settings → Display → Window → Stretch |

---

## 3. Animation Frame Counts

| Character Type | Animation | Frame Count | Notes |
|---------------|-----------|-------------|-------|
| **Kael** | Idle | 6 | Subtle breathing + sword glow |
| | Walk | 8 | |
| | Run | 8 | |
| | Jump | 4 | Rise + peak |
| | Fall | 2 | |
| | Land | 3 | |
| | Light Attack | 6 | Quick slash |
| | Heavy Attack | 10 | Wind-up + impact |
| | Dodge Roll | 6 | With afterimage |
| | Parry | 4 | Shield flash |
| | Hurt | 3 | |
| | Death | 8 | Collapse + fade |
| | Ledge Grab | 4 | |
| | Dash | 4 | |
| **Enemies** | Patrol | 6 | |
| | Chase | 6 | |
| | Attack | 6–8 | |
| | Hurt | 3 | |
| | Death | 6 | |
| **Bosses** | Idle | 8–12 | Large, detailed |
| | Attack (×3-5) | 12–16 | Unique per pattern |
| | Phase Shift | 8–12 | Transformation |
| | Defeat | 16–24 | Dissolve into light |
| **Primordials** | Appear | 6–8 | Materialize effect |
| | Idle | 4–6 | Presence, not motion |
| | Speak | 4 | Subtle gesture |
| | Disappear | 6–8 | Fade/dissolve |
| **Environment** | Water flow | 8 | Tilemap animated |
| | Fire flicker | 6 | |
| | Plant growth | 8 | Realm of Life |
| | Portal swirl | 8 | |

---

## 4. Color Palettes

### Village (Home)
Warm, nostalgic, safe. Golden hour lighting.
```
Background:  #2B1B3D  #3D2B52  #4A3562
Midground:   #6B8C5A  #8BAF6E  #A8CC84
Foreground:  #D4A653  #E8C76D  #F2D98A
Accents:     #C75B3F  #E87655  #FF9B7A
Sky:         #FF9F6B  #FFB98A  #FFDBB5
```

### Realm of Time (Chronar)
Ethereal, clockwork, amber-gold and blue-grey.
```
Background:  #1A1A2E  #16213E  #0F3460
Midground:   #3D5A80  #5A7FA0  #98C1D9
Foreground:  #D4A96A  #E8C888  #F0D9A8
Time FX:     #FFD700  #FFA500  #FF6B00
Accents:     #E2E2E2  #C8C8C8  #9E9E9E
```

### Realm of Space (Vastara)
Cosmic, infinite, deep purples and starlight.
```
Background:  #0D0221  #150734  #1A0E4A
Midground:   #3C1874  #5C2D91  #7B52AB
Foreground:  #A67EC4  #C9A8E0  #E8D5F5
Stars:       #FFFFFF  #E0E7FF  #B8C6FF
Portals:     #00FFE7  #00D4CC  #009E98
```

### Realm of Life (Elaria)
Lush, overwhelming green, warm gold accents.
```
Background:  #0A2E14  #134A24  #1D6638
Midground:   #2E8B4A  #4CAF6E  #6ECF8E
Foreground:  #8FEF9F  #B4FFB8  #D4FFD6
Flowers:     #FF6B9D  #FF89B4  #FFADD2
Gold:        #FFD700  #FFEA00  #FFF59D
```

### Realm of Death (Morvex)
Muted, peaceful, soft grays and pale blues.
```
Background:  #1A1A1E  #252528  #2F2F34
Midground:   #5C5C66  #7A7A86  #9898A4
Foreground:  #B8B8C4  #D2D2DE  #E8E8F0
Souls:       #C8D8FF  #A8B8E8  #8898D0
Accents:     #6B8CFF  #4A6BCC  #2A4A99
```

### Realm of Chaos (Xael)
Multicolor, glitchy, constantly shifting.
```
Background:  #1A0A2E  #2E0A1A  #0A1A2E
Midground:   #FF006B  #00FF88  #6B00FF
Foreground:  #FFD700  #00DDFF  #FF4488
Glitch:      #FF0000  #00FF00  #0000FF
Static:      #FFFFFF  #000000  #808080
```

### Realm of Creation (Aetherion)
Hopeful, expansive, gold and white with cosmic hues.
```
Background:  #0A0A1E  #141428  #1E1E38
Midground:   #3E3E6E  #5E5EA0  #7E7ED0
Foreground:  #FFD700  #FFE44D  #FFF099
Light:       #FFFFFF  #FFFFCC  #FFFF99
Crystals:    #00CCFF  #33DDFF  #66EEFF
```

### Realm of Destruction (Nihros)
Somber, resilient, dark reds, grays, and embers.
```
Background:  #1A0A0A  #2E1414  #3D1E1E
Midground:   #5C2828  #7A3636  #984444
Foreground:  #B85C5C  #D07474  #E88C8C
Embers:      #FF4400  #FF6622  #FF8844
Ash:         #808080  #A0A0A0  #C0C0C0
```

### The Core
Unique — less pixel art, more painterly/abstract. White light, floating memories.
```
Background:  #FFFFFF  #F8F4FF  #F0EAFF
Light:       #FFE8D0  #FFD4B0  #FFC090
Memory:      #D0D8FF  #B0C0FF  #90A8FF
Accent:      #FFB800  #FF9500  #FF7200
```

---

## 5. Primordial Visual Design

| Primordial | Visual Direction | Key Visual Element |
|-----------|-----------------|-------------------|
| **Chronar** | Exhausted despite immortality. Eyes that have seen too much. | Clock-like motifs, amber glow |
| **Vastara** | Ethereal, slightly translucent. Stars visible within her form. | Cosmic particles, deep purple |
| **Elaria** | Surrounded by small blooming flowers. Compassionate but overwhelmed. | Green/gold aura, living flora |
| **Morvex** | Emotionally distant, not evil. Gentle eyes filled with ancient guilt. | Pale glow, ghost-like wisps |
| **Xael** | Appearance subtly shifts — never the same twice. Playful expression. | Multicolor glitch effects |
| **Aetherion** | Surrounded by tiny stars and light. Warm, hopeful expression. | Gold/white radiance, orbiting lights |
| **Nihros** | Covered in scars from every necessary destruction. Powerful, weary. | Dark reds/grays, ember particles |

---

## 6. VFX Guidelines

| Effect | Style | Usage |
|--------|-------|-------|
| Sword slash trail | 4-frame overlay sprite | Combat |
| Dodge afterimage | Ghost-opacity copy of player | Dodge roll |
| Hit spark | 4-frame burst sprite | On damage |
| Healing glow | 6-frame radial pulse | Health recovery |
| Black Sword glow | Constant subtle oscillation | Narrative moments |
| Screen shake | Camera displacement | Boss impacts, explosions |
| Chromatic aberration | RGB channel offset shader | Nightmare, Chaos realm |
| Vignette pulse | Edge darkening shader | Nightmare sequences |

---

## 7. Parallax Background Layers

Each area should use 3-5 parallax layers:
1. **Far background** — Sky/atmosphere (slowest scroll, ~0.1x)
2. **Mid-far background** — Distant landscape (0.3x)
3. **Mid background** — Environmental features (0.5x)
4. **Foreground** — Tilemap (1.0x, camera speed)
5. **Near foreground** — Atmospheric particles, fog (1.2x, slight overlap)

---

*Reference board to be compiled using concept art and Pinterest mood boards.*
