//! The Last Primordial
//!
//! A 2D action-adventure built in Bevy (Rust), featuring seven unique realms,
//! philosophical storytelling, pixel art visuals, and three branching endings.
//!
//! Protagonist: Kael — the last Choice Wielder
//! Engine: Bevy 0.19 + Avian2D physics

mod audio;
mod camera;
mod components;
mod dialogue;
mod enemy;
mod level;
mod player;
mod save;
mod sprites;
mod states;
mod ui;

use avian2d::prelude::*;
use bevy::prelude::*;

use states::GameState;

/// Application entry point.
fn main() {
    App::new()
        // ── Core Bevy ──
        .add_plugins(
            DefaultPlugins
                .set(WindowPlugin {
                    primary_window: Some(Window {
                        title: "The Last Primordial".to_string(),
                        resolution: (1280u32, 720u32).into(),
                        resizable: true,
                        ..default()
                    }),
                    ..default()
                })
                .set(ImagePlugin::default_nearest()), // Pixel-perfect rendering
        )
        .insert_resource(ClearColor(Color::srgb(0.05, 0.03, 0.08)))
        // ── Physics ──
        .add_plugins(PhysicsPlugins::default().with_length_unit(100.0))
        // ── Game State ──
        .init_state::<GameState>()
        // ── Game Plugins ──
        .add_plugins((
            player::PlayerPlugin,
            camera::GameCameraPlugin,
            enemy::EnemyPlugin,
            level::LevelPlugin,
            ui::UiPlugin,
            dialogue::DialoguePlugin,
            save::SavePlugin,
            sprites::SpritePlugin,
            audio::GameAudioPlugin,
        ))
        // ── Startup ──
        .add_systems(Startup, setup_camera)
        // ── Debug: trigger test dialogue on T key ──
        .add_systems(
            Update,
            debug_dialogue_trigger.run_if(in_state(GameState::Playing)),
        )
        .run();
}

/// Sets up the 2D camera for the game.
fn setup_camera(mut commands: Commands) {
    commands.spawn((
        Camera2d,
        Transform::from_translation(Vec3::new(0.0, 0.0, 999.0)),
    ));
}

/// Debug: Press T to trigger a test dialogue sequence.
fn debug_dialogue_trigger(
    keyboard: Res<ButtonInput<KeyCode>>,
    mut dialogue: ResMut<dialogue::ActiveDialogue>,
    mut next_state: ResMut<NextState<GameState>>,
) {
    if keyboard.just_pressed(KeyCode::KeyT) {
        dialogue::start_test_dialogue(&mut dialogue);
        next_state.set(GameState::Dialogue);
    }
}
