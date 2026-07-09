//! Enemy plugin — registers AI systems and provides spawn functions.

pub mod ai;

use avian2d::prelude::*;
use bevy::prelude::*;

use crate::components::*;
use crate::player::combat::DamageEvent;
use crate::states::GameState;
use ai::*;

/// Plugin managing all enemy systems.
pub struct EnemyPlugin;

impl Plugin for EnemyPlugin {
    fn build(&self, app: &mut App) {
        app.add_systems(
            Update,
            (
                ai_state_machine_system,
                enemy_patrol_system,
                enemy_chase_system,
                enemy_attack_system,
                enemy_hurt_system,
                enemy_death_system,
                enemy_flip_sprite_system,
            )
                .run_if(in_state(GameState::Playing)),
        );
    }
}

/// System: Update AI state machine transitions.
pub fn ai_state_machine_system(
    time: Res<Time>,
    player_q: Query<&Transform, With<Player>>,
    mut enemies: Query<(&Transform, &mut AiController, &Health), With<Enemy>>,
) {
    let dt = time.delta_secs();

    let Ok(player_tf) = player_q.single() else {
        return;
    };
    let player_pos = player_tf.translation.truncate();

    for (enemy_tf, mut ai, health) in &mut enemies {
        // Tick cooldowns
        ai.attack_cooldown = (ai.attack_cooldown - dt).max(0.0);
        ai.action_timer = (ai.action_timer - dt).max(0.0);
        ai.hurt_timer = (ai.hurt_timer - dt).max(0.0);

        if health.is_dead() {
            ai.state = AiState::Dead;
            continue;
        }

        if ai.hurt_timer > 0.0 {
            ai.state = AiState::Hurt;
            continue;
        }

        if ai.action_timer > 0.0 {
            continue; // Still in current action
        }

        let enemy_pos = enemy_tf.translation.truncate();
        let distance = enemy_pos.distance(player_pos);
        let dx = player_pos.x - enemy_pos.x;

        match ai.state {
            AiState::Patrol => {
                if distance < consts::DETECTION_RANGE {
                    ai.state = AiState::Detect;
                    ai.action_timer = 0.3; // Brief detection pause
                    ai.patrol_direction = dx.signum();
                }
            }
            AiState::Detect => {
                ai.state = AiState::Chase;
            }
            AiState::Chase => {
                if distance > consts::LOSE_INTEREST_RANGE {
                    ai.state = AiState::Patrol;
                } else if distance < consts::ATTACK_RANGE && ai.attack_cooldown <= 0.0 {
                    ai.state = AiState::Attack;
                    ai.action_timer = consts::ATTACK_DURATION;
                    ai.attack_cooldown = consts::ATTACK_COOLDOWN;
                }
                // Update facing toward player
                ai.patrol_direction = dx.signum();
            }
            AiState::Attack => {
                // Attack finished, return to chase
                ai.state = AiState::Chase;
            }
            AiState::Hurt => {
                ai.state = AiState::Chase;
            }
            AiState::Dead => {} // Handled by death system
        }
    }
}

/// System: Patrol — walk between waypoints.
pub fn enemy_patrol_system(
    time: Res<Time>,
    mut query: Query<(&mut Transform, &mut AiController), With<Enemy>>,
) {
    let dt = time.delta_secs();

    for (mut tf, mut ai) in &mut query {
        if ai.state != AiState::Patrol {
            continue;
        }

        tf.translation.x += ai.patrol_direction * consts::PATROL_SPEED * dt;

        // Reverse at patrol bounds
        if tf.translation.x <= ai.patrol_left {
            ai.patrol_direction = 1.0;
        } else if tf.translation.x >= ai.patrol_right {
            ai.patrol_direction = -1.0;
        }
    }
}

/// System: Chase — move toward player.
pub fn enemy_chase_system(
    time: Res<Time>,
    player_q: Query<&Transform, With<Player>>,
    mut enemies: Query<(&mut Transform, &AiController), (With<Enemy>, Without<Player>)>,
) {
    let dt = time.delta_secs();

    let Ok(player_tf) = player_q.single() else {
        return;
    };

    for (mut enemy_tf, ai) in &mut enemies {
        if ai.state != AiState::Chase {
            continue;
        }

        let dx = player_tf.translation.x - enemy_tf.translation.x;
        enemy_tf.translation.x += dx.signum() * consts::CHASE_SPEED * dt;
    }
}

