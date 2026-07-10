# Dialogue Script Format Specification

> **Engine:** Godot 4.3 (.NET/C#) | **Format:** JSON

## Overview
All dialogue in The Last Primordial uses **JSON** files, parsed by C# using `System.Text.Json`.
JSON provides easy serialization/deserialization with C# and is human-readable for writing and editing.

## File Location
All dialogue files go in `assets/dialogues/`

## Naming Convention
`<context>_<scene_name>.json`
Examples:
- `village_elder.json`
- `chronar_meeting.json`
- `nightmare_prologue.json`
- `black_sword_discovery.json`

## Data Structures

### DialogueSequence (Top-Level)
The root object of every dialogue file.
```json
{
  "id": "unique_scene_id",
  "trigger": { "type": "...", ... },
  "scenes": [ ... ]
}
```

### Trigger Types
When this dialogue activates:
| Type | Fields | Example |
|------|--------|---------|
| `OnInteract` | `entity_id` | `{ "type": "OnInteract", "entity_id": "elder_home_npc" }` |
| `AfterBoss` | `boss_id` | `{ "type": "AfterBoss", "boss_id": "spirit_of_time" }` |
| `OnEnterZone` | `zone_id` | `{ "type": "OnEnterZone", "zone_id": "realm_time_entrance" }` |
| `OnItemPickup` | `item_id` | `{ "type": "OnItemPickup", "item_id": "black_sword" }` |
| `StoryFlag` | `flag` | `{ "type": "StoryFlag", "flag": "village_explored" }` |
| `Auto` | *(none)* | `{ "type": "Auto" }` |

### Scene Entry Types

#### DialogueLine
A single line of dialogue:
```json
{
  "type": "DialogueLine",
  "speaker": "Character Name",
  "portrait": "portrait_asset_id",
  "text": "The dialogue text...",
  "emotion": "Neutral",
  "duration": { "type": "Auto" },
  "on_complete": null
}
```

**Duration options:**
- `{ "type": "Auto" }` — waits for player input
- `{ "type": "Timed", "seconds": 3.0 }` — auto-advances after N seconds

**on_complete options:**
- `null` — no action
- `{ "type": "FadeOut", "duration": 2.0 }`
- `{ "type": "PlaySfx", "sound": "sound_id" }`
- `{ "type": "SetFlag", "flag": "flag_name" }`

#### Narration
Non-character text (internal thoughts, descriptions):
```json
{
  "type": "Narration",
  "text": "The narration text...",
  "style": "Italic",
  "duration": { "type": "Auto" }
}
```
**Style options:** `Normal`, `Italic`, `Bold`

#### PlayerChoice
Branching dialogue:
```json
{
  "type": "PlayerChoice",
  "options": [
    { "text": "Choice text", "next_scene": "scene_id" },
    { "text": "Another choice", "next_scene": "other_scene_id" }
  ]
}
```

#### SwordVoice
Black Sword wielder voices:
```json
{
  "type": "SwordVoice",
  "wielder_id": "wielder_003",
  "portrait": "wielder_003_angry",
  "text": "Their words...",
  "emotion": "Angry",
  "color": "#FF6B6B"
}
```

### Emotion Tags
`Neutral`, `Angry`, `Sad`, `Happy`, `Mysterious`, `Exhausted`,
`Melancholy`, `Wisdom`, `Fear`, `Hope`, `Guilt`, `Determination`,
`Surprise`, `Compassion`, `Playful`

### Branching
Scenes that belong to a specific branch include a `_branch` field:
```json
{
  "_branch": "chronar_response_grief",
  "type": "DialogueLine",
  "speaker": "Chronar",
  ...
}
```
The dialogue system reads the `_branch` field and only displays that entry when the player's previous choice pointed to that branch ID.

## Choice Tracking
Player choices are stored in a save-game `ChoiceHistory`:
- Each choice is tagged with a philosophy: `Mercy`, `Aggression`, `Sacrifice`, `Pragmatism`
- The Black Sword's voice composition shifts based on accumulated philosophy
- Ending availability depends on choice patterns throughout the game

## C# Integration
Dialogue files are loaded via `FileAccess.Open()` and deserialized with `System.Text.Json.JsonSerializer.Deserialize<DialogueSequence>()`.
See `src/Dialogue/` for the C# data models and parser.
