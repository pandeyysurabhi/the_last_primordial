//! Level loading — programmatic test level with platforms and collision.
//! Will integrate LDtk when tileset art is ready.

use avian2d::prelude::*;
use bevy::prelude::*;

use crate::components::*;
use crate::enemy::{self, ai::EnemyType};
use crate::player;
use crate::camera;
use crate::states::GameState;

/// Level plugin.
pub struct LevelPlugin;

impl Plugin for LevelPlugin {
    fn build(&self, app: &mut App) {
        app.add_systems(OnEnter(GameState::Playing), setup_test_level);
    }
}

/// Platform definition for level building.
struct PlatformDef {
    position: Vec2,
    size: Vec2,
    color: Color,
}

/// System: Build the test level with platforms, walls, and enemies.
fn setup_test_level(mut commands: Commands) {
    // ── Parallax backgrounds ──
    camera::spawn_parallax_backgrounds(&mut commands);

    // ── Ground platforms ──
    let platforms = vec![
        // Main ground
        PlatformDef {
            position: Vec2::new(0.0, -100.0),
            size: Vec2::new(1200.0, 32.0),
            color: Color::srgb(0.15, 0.12, 0.2),
        },
        // Left wall
        PlatformDef {
            position: Vec2::new(-616.0, 0.0),
            size: Vec2::new(32.0, 400.0),
            color: Color::srgb(0.12, 0.1, 0.18),
        },
        // Right wall
        PlatformDef {
            position: Vec2::new(616.0, 0.0),
            size: Vec2::new(32.0, 400.0),
            color: Color::srgb(0.12, 0.1, 0.18),
        },
        // Floating platform 1 (left)
        PlatformDef {
            position: Vec2::new(-250.0, 0.0),
            size: Vec2::new(160.0, 16.0),
            color: Color::srgb(0.2, 0.15, 0.28),
        },
        // Floating platform 2 (center high)
        PlatformDef {
            position: Vec2::new(0.0, 80.0),
            size: Vec2::new(120.0, 16.0),
            color: Color::srgb(0.2, 0.15, 0.28),
        },
        // Floating platform 3 (right)
        PlatformDef {
            position: Vec2::new(250.0, -20.0),
            size: Vec2::new(180.0, 16.0),
            color: Color::srgb(0.2, 0.15, 0.28),
        },
        // Stepped platforms (right side staircase)
        PlatformDef {
            position: Vec2::new(380.0, -50.0),
            size: Vec2::new(80.0, 16.0),
            color: Color::srgb(0.18, 0.14, 0.25),
        },
        PlatformDef {
            position: Vec2::new(440.0, 0.0),
            size: Vec2::new(80.0, 16.0),
            color: Color::srgb(0.18, 0.14, 0.25),
        },
        PlatformDef {
            position: Vec2::new(500.0, 50.0),
            size: Vec2::new(80.0, 16.0),
            color: Color::srgb(0.18, 0.14, 0.25),
        },
    ];

    for plat in &platforms {
        spawn_platform(&mut commands, plat.position, plat.size, plat.color);
    }

    // ── Spawn player ──
    player::spawn_player(&mut commands, Vec2::new(-300.0, -50.0));

    // ── Spawn enemies ──
    enemy::spawn_enemy(
        &mut commands,
        Vec2::new(100.0, -70.0),
        EnemyType::MeleeGrunt,
        50.0,
        200.0,
    );

    enemy::spawn_enemy(
        &mut commands,
        Vec2::new(300.0, -70.0),
        EnemyType::RangedArcher,
        250.0,
        400.0,
    );

    enemy::spawn_enemy(
        &mut commands,
        Vec2::new(-100.0, -70.0),
        EnemyType::ShieldGuard,
        -200.0,
        -50.0,
    );

    // ── Decorative elements ──
    // Glowing sword indicator (bottom right of level)
    commands.spawn((
        Sprite {
            color: Color::srgba(0.6, 0.4, 1.0, 0.3),
            custom_size: Some(Vec2::new(8.0, 40.0)),
            ..default()
        },
        Transform::from_translation(Vec3::new(500.0, -55.0, 5.0)),
        Name::new("SwordGlow"),
    ));
}

/// Spawn a static platform with collision.
fn spawn_platform(commands: &mut Commands, position: Vec2, size: Vec2, color: Color) {
    commands.spawn((
        Sprite {
            color,
            custom_size: Some(size),
            ..default()
        },
        Transform::from_translation(position.extend(0.0)),
        RigidBody::Static,
        Collider::rectangle(size.x, size.y),
        Ground,
        Name::new("Platform"),
    ));
}
