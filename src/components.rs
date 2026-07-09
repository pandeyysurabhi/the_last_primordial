//! Shared ECS components used across multiple systems.

use bevy::prelude::*;
use serde::{Deserialize, Serialize};

// ── Health & Resources ──────────────────────────────────────────────

/// Health component for any damageable entity.
#[derive(Component, Debug, Clone, Serialize, Deserialize)]
pub struct Health {
    pub current: f32,
    pub maximum: f32,
}

impl Health {
    pub fn new(max: f32) -> Self {
        Self {
            current: max,
            maximum: max,
        }
    }

    pub fn take_damage(&mut self, amount: f32) {
        self.current = (self.current - amount).max(0.0);
    }

    pub fn heal(&mut self, amount: f32) {
        self.current = (self.current + amount).min(self.maximum);
    }

    pub fn is_dead(&self) -> bool {
        self.current <= 0.0
    }

    pub fn fraction(&self) -> f32 {
        if self.maximum > 0.0 {
            self.current / self.maximum
        } else {
            0.0
        }
    }
}

/// Stamina resource for Kael (dodge, heavy attacks).
#[derive(Component, Debug, Clone, Serialize, Deserialize)]
pub struct Stamina {
    pub current: f32,
    pub maximum: f32,
    pub regen_rate: f32,
}

impl Stamina {
    pub fn new(max: f32) -> Self {
        Self {
            current: max,
            maximum: max,
            regen_rate: 20.0,
        }
    }

    pub fn consume(&mut self, amount: f32) -> bool {
        if self.current >= amount {
            self.current -= amount;
            true
        } else {
            false
        }
    }

    pub fn regenerate(&mut self, dt: f32) {
        self.current = (self.current + self.regen_rate * dt).min(self.maximum);
    }

    pub fn fraction(&self) -> f32 {
        if self.maximum > 0.0 {
            self.current / self.maximum
        } else {
            0.0
        }
    }
}

// ── Movement & Physics ──────────────────────────────────────────────

/// Direction the entity is facing.
#[derive(Component, Debug, Clone, Copy, PartialEq, Eq, Serialize, Deserialize)]
pub enum Facing {
    Left,
    Right,
}

impl Default for Facing {
    fn default() -> Self {
        Facing::Right
    }
}

/// Whether the entity is standing on ground.
#[derive(Component, Debug, Clone, Copy, Default)]
pub struct Grounded(pub bool);

// ── Combat ──────────────────────────────────────────────────────────

/// Damage value carried by a hitbox entity.
#[derive(Component, Debug, Clone)]
pub struct Damage {
    pub amount: f32,
    pub knockback: Vec2,
}

/// I-frames (invincibility frames) after taking damage.
#[derive(Component, Debug)]
pub struct Invincibility {
    pub timer: Timer,
}

impl Invincibility {
    pub fn new(duration: f32) -> Self {
        Self {
            timer: Timer::from_seconds(duration, TimerMode::Once),
        }
    }
}

/// Marks an entity as a hitbox (attack sensor).
#[derive(Component, Debug)]
pub struct Hitbox {
    pub lifetime: Timer,
}

/// Marks an entity as a hurtbox (can receive damage).
#[derive(Component, Debug)]
pub struct Hurtbox;

// ── Tags ────────────────────────────────────────────────────────────

/// Marks the player entity.
#[derive(Component, Debug)]
pub struct Player;

/// Marks an enemy entity.
#[derive(Component, Debug)]
pub struct Enemy;

/// Marks an NPC entity.
#[derive(Component, Debug)]
pub struct Npc;

/// Marks a wall/platform collision entity.
#[derive(Component, Debug)]
pub struct Wall;

/// Marks a ground/floor collision entity.
#[derive(Component, Debug)]
pub struct Ground;
