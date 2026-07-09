//! UI plugin — registers HUD and menu systems.

pub mod hud;
pub mod menus;

use bevy::prelude::*;

use crate::states::GameState;

/// Plugin managing all UI systems.
pub struct UiPlugin;

impl Plugin for UiPlugin {
    fn build(&self, app: &mut App) {
        app
            // Main menu
            .add_systems(OnEnter(GameState::MainMenu), menus::spawn_main_menu)
            .add_systems(OnExit(GameState::MainMenu), menus::cleanup_main_menu)
            .add_systems(
                Update,
                (menus::main_menu_button_system, menus::button_hover_system)
                    .run_if(in_state(GameState::MainMenu)),
            )
            // HUD (spawns when entering Playing state)
            .add_systems(OnEnter(GameState::Playing), hud::spawn_hud)
            .add_systems(
                Update,
                (hud::update_health_bar, hud::update_stamina_bar)
                    .run_if(in_state(GameState::Playing)),
            )
            // Pause menu
            .add_systems(OnEnter(GameState::Paused), menus::spawn_pause_menu)
            .add_systems(OnExit(GameState::Paused), menus::cleanup_pause_menu)
            .add_systems(
                Update,
                (menus::pause_menu_button_system, menus::button_hover_system)
                    .run_if(in_state(GameState::Paused)),
            )
            // Pause toggle (runs in both Playing and Paused)
            .add_systems(
                Update,
                menus::pause_toggle_system
                    .run_if(in_state(GameState::Playing).or_else(in_state(GameState::Paused))),
            );
    }
}
