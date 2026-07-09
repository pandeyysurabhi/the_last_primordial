//! Player movement system — walk, run, jump, dash, wall slide, ledge grab.
//! Uses Avian2D physics with KinematicCharacterController.

use avian2d::prelude::*;
use bevy::prelude::*;

use crate::components::*;

/// Player movement constants.
pub mod consts {
    pub const MOVE_SPEED: f32 = 150.0;
    pub const RUN_SPEED: f32 = 220.0;
    pub const JUMP_FORCE: f32 = 380.0;
    pub const DASH_SPEED: f32 = 400.0;
    pub const DASH_DURATION: f32 = 0.15;
    pub const DASH_COOLDOWN: f32 = 0.5;
    pub const GRAVITY: f32 = -980.0;
    pub const MAX_FALL_SPEED: f32 = -600.0;
    pub const COYOTE_TIME: f32 = 0.1;
    pub const JUMP_BUFFER_TIME: f32 = 0.1;
    pub const WALL_SLIDE_SPEED: f32 = -60.0;
    pub const WALL_JUMP_FORCE: bevy::prelude::Vec2 = bevy::prelude::Vec2::new(250.0, 350.0);
}

/// Current movement state of the player.
#[derive(Component, Debug, Clone, Copy, PartialEq, Eq)]
pub enum MoveState {
    Idle,
    Walking,
    Running,
    Jumping,
    Falling,
    Dashing,
    WallSlide,
    LedgeGrab,
}

impl Default for MoveState {
    fn default() -> Self {
        MoveState::Idle
    }
}

/// Player movement data tracked per-frame.
#[derive(Component, Debug)]
pub struct PlayerMovement {
    pub velocity: Vec2,
    pub state: MoveState,
    pub coyote_timer: f32,
    pub jump_buffer_timer: f32,
    pub dash_timer: f32,
    pub dash_cooldown_timer: f32,
    pub can_double_jump: bool,
    pub wall_direction: f32, // -1.0 left, 1.0 right, 0.0 none
}

impl Default for PlayerMovement {
    fn default() -> Self {
        Self {
            velocity: Vec2::ZERO,
            state: MoveState::Idle,
            coyote_timer: 0.0,
            jump_buffer_timer: 0.0,
            dash_timer: 0.0,
            dash_cooldown_timer: 0.0,
            can_double_jump: true,
            wall_direction: 0.0,
        }
    }
}

