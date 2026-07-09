//! Player plugin — registers all player-related systems.

pub mod combat;
pub mod movement;

use avian2d::prelude::*;
use bevy::prelude::*;

use crate::components::*;
use crate::states::GameState;
use combat::*;
use movement::*;

/// Plugin that manages all player systems.
pub struct PlayerPlugin;

impl Plugin for PlayerPlugin {
    fn build(&self, app: &mut App) {
        app.add_message::<AttackEvent>()
            .add_message::<DamageEvent>()
            .add_systems(
                Update,
                (
                    player_movement_system,
                    ground_detection_system,
                    apply_player_velocity,
                    flip_sprite_system,
                    combat_input_system,
                    resolve_attacks_system,
                    apply_damage_system,
                    invincibility_system,
                    stamina_regen_system,
                )
                    .run_if(in_state(GameState::Playing)),
            );
    }
}

/// Spawn the player entity with all required components.
pub fn spawn_player(commands: &mut Commands, position: Vec2) -> Entity {
    commands
        .spawn((
            // Rendering
            Sprite {
                color: Color::srgb(0.2, 0.6, 1.0), // Placeholder blue
                custom_size: Some(Vec2::new(32.0, 48.0)),
                ..default()
            },
            Transform::from_translation(position.extend(10.0)),
            // Physics
            RigidBody::Kinematic,
            Collider::rectangle(32.0, 48.0),
            // Game components
            Player,
            Health::new(100.0),
            Stamina::new(100.0),
            Facing::default(),
            Grounded::default(),
            PlayerMovement::default(),
            CombatController::default(),
            Hurtbox,
            Name::new("Kael"),
        ))
        .id()
}
