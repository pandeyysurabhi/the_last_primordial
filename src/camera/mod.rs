//! Camera system — smooth-follow with dead zone, room boundaries, parallax layers.

use bevy::prelude::*;

use crate::components::Player;
use crate::states::GameState;

/// Camera plugin for the game.
pub struct GameCameraPlugin;

impl Plugin for GameCameraPlugin {
    fn build(&self, app: &mut App) {
        app.insert_resource(CameraConfig::default())
            .add_systems(
                Update,
                (camera_follow_system, parallax_system).run_if(in_state(GameState::Playing)),
            );
    }
}

/// Configuration for camera behavior.
#[derive(Resource, Debug)]
pub struct CameraConfig {
    /// How fast camera catches up to target (higher = snappier).
    pub lerp_speed: f32,
    /// Dead zone — player can move within this box before camera moves.
    pub dead_zone: Vec2,
    /// Optional room boundaries (min_x, min_y, max_x, max_y).
    pub bounds: Option<CameraBounds>,
    /// Pixel-perfect zoom (4x for 320x180 → 1280x720).
    pub zoom: f32,
}

impl Default for CameraConfig {
    fn default() -> Self {
        Self {
            lerp_speed: 5.0,
            dead_zone: Vec2::new(30.0, 20.0),
            bounds: None,
            zoom: 1.0,
        }
    }
}

/// Camera room boundaries.
#[derive(Debug, Clone, Copy)]
pub struct CameraBounds {
    pub min: Vec2,
    pub max: Vec2,
}

/// Marker for parallax background layers.
#[derive(Component, Debug)]
pub struct ParallaxLayer {
    /// Scroll speed multiplier (0.0 = fixed, 1.0 = same as camera).
    pub speed: f32,
}

/// System: Camera smoothly follows the player with dead zone and boundaries.
pub fn camera_follow_system(
    time: Res<Time>,
    config: Res<CameraConfig>,
    player_q: Query<&Transform, (With<Player>, Without<Camera2d>)>,
    mut camera_q: Query<&mut Transform, With<Camera2d>>,
) {
    let Ok(player_tf) = player_q.single() else {
        return;
    };
    let Ok(mut camera_tf) = camera_q.single_mut() else {
        return;
    };

    let dt = time.delta_secs();
    let player_pos = player_tf.translation.truncate();
    let camera_pos = camera_tf.translation.truncate();

    // Calculate target with dead zone
    let mut target = camera_pos;

    let diff = player_pos - camera_pos;

    // Only move camera when player exits dead zone
    if diff.x.abs() > config.dead_zone.x {
        target.x = player_pos.x - config.dead_zone.x * diff.x.signum();
    }
    if diff.y.abs() > config.dead_zone.y {
        target.y = player_pos.y - config.dead_zone.y * diff.y.signum();
    }

    // Clamp to room boundaries if set
    if let Some(bounds) = &config.bounds {
        target.x = target.x.clamp(bounds.min.x, bounds.max.x);
        target.y = target.y.clamp(bounds.min.y, bounds.max.y);
    }

    // Smooth lerp toward target
    let new_pos = camera_pos.lerp(target, (config.lerp_speed * dt).min(1.0));

    camera_tf.translation.x = new_pos.x;
    camera_tf.translation.y = new_pos.y;
}

/// System: Move parallax layers based on camera position.
pub fn parallax_system(
    camera_q: Query<&Transform, With<Camera2d>>,
    mut layers: Query<(&mut Transform, &ParallaxLayer), Without<Camera2d>>,
) {
    let Ok(camera_tf) = camera_q.single() else {
        return;
    };

    let camera_pos = camera_tf.translation.truncate();

    for (mut layer_tf, parallax) in &mut layers {
        layer_tf.translation.x = camera_pos.x * parallax.speed;
        layer_tf.translation.y = camera_pos.y * parallax.speed;
    }
}

/// Spawn parallax background layers (placeholder colored rectangles).
pub fn spawn_parallax_backgrounds(commands: &mut Commands) {
    let layers = [
        (Color::srgba(0.02, 0.01, 0.05, 1.0), 0.1, -50.0, Vec2::new(2000.0, 800.0)),  // Far sky
        (Color::srgba(0.05, 0.03, 0.1, 0.8), 0.3, -40.0, Vec2::new(2000.0, 600.0)),    // Far mountains
        (Color::srgba(0.08, 0.05, 0.15, 0.6), 0.5, -30.0, Vec2::new(2000.0, 500.0)),   // Mid hills
    ];

    for (color, speed, z, size) in layers {
        commands.spawn((
            Sprite {
                color,
                custom_size: Some(size),
                ..default()
            },
            Transform::from_translation(Vec3::new(0.0, 0.0, z)),
            ParallaxLayer { speed },
        ));
    }
}
