//! Enemy AI — state machine with patrol, detect, chase, attack behaviors.

use bevy::prelude::*;


/// AI behaviour constants.
pub mod consts {
    pub const DETECTION_RANGE: f32 = 200.0;
    pub const ATTACK_RANGE: f32 = 35.0;
    pub const CHASE_SPEED: f32 = 100.0;
    pub const PATROL_SPEED: f32 = 50.0;
    pub const ATTACK_COOLDOWN: f32 = 1.2;
    pub const ATTACK_DAMAGE: f32 = 8.0;
    pub const ATTACK_DURATION: f32 = 0.4;
    pub const LOSE_INTEREST_RANGE: f32 = 350.0;
    pub const KNOCKBACK_FORCE: f32 = 150.0;
}

/// AI state machine states.
#[derive(Component, Debug, Clone, Copy, PartialEq, Eq)]
pub enum AiState {
    /// Walking between waypoints.
    Patrol,
    /// Player detected — turning toward them.
    Detect,
    /// Moving toward player.
    Chase,
    /// Executing an attack.
    Attack,
    /// Taking damage reaction.
    Hurt,
    /// Dead — playing death animation.
    Dead,
}

impl Default for AiState {
    fn default() -> Self {
        AiState::Patrol
    }
}

/// Enemy type determines behavior variations.
#[derive(Component, Debug, Clone, Copy)]
pub enum EnemyType {
    /// Close-range melee fighter.
    MeleeGrunt,
    /// Keeps distance, fires projectiles.
    RangedArcher,
    /// Blocks frontal attacks.
    ShieldGuard,
}

/// AI controller with state and timing.
#[derive(Component, Debug)]
pub struct AiController {
    pub state: AiState,
    pub attack_cooldown: f32,
    pub action_timer: f32,
    /// Patrol: left and right x-bounds.
    pub patrol_left: f32,
    pub patrol_right: f32,
    pub patrol_direction: f32,
    pub hurt_timer: f32,
}

impl Default for AiController {
    fn default() -> Self {
        Self {
            state: AiState::Patrol,
            attack_cooldown: 0.0,
            action_timer: 0.0,
            patrol_left: -100.0,
            patrol_right: 100.0,
            patrol_direction: 1.0,
            hurt_timer: 0.0,
        }
    }
}

impl AiController {
    pub fn with_patrol_bounds(mut self, left: f32, right: f32) -> Self {
        self.patrol_left = left;
        self.patrol_right = right;
        self
    }
}