/// System: Attack — fire damage event at player.
pub fn enemy_attack_system(
    query: Query<(&Transform, &AiController), With<Enemy>>,
    player_q: Query<(Entity, &Transform), With<Player>>,
    mut damage_events: MessageWriter<DamageEvent>,
) {
    let Ok((player_entity, player_tf)) = player_q.single() else {
        return;
    };

    for (enemy_tf, ai) in &query {
        if ai.state != AiState::Attack || ai.action_timer < consts::ATTACK_DURATION * 0.5 {
            continue; // Only deal damage at the start of the attack
        }

        let distance = (player_tf.translation.x - enemy_tf.translation.x).abs();
        if distance < consts::ATTACK_RANGE * 1.5 {
            let dir = (player_tf.translation.x - enemy_tf.translation.x).signum();
            damage_events.write(DamageEvent {
                target: player_entity,
                amount: consts::ATTACK_DAMAGE,
                knockback: Vec2::new(dir * consts::KNOCKBACK_FORCE, 50.0),
            });
        }
    }
}

/// System: React to taking damage.
pub fn enemy_hurt_system(
    mut damage_events: MessageReader<DamageEvent>,
    mut enemies: Query<(&mut AiController, &mut Health), With<Enemy>>,
) {
    for event in damage_events.read() {
        if let Ok((mut ai, _health)) = enemies.get_mut(event.target) {
            ai.hurt_timer = 0.3;
            ai.state = AiState::Hurt;
        }
    }
}

/// System: Handle enemy death — despawn after delay.
pub fn enemy_death_system(
    mut commands: Commands,
    time: Res<Time>,
    mut query: Query<(Entity, &AiController, &Health, &mut Sprite), With<Enemy>>,
) {
    for (entity, ai, _health, mut sprite) in &mut query {
        if ai.state == AiState::Dead {
            // Fade out effect
            let current_alpha = sprite.color.alpha();
            if current_alpha > 0.01 {
                sprite.color = sprite.color.with_alpha((current_alpha - time.delta_secs() * 2.0).max(0.0));
            } else {
                commands.entity(entity).despawn();
            }
        }
    }
}

/// System: Flip enemy sprite based on patrol/chase direction.
pub fn enemy_flip_sprite_system(
    mut query: Query<(&AiController, &mut Sprite), With<Enemy>>,
) {
    for (ai, mut sprite) in &mut query {
        sprite.flip_x = ai.patrol_direction < 0.0;
    }
}

/// Spawn an enemy entity.
pub fn spawn_enemy(
    commands: &mut Commands,
    position: Vec2,
    enemy_type: EnemyType,
    patrol_left: f32,
    patrol_right: f32,
) -> Entity {
    let (color, size, health_val) = match enemy_type {
        EnemyType::MeleeGrunt => (
            Color::srgb(0.9, 0.2, 0.2), // Red
            Vec2::new(28.0, 36.0),
            30.0,
        ),
        EnemyType::RangedArcher => (
            Color::srgb(0.2, 0.8, 0.2), // Green
            Vec2::new(24.0, 38.0),
            20.0,
        ),
        EnemyType::ShieldGuard => (
            Color::srgb(0.6, 0.6, 0.8), // Steel blue
            Vec2::new(32.0, 40.0),
            50.0,
        ),
    };

    commands
        .spawn((
            Sprite {
                color,
                custom_size: Some(size),
                ..default()
            },
            Transform::from_translation(position.extend(10.0)),
            RigidBody::Kinematic,
            Collider::rectangle(size.x, size.y),
            Enemy,
            enemy_type,
            Health::new(health_val),
            AiController::default().with_patrol_bounds(patrol_left, patrol_right),
            Hurtbox,
            Name::new("Enemy"),
        ))
        .id()
}
