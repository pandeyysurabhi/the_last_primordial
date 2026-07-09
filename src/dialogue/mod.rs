//! Dialogue system — displays text boxes, typewriter effect, choices, sword voices.

pub mod parser;

use bevy::prelude::*;
use std::collections::HashMap;

use crate::states::GameState;
use parser::*;

/// Plugin managing the dialogue system.
pub struct DialoguePlugin;

impl Plugin for DialoguePlugin {
    fn build(&self, app: &mut App) {
        app.init_resource::<ChoiceHistory>()
            .init_resource::<ActiveDialogue>()
            .add_systems(OnEnter(GameState::Dialogue), spawn_dialogue_ui)
            .add_systems(OnExit(GameState::Dialogue), cleanup_dialogue_ui)
            .add_systems(
                Update,
                (
                    typewriter_system,
                    dialogue_advance_system,
                )
                    .run_if(in_state(GameState::Dialogue)),
            );
    }
}

/// Tracks all player choices throughout the game.
#[derive(Resource, Debug, Default)]
pub struct ChoiceHistory {
    pub choices: Vec<ChoiceRecord>,
    pub philosophy_scores: HashMap<String, i32>,
}

/// A single recorded choice.
#[derive(Debug, Clone)]
pub struct ChoiceRecord {
    pub dialogue_id: String,
    pub choice_text: String,
    pub chosen_branch: String,
}

/// Currently active dialogue state.
#[derive(Resource, Debug, Default)]
pub struct ActiveDialogue {
    pub nodes: Vec<DialogueNode>,
    pub current_index: usize,
    pub is_active: bool,
    pub typewriter_progress: usize,
    pub typewriter_timer: f32,
    pub full_text: String,
    pub speaker: String,
}

impl ActiveDialogue {
    /// Start a new dialogue sequence.
    pub fn start(&mut self, nodes: Vec<DialogueNode>) {
        self.nodes = nodes;
        self.current_index = 0;
        self.is_active = true;
        self.advance_to_current();
    }

    /// Set up current node for display.
    fn advance_to_current(&mut self) {
        if self.current_index >= self.nodes.len() {
            self.is_active = false;
            return;
        }

        match &self.nodes[self.current_index] {
            DialogueNode::DialogueLine(line) => {
                self.speaker = line.speaker.clone();
                self.full_text = line.text.clone();
                self.typewriter_progress = 0;
                self.typewriter_timer = 0.0;
            }
            DialogueNode::Narration(narr) => {
                self.speaker = String::new();
                self.full_text = narr.text.clone();
                self.typewriter_progress = 0;
                self.typewriter_timer = 0.0;
            }
            DialogueNode::SwordVoice(voice) => {
                self.speaker = format!("⚔ {}", voice.wielder_id);
                self.full_text = voice.text.clone();
                self.typewriter_progress = 0;
                self.typewriter_timer = 0.0;
            }
            DialogueNode::PlayerChoice(_) => {
                // Choice UI handled separately
            }
        }
    }

    /// Advance to next node.
    pub fn advance(&mut self) {
        self.current_index += 1;
        self.advance_to_current();
    }

    /// Get the currently visible text (typewriter effect).
    pub fn visible_text(&self) -> &str {
        let end = self.typewriter_progress.min(self.full_text.len());
        &self.full_text[..end]
    }

    /// Check if typewriter has finished revealing all text.
    pub fn is_text_complete(&self) -> bool {
        self.typewriter_progress >= self.full_text.len()
    }

    /// Complete the typewriter immediately.
    pub fn complete_text(&mut self) {
        self.typewriter_progress = self.full_text.len();
    }
}

/// Marker for dialogue UI root.
#[derive(Component)]
pub struct DialogueUiRoot;

/// Marker for dialogue text element.
#[derive(Component)]
pub struct DialogueText;

/// Marker for speaker name element.
#[derive(Component)]
pub struct SpeakerName;

