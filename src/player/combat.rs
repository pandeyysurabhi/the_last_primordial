//! Player combat system — light/heavy attack, dodge roll, parry, hitbox/hurtbox.

use bevy::prelude::*;

use crate::components::*;

/// Combat-specific constants.
pub mod consts {
    pub const LIGHT_ATTACK_DAMAGE: f32 = 10.0;
    pub const HEAVY_ATTACK_DAMAGE: f32 = 25.0;
    pub const LIGHT_ATTACK_RANGE: f32 = 28.0;
    pub const HEAVY_ATTACK_RANGE: f32 = 36.0;
    pub const LIGHT_ATTACK_DURATION: f32 = 0.25;
    pub const HEAVY_ATTACK_DURATION: f32 = 0.5;
    pub const DODGE_DURATION: f32 = 0.3;
    pub const DODGE_SPEED: f32 = 350.0;
    pub const DODGE_STAMINA_COST: f32 = 15.0;
    pub const HEAVY_STAMINA_COST: f32 = 20.0;
    pub const PARRY_WINDOW: f32 = 0.15;
    pub const I_FRAME_DURATION: f32 = 0.5;
    pub const KNOCKBACK_FORCE: f32 = 200.0;
    pub const COMBO_WINDOW: f32 = 0.4;
}

/// Player combat state.
#[derive(Component, Debug, Clone, PartialEq, Eq)]
pub enum CombatState {
    Idle,
    LightAttack,
    HeavyAttack,
    Dodging,
    Blocking,
    Parrying,
}

impl Default for CombatState {
    fn default() -> Self {
        CombatState::Idle
    }
}

/// Tracks combat timing and combos.
#[derive(Component, Debug)]
pub struct CombatController {
    pub state: CombatState,
    pub action_timer: f32,
    pub combo_count: u32,
    pub combo_timer: f32,
    pub attack_connected: bool,
}

impl Default for CombatController {
    fn default() -> Self {
        Self {
            state: CombatState::Idle,
            action_timer: 0.0,
            combo_count: 0,
            combo_timer: 0.0,
            attack_connected: false,
        }
    }
}

/// Event fired when player attacks.
#[derive(Message, Debug)]
pub struct AttackEvent {
    pub attacker: Entity,
    pub damage: f32,
    pub range: f32,
    pub direction: f32, // -1.0 or 1.0
    pub knockback: Vec2,
}

/// Event fired when an entity takes damage.
#[derive(Message, Debug)]
pub struct DamageEvent {
    pub target: Entity,
    pub amount: f32,
    pub knockback: Vec2,
}

