//! The Last Primordial
//! 
//! A 2D action-adventure built in Bevy (Rust), featuring seven unique realms,
//! philosophical storytelling, pixel art visuals, and three branching endings.
//! 
//! Protagonist: Kael — the last Choice Wielder
//! Engine: Bevy 0.19

use bevy::prelude::*;

/// Application entry point.
/// Initializes the Bevy engine with game window configuration.
fn main() {
    App::new()
        .add_plugins(DefaultPlugins.set(WindowPlugin {
            primary_window: Some(Window {
                title: "The Last Primordial".to_string(),
                resolution: (1280u32, 720u32).into(),
                resizable: true,
                ..default()
            }),
            ..default()
        }))
        .insert_resource(ClearColor(Color::srgb(0.05, 0.03, 0.08))) // Deep dark purple-black
        .add_systems(Startup, setup_camera)
        .run();
}

/// Sets up the 2D camera for the game.
fn setup_camera(mut commands: Commands) {
    commands.spawn(Camera2d);
}