/// System: Read keyboard input and update player velocity / state.
pub fn player_movement_system(
    keyboard: Res<ButtonInput<KeyCode>>,
    time: Res<Time>,
    mut query: Query<(
        &mut PlayerMovement,
        &mut Facing,
        &Grounded,
    ), With<Player>>,
) {
    let dt = time.delta_secs();

    for (mut movement, mut facing, grounded) in &mut query {
        let was_grounded = grounded.0;

        // ── Tick timers ──
        movement.coyote_timer = (movement.coyote_timer - dt).max(0.0);
        movement.jump_buffer_timer = (movement.jump_buffer_timer - dt).max(0.0);
        movement.dash_cooldown_timer = (movement.dash_cooldown_timer - dt).max(0.0);

        // ── Coyote time: allow brief jump window after leaving ground ──
        if was_grounded {
            movement.coyote_timer = consts::COYOTE_TIME;
            movement.can_double_jump = true;
        }

        // ── Dashing ──
        if movement.dash_timer > 0.0 {
            movement.dash_timer -= dt;
            if movement.dash_timer <= 0.0 {
                movement.state = if was_grounded {
                    MoveState::Idle
                } else {
                    MoveState::Falling
                };
            }
            // While dashing, maintain dash velocity — skip normal movement
            continue;
        }

        // ── Horizontal movement ──
        let mut move_x = 0.0;
        if keyboard.pressed(KeyCode::KeyA) || keyboard.pressed(KeyCode::ArrowLeft) {
            move_x -= 1.0;
        }
        if keyboard.pressed(KeyCode::KeyD) || keyboard.pressed(KeyCode::ArrowRight) {
            move_x += 1.0;
        }

        let running = keyboard.pressed(KeyCode::ShiftLeft) || keyboard.pressed(KeyCode::ShiftRight);
        let speed = if running { consts::RUN_SPEED } else { consts::MOVE_SPEED };
        movement.velocity.x = move_x * speed;

        // ── Update facing direction ──
        if move_x < 0.0 {
            *facing = Facing::Left;
        } else if move_x > 0.0 {
            *facing = Facing::Right;
        }

        // ── Jump buffer ──
        if keyboard.just_pressed(KeyCode::Space) {
            movement.jump_buffer_timer = consts::JUMP_BUFFER_TIME;
        }

        // ── Jump (with coyote time + jump buffering) ──
        let can_jump = movement.coyote_timer > 0.0 || was_grounded;
        if movement.jump_buffer_timer > 0.0 && can_jump {
            movement.velocity.y = consts::JUMP_FORCE;
            movement.coyote_timer = 0.0;
            movement.jump_buffer_timer = 0.0;
            movement.state = MoveState::Jumping;
        }

        // ── Variable jump height (release early = lower jump) ──
        if keyboard.just_released(KeyCode::Space) && movement.velocity.y > 0.0 {
            movement.velocity.y *= 0.5;
        }

        // ── Gravity ──
        if !was_grounded {
            movement.velocity.y += consts::GRAVITY * dt;
            movement.velocity.y = movement.velocity.y.max(consts::MAX_FALL_SPEED);
        } else if movement.velocity.y < 0.0 {
            movement.velocity.y = 0.0;
        }

        // ── Dash ──
        if keyboard.just_pressed(KeyCode::ControlLeft) && movement.dash_cooldown_timer <= 0.0 {
            let dash_dir = match *facing {
                Facing::Right => 1.0,
                Facing::Left => -1.0,
            };
            movement.velocity = Vec2::new(dash_dir * consts::DASH_SPEED, 0.0);
            movement.dash_timer = consts::DASH_DURATION;
            movement.dash_cooldown_timer = consts::DASH_COOLDOWN;
            movement.state = MoveState::Dashing;
        }

        // ── Determine state ──
        if movement.state != MoveState::Dashing {
            if was_grounded {
                if move_x.abs() > 0.0 {
                    movement.state = if running {
                        MoveState::Running
                    } else {
                        MoveState::Walking
                    };
                } else {
                    movement.state = MoveState::Idle;
                }
            } else if movement.velocity.y > 0.0 {
                movement.state = MoveState::Jumping;
            } else {
                movement.state = MoveState::Falling;
            }
        }
    }
}

/// System: Apply player velocity to physics via translation (kinematic).
pub fn apply_player_velocity(
    time: Res<Time>,
    mut query: Query<(&PlayerMovement, &mut Transform), With<Player>>,
) {
    let dt = time.delta_secs();
    for (movement, mut transform) in &mut query {
        transform.translation.x += movement.velocity.x * dt;
        transform.translation.y += movement.velocity.y * dt;
    }
}

/// System: Check if player is grounded using a short downward raycast.
pub fn ground_detection_system(
    spatial_query: SpatialQuery,
    mut query: Query<(Entity, &Transform, &mut Grounded), With<Player>>,
) {
    for (entity, transform, mut grounded) in &mut query {
        // Cast from the bottom of the player sprite (half height = 24px)
        let origin = Vec2::new(
            transform.translation.x,
            transform.translation.y - 24.0,
        );
        let direction = Dir2::NEG_Y;
        let max_distance = 6.0; // small distance below feet

        let mut filter = SpatialQueryFilter::default();
        filter.excluded_entities.insert(entity);

        let hit = spatial_query.cast_ray(
            origin,
            direction,
            max_distance,
            true,
            &filter,
        );

        grounded.0 = hit.is_some();
    }
}

/// System: Flip sprite based on facing direction.
pub fn flip_sprite_system(
    mut query: Query<(&Facing, &mut Sprite), (With<Player>, Changed<Facing>)>,
) {
    for (facing, mut sprite) in &mut query {
        sprite.flip_x = *facing == Facing::Left;
    }
}