/// Spawn the dialogue box UI.
pub fn spawn_dialogue_ui(mut commands: Commands) {
    commands
        .spawn((
            Node {
                width: Val::Percent(100.0),
                height: Val::Percent(100.0),
                flex_direction: FlexDirection::Column,
                justify_content: JustifyContent::FlexEnd,
                align_items: AlignItems::Center,
                padding: UiRect::all(Val::Px(20.0)),
                ..default()
            },
            DialogueUiRoot,
            Name::new("DialogueUI"),
        ))
        .with_children(|parent| {
            // Dialogue box container
            parent
                .spawn((
                    Node {
                        width: Val::Percent(80.0),
                        min_height: Val::Px(120.0),
                        flex_direction: FlexDirection::Column,
                        padding: UiRect::all(Val::Px(16.0)),
                        row_gap: Val::Px(8.0),
                        border_radius: BorderRadius::all(Val::Px(8.0)),
                        ..default()
                    },
                    BackgroundColor(Color::srgba(0.05, 0.03, 0.1, 0.92)),
                ))
                .with_children(|box_parent| {
                    // Speaker name
                    box_parent.spawn((
                        Text::new(""),
                        TextFont {
                            font_size: FontSize::Px(16.0),
                            ..default()
                        },
                        TextColor(Color::srgb(0.7, 0.55, 1.0)),
                        SpeakerName,
                    ));

                    // Dialogue text (typewriter)
                    box_parent.spawn((
                        Text::new(""),
                        TextFont {
                            font_size: FontSize::Px(14.0),
                            ..default()
                        },
                        TextColor(Color::srgb(0.9, 0.88, 0.95)),
                        DialogueText,
                    ));

                    // Advance hint
                    box_parent.spawn((
                        Text::new("[E] Continue"),
                        TextFont {
                            font_size: FontSize::Px(11.0),
                            ..default()
                        },
                        TextColor(Color::srgba(0.5, 0.45, 0.6, 0.6)),
                        Node {
                            align_self: AlignSelf::FlexEnd,
                            ..default()
                        },
                    ));
                });
        });
}

/// System: Typewriter text reveal effect.
pub fn typewriter_system(
    time: Res<Time>,
    mut dialogue: ResMut<ActiveDialogue>,
    mut text_q: Query<&mut Text, With<DialogueText>>,
    mut speaker_q: Query<&mut Text, (With<SpeakerName>, Without<DialogueText>)>,
) {
    if !dialogue.is_active {
        return;
    }

    let dt = time.delta_secs();
    let chars_per_second = 30.0;

    dialogue.typewriter_timer += dt;

    let chars_to_show = (dialogue.typewriter_timer * chars_per_second) as usize;
    dialogue.typewriter_progress = chars_to_show.min(dialogue.full_text.len());

    // Update displayed text
    let visible = dialogue.visible_text().to_string();
    for mut text in &mut text_q {
        **text = visible.clone();
    }

    // Update speaker name
    for mut text in &mut speaker_q {
        **text = dialogue.speaker.clone();
    }
}

/// System: Advance dialogue on input.
pub fn dialogue_advance_system(
    keyboard: Res<ButtonInput<KeyCode>>,
    mut dialogue: ResMut<ActiveDialogue>,
    mut next_state: ResMut<NextState<GameState>>,
) {
    if !dialogue.is_active {
        next_state.set(GameState::Playing);
        return;
    }

    if keyboard.just_pressed(KeyCode::KeyE) || keyboard.just_pressed(KeyCode::Space) {
        if dialogue.is_text_complete() {
            dialogue.advance();
        } else {
            // Skip typewriter — show full text immediately
            dialogue.complete_text();
        }
    }
}

/// Cleanup dialogue UI when exiting dialogue state.
pub fn cleanup_dialogue_ui(mut commands: Commands, query: Query<Entity, With<DialogueUiRoot>>) {
    for entity in &query {
        commands.entity(entity).despawn();
    }
}

/// Helper: Start a simple dialogue from code (for testing).
pub fn start_test_dialogue(dialogue: &mut ActiveDialogue) {
    dialogue.start(vec![
        DialogueNode::Narration(Narration {
            text: "A sword stands upright outside your home. Black as void.".to_string(),
            style: parser::TextStyle::Italic,
            duration: parser::Duration::Auto,
        }),
        DialogueNode::DialogueLine(DialogueLine {
            speaker: "Elder Maren".to_string(),
            portrait: "elder_neutral".to_string(),
            text: "Kael. You're awake early. The nightmares again?".to_string(),
            emotion: Emotion::Compassion,
            duration: parser::Duration::Auto,
            on_complete: OnComplete::None,
        }),
        DialogueNode::SwordVoice(SwordVoice {
            wielder_id: "Ancient Wielder".to_string(),
            portrait: "wielder_ancient".to_string(),
            text: "Another one. After all this time...".to_string(),
            emotion: Emotion::Exhausted,
            color: "#8888CC".to_string(),
        }),
        DialogueNode::Narration(Narration {
            text: "The voices fade. You are alone again.".to_string(),
            style: parser::TextStyle::Normal,
            duration: parser::Duration::Auto,
        }),
    ]);
}
