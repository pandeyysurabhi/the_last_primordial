# Dialogue Script Format Specification

## Overview
All dialogue in The Last Primordial uses RON (Rusty Object Notation) files.
RON is a Rust-native data format that maps cleanly to Rust structs.

## File Location
All dialogue files go in `assets/dialogues/`

## Naming Convention
`<context>_<scene_name>.ron`
Examples:
- `village_elder_intro.ron`
- `chronar_meeting.ron`
- `nightmare_prologue.ron`
- `black_sword_discovery.ron`

## Data Structures

### DialogueSequence
The top-level container for a dialogue scene.
```ron
DialogueSequence(
    id: "unique_scene_id",
    trigger: <TriggerCondition>,
    scenes: [ ... ],
)
```

### TriggerCondition
When this dialogue activates:
- `OnInteract("entity_id")` — Player interacts with NPC/object
- `AfterBoss("boss_id")` — After defeating a boss
- `OnEnterZone("zone_id")` — When entering an area
- `OnItemPickup("item_id")` — When picking up an item
- `StoryFlag("flag_name")` — When a story flag is set
- `Auto` — Triggers automatically when loaded

### DialogueLine
A single line of dialogue:
```ron
DialogueLine(
    speaker: "Character Name",
    portrait: "portrait_asset_id",
    text: "The dialogue text...",
    emotion: <Emotion>,
    duration: Auto,  // or Timed(3.0) for seconds
    on_complete: None,  // or FadeOut(2.0), PlaySfx("sound"), SetFlag("flag")
)
```

### Emotion Tags
`Neutral`, `Angry`, `Sad`, `Happy`, `Mysterious`, `Exhausted`,
`Melancholy`, `Wisdom`, `Fear`, `Hope`, `Guilt`, `Determination`,
`Surprise`, `Compassion`, `Playful`

### PlayerChoice
Branching dialogue:
```ron
PlayerChoice(
    options: [
        ("Choice text", "next_scene_id"),
        ("Another choice", "other_scene_id"),
    ]
)
```

### Narration
Non-character text (internal thoughts, descriptions):
```ron
Narration(
    text: "The narration text...",
    style: Italic,  // or Normal, Bold
    duration: Auto,
)
```

### SwordVoice
Black Sword wielder voices:
```ron
SwordVoice(
    wielder_id: "wielder_003",
    portrait: "wielder_003_angry",
    text: "Their words...",
    emotion: Angry,
    color: "#FF6B6B",  // Unique color per wielder
)
```

## Choice Tracking
Player choices are stored in a `ChoiceHistory` resource:
- Each choice is tagged with a philosophy: `Mercy`, `Aggression`, `Sacrifice`, `Pragmatism`
- The Black Sword's voice composition shifts based on accumulated philosophy
- Ending availability depends on choice patterns throughout the game