/// System: Handle combat input — attack, dodge, parry.
pub fn combat_input_system(
    keyboard: Res<ButtonInput<KeyCode>>,
    mouse: Res<ButtonInput<MouseButton>>,
    time: Res<Time>,
    mut query: Query<(
        &mut CombatController,
        &mut Stamina,
        &Facing,
    ), With<Player>>,
    mut attack_events: MessageWriter<AttackEvent>,
    player_entity_q: Query<Entity, With<Player>>,
) {
    let dt = time.delta_secs();

    for (mut combat, mut stamina, facing) in &mut query {
        // Tick timers
        combat.action_timer = (combat.action_timer - dt).max(0.0);
        combat.combo_timer = (combat.combo_timer - dt).max(0.0);

        // Reset combo if window expired
        if combat.combo_timer <= 0.0 {
            combat.combo_count = 0;
        }

        // If in action, wait for it to finish
        if combat.action_timer > 0.0 {
            continue;
        }

        // Return to idle when action completes
        if combat.state != CombatState::Idle {
            combat.state = CombatState::Idle;
        }

        let dir = match *facing {
            Facing::Right => 1.0,
            Facing::Left => -1.0,
        };

        // Light attack (left click or J)
        if mouse.just_pressed(MouseButton::Left) || keyboard.just_pressed(KeyCode::KeyJ) {
            combat.state = CombatState::LightAttack;
            combat.action_timer = consts::LIGHT_ATTACK_DURATION;
            combat.combo_count += 1;
            combat.combo_timer = consts::COMBO_WINDOW;

            // Combo bonus: third hit does extra damage
            let damage = if combat.combo_count >= 3 {
                combat.combo_count = 0;
                consts::LIGHT_ATTACK_DAMAGE * 1.5
            } else {
                consts::LIGHT_ATTACK_DAMAGE
            };

            if let Ok(entity) = player_entity_q.single() {
                attack_events.write(AttackEvent {
                    attacker: entity,
                    damage,
                    range: consts::LIGHT_ATTACK_RANGE,
                    direction: dir,
                    knockback: Vec2::new(dir * consts::KNOCKBACK_FORCE, 50.0),
                });
            }
        }

        // Heavy attack (right click or K) — costs stamina
        if (mouse.just_pressed(MouseButton::Right) || keyboard.just_pressed(KeyCode::KeyK))
            && stamina.consume(consts::HEAVY_STAMINA_COST)
        {
            combat.state = CombatState::HeavyAttack;
            combat.action_timer = consts::HEAVY_ATTACK_DURATION;
            combat.combo_count = 0;

            if let Ok(entity) = player_entity_q.single() {
                attack_events.write(AttackEvent {
                    attacker: entity,
                    damage: consts::HEAVY_ATTACK_DAMAGE,
                    range: consts::HEAVY_ATTACK_RANGE,
                    direction: dir,
                    knockback: Vec2::new(dir * consts::KNOCKBACK_FORCE * 1.5, 80.0),
                });
            }
        }

        // Dodge roll (Shift when not running) — costs stamina
        if keyboard.just_pressed(KeyCode::ShiftLeft)
            && combat.state == CombatState::Idle
            && stamina.consume(consts::DODGE_STAMINA_COST)
        {
            combat.state = CombatState::Dodging;
            combat.action_timer = consts::DODGE_DURATION;
        }

        // Block/Parry (Ctrl)
        if keyboard.just_pressed(KeyCode::ControlRight) {
            combat.state = CombatState::Parrying;
            combat.action_timer = consts::PARRY_WINDOW;
        } else if keyboard.pressed(KeyCode::ControlRight) && combat.state != CombatState::Parrying {
            combat.state = CombatState::Blocking;
        }
    }
}

/// System: Resolve attack events — check hit against enemies.
pub fn resolve_attacks_system(
    mut attack_events: MessageReader<AttackEvent>,
    mut damage_events: MessageWriter<DamageEvent>,
    attacker_q: Query<&Transform>,
    enemies: Query<(Entity, &Transform), (With<Enemy>, Without<Player>)>,
) {
    for event in attack_events.read() {
        let Ok(attacker_tf) = attacker_q.get(event.attacker) else {
            continue;
        };

        for (enemy_entity, enemy_tf) in &enemies {
            let diff = enemy_tf.translation.x - attacker_tf.translation.x;
            let distance = diff.abs();
            let same_direction = (diff > 0.0 && event.direction > 0.0)
                || (diff < 0.0 && event.direction < 0.0);

            // Check if enemy is in range and in attack direction
            if distance <= event.range && same_direction {
                let y_diff = (enemy_tf.translation.y - attacker_tf.translation.y).abs();
                if y_diff < 32.0 {
                    damage_events.write(DamageEvent {
                        target: enemy_entity,
                        amount: event.damage,
                        knockback: event.knockback,
                    });
                }
            }
        }
    }
}

/// System: Apply damage to entities.
pub fn apply_damage_system(
    mut commands: Commands,
    mut damage_events: MessageReader<DamageEvent>,
    mut targets: Query<&mut Health>,
    invincible: Query<&Invincibility>,
) {
    for event in damage_events.read() {
        // Skip if target has i-frames
        if invincible.get(event.target).is_ok() {
            continue;
        }

        if let Ok(mut health) = targets.get_mut(event.target) {
            health.take_damage(event.amount);

            // Grant i-frames
            commands
                .entity(event.target)
                .insert(Invincibility::new(consts::I_FRAME_DURATION));
        }
    }
}

/// System: Tick invincibility timers and remove when done.
pub fn invincibility_system(
    mut commands: Commands,
    time: Res<Time>,
    mut query: Query<(Entity, &mut Invincibility)>,
) {
    for (entity, mut inv) in &mut query {
        inv.timer.tick(time.delta());
        if inv.timer.is_finished() {
            commands.entity(entity).remove::<Invincibility>();
        }
    }
}

/// System: Regenerate stamina over time.
pub fn stamina_regen_system(time: Res<Time>, mut query: Query<&mut Stamina, With<Player>>) {
    let dt = time.delta_secs();
    for mut stamina in &mut query {
        stamina.regenerate(dt);
    }
}
