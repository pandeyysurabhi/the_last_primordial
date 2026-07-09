//! RON dialogue file parser — deserializes dialogue sequences into game data.

use serde::{Deserialize, Serialize};

/// Top-level dialogue sequence loaded from a .ron file.
#[derive(Debug, Clone, Serialize, Deserialize)]
pub struct DialogueSequence {
    pub id: String,
    pub trigger: TriggerCondition,
    pub scenes: Vec<DialogueNode>,
}

/// What triggers a dialogue to start.
#[derive(Debug, Clone, Serialize, Deserialize)]
pub enum TriggerCondition {
    OnInteract(String),
    AfterBoss(String),
    OnEnterZone(String),
    OnItemPickup(String),
    StoryFlag(String),
    Auto,
}

/// A single node in a dialogue tree.
#[derive(Debug, Clone, Serialize, Deserialize)]
pub enum DialogueNode {
    DialogueLine(DialogueLine),
    PlayerChoice(PlayerChoice),
    Narration(Narration),
    SwordVoice(SwordVoice),
}

/// A line spoken by a character.
#[derive(Debug, Clone, Serialize, Deserialize)]
pub struct DialogueLine {
    pub speaker: String,
    pub portrait: String,
    pub text: String,
    pub emotion: Emotion,
    pub duration: Duration,
    #[serde(default)]
    pub on_complete: OnComplete,
}

/// Player choice with branching options.
#[derive(Debug, Clone, Serialize, Deserialize)]
pub struct PlayerChoice {
    pub options: Vec<(String, String)>, // (display_text, next_scene_id)
}

/// Narration text (non-character).
#[derive(Debug, Clone, Serialize, Deserialize)]
pub struct Narration {
    pub text: String,
    pub style: TextStyle,
    pub duration: Duration,
}

/// Black Sword wielder voice.
#[derive(Debug, Clone, Serialize, Deserialize)]
pub struct SwordVoice {
    pub wielder_id: String,
    pub portrait: String,
    pub text: String,
    pub emotion: Emotion,
    pub color: String, // Hex color like "#FF6B6B"
}

/// Character emotion for portrait selection.
#[derive(Debug, Clone, Serialize, Deserialize)]
pub enum Emotion {
    Neutral,
    Angry,
    Sad,
    Happy,
    Mysterious,
    Exhausted,
    Melancholy,
    Wisdom,
    Fear,
    Hope,
    Guilt,
    Determination,
    Surprise,
    Compassion,
    Playful,
}

/// Duration of a dialogue element.
#[derive(Debug, Clone, Serialize, Deserialize)]
pub enum Duration {
    Auto,
    Timed(f32),
}

/// Text display style for narration.
#[derive(Debug, Clone, Serialize, Deserialize)]
pub enum TextStyle {
    Normal,
    Italic,
    Bold,
}

/// Action to perform when a dialogue line completes.
#[derive(Debug, Clone, Serialize, Deserialize)]
pub enum OnComplete {
    None,
    FadeOut(f32),
    PlaySfx(String),
    SetFlag(String),
}

impl Default for OnComplete {
    fn default() -> Self {
        OnComplete::None
    }
}

/// Choice philosophy tag for tracking player decisions.
#[derive(Debug, Clone, Serialize, Deserialize, PartialEq, Eq, Hash)]
pub enum Philosophy {
    Mercy,
    Aggression,
    Sacrifice,
    Pragmatism,
}
