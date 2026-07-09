//! Save/Load system — serializes game state to JSON files.

use bevy::prelude::*;
use serde::{Deserialize, Serialize};
use std::fs;
use std::path::PathBuf;

use crate::components::*;
use crate::dialogue::ChoiceHistory;
use crate::states::GameState;

/// Plugin managing save/load functionality.
pub struct SavePlugin;

impl Plugin for SavePlugin {
    fn build(&self, app: &mut App) {
        app.insert_resource(SaveSlots::default())
            .add_message::<SaveGameEvent>()
            .add_message::<LoadGameEvent>()
            .add_systems(
                Update,
                (handle_save_event, handle_load_event, save_hotkey_system)
                    .run_if(in_state(GameState::Playing).or_else(in_state(GameState::Paused))),
            );
    }
}

/// Serializable game state for saving.
#[derive(Debug, Clone, Serialize, Deserialize)]
pub struct SaveData {
    /// Save metadata
    pub slot: usize,
    pub save_name: String,
    pub timestamp: String,

    /// Player state
    pub player_position: [f32; 2],
    pub player_health: f32,
    pub player_max_health: f32,
    pub player_stamina: f32,

    /// Progress
    pub current_level: String,
    pub realms_completed: Vec<String>,
    pub story_flags: Vec<String>,

    /// Choice tracking
    pub choice_records: Vec<SerializedChoice>,
}

/// Serializable choice record.
#[derive(Debug, Clone, Serialize, Deserialize)]
pub struct SerializedChoice {
    pub dialogue_id: String,
    pub choice_text: String,
    pub chosen_branch: String,
}

/// Resource tracking save slots.
#[derive(Resource, Debug, Default)]
pub struct SaveSlots {
    pub slots: [Option<SaveData>; 3],
}

/// Event to trigger a save.
#[derive(Message, Debug)]
pub struct SaveGameEvent {
    pub slot: usize,
}

/// Event to trigger a load.
#[derive(Message, Debug)]
pub struct LoadGameEvent {
    pub slot: usize,
}

/// Get the save directory path.
fn save_dir() -> PathBuf {
    PathBuf::from("saves")
}

/// Get the save file path for a slot.
fn save_path(slot: usize) -> PathBuf {
    save_dir().join(format!("save_slot_{}.json", slot))
}

/// System: Handle save events.
pub fn handle_save_event(
    mut events: MessageReader<SaveGameEvent>,
    player_q: Query<(&Transform, &Health, &Stamina), With<Player>>,
    choice_history: Res<ChoiceHistory>,
    mut save_slots: ResMut<SaveSlots>,
) {
    for event in events.read() {
        let Ok((transform, health, stamina)) = player_q.single() else {
            warn!("Cannot save: player not found");
            continue;
        };

        let save_data = SaveData {
            slot: event.slot,
            save_name: format!("Save Slot {}", event.slot + 1),
            timestamp: chrono_now(),
            player_position: [transform.translation.x, transform.translation.y],
            player_health: health.current,
            player_max_health: health.maximum,
            player_stamina: stamina.current,
            current_level: "test_level".to_string(),
            realms_completed: Vec::new(),
            story_flags: Vec::new(),
            choice_records: choice_history
                .choices
                .iter()
                .map(|c| SerializedChoice {
                    dialogue_id: c.dialogue_id.clone(),
                    choice_text: c.choice_text.clone(),
                    chosen_branch: c.chosen_branch.clone(),
                })
                .collect(),
        };

        // Write to file
        if let Err(e) = write_save(&save_data) {
            error!("Failed to save game: {}", e);
        } else {
            info!("Game saved to slot {}", event.slot);
            save_slots.slots[event.slot] = Some(save_data);
        }
    }
}

/// System: Handle load events.
pub fn handle_load_event(
    mut events: MessageReader<LoadGameEvent>,
    mut player_q: Query<(&mut Transform, &mut Health, &mut Stamina), With<Player>>,
) {
    for event in events.read() {
        match read_save(event.slot) {
            Ok(save_data) => {
                if let Ok((mut transform, mut health, mut stamina)) = player_q.single_mut() {
                    transform.translation.x = save_data.player_position[0];
                    transform.translation.y = save_data.player_position[1];
                    health.current = save_data.player_health;
                    health.maximum = save_data.player_max_health;
                    stamina.current = save_data.player_stamina;
                    info!("Game loaded from slot {}", event.slot);
                }
            }
            Err(e) => {
                warn!("Failed to load save slot {}: {}", event.slot, e);
            }
        }
    }
}

/// System: Quick save on F5, quick load on F9.
pub fn save_hotkey_system(
    keyboard: Res<ButtonInput<KeyCode>>,
    mut save_events: MessageWriter<SaveGameEvent>,
    mut load_events: MessageWriter<LoadGameEvent>,
) {
    if keyboard.just_pressed(KeyCode::F5) {
        save_events.write(SaveGameEvent { slot: 0 });
    }
    if keyboard.just_pressed(KeyCode::F9) {
        load_events.write(LoadGameEvent { slot: 0 });
    }
}

/// Write save data to disk.
fn write_save(data: &SaveData) -> Result<(), String> {
    let dir = save_dir();
    fs::create_dir_all(&dir).map_err(|e| format!("Cannot create save dir: {}", e))?;

    let path = save_path(data.slot);
    let json = serde_json::to_string_pretty(data).map_err(|e| format!("Serialize error: {}", e))?;
    fs::write(&path, json).map_err(|e| format!("Write error: {}", e))?;

    Ok(())
}

/// Read save data from disk.
fn read_save(slot: usize) -> Result<SaveData, String> {
    let path = save_path(slot);
    let json = fs::read_to_string(&path).map_err(|e| format!("Read error: {}", e))?;
    let data: SaveData =
        serde_json::from_str(&json).map_err(|e| format!("Deserialize error: {}", e))?;
    Ok(data)
}

/// Simple timestamp (no chrono crate needed).
fn chrono_now() -> String {
    "save".to_string() // Placeholder — would use system time in production
}
