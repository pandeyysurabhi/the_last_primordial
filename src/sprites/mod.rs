//! Sprite sheet pipeline — animation clips, controller, frame advancement.
//! Handles loading Aseprite-exported sprite sheets and animating them.

use bevy::prelude::*;

use crate::states::GameState;

/// Plugin managing sprite animations.
pub struct SpritePlugin;

impl Plugin for SpritePlugin {
    fn build(&self, app: &mut App) {
        app.add_systems(
            Update,
            animate_sprites_system.run_if(in_state(GameState::Playing)),
        );
    }
}

/// Defines a single animation clip (a sequence of frames).
#[derive(Debug, Clone)]
pub struct AnimationClip {
    /// Name identifier (e.g., "idle", "walk", "attack_light").
    pub name: String,
    /// Indices into the TextureAtlas.
    pub frames: Vec<usize>,
    /// Seconds per frame.
    pub frame_duration: f32,
    /// How the animation loops.
    pub loop_mode: LoopMode,
}

/// Animation loop behavior.
#[derive(Debug, Clone, Copy, PartialEq, Eq)]
pub enum LoopMode {
    /// Loops forever.
    Loop,
    /// Plays once and stays on last frame.
    Once,
    /// Plays forward then backward.
    PingPong,
}

/// Component that drives sprite animation.
#[derive(Component, Debug)]
pub struct AnimationController {
    /// Current clip being played.
    pub current_clip: String,
    /// All available clips for this entity.
    pub clips: Vec<AnimationClip>,
    /// Timer for frame advancement.
    pub timer: f32,
    /// Current frame index within the clip.
    pub frame_index: usize,
    /// PingPong direction: true = forward, false = backward.
    pub ping_pong_forward: bool,
    /// Whether the animation has finished (for Once mode).
    pub finished: bool,
}

impl AnimationController {
    /// Create a new controller with the given clips.
    pub fn new(clips: Vec<AnimationClip>) -> Self {
        let default_name = clips.first().map(|c| c.name.clone()).unwrap_or_default();
        Self {
            current_clip: default_name,
            clips,
            timer: 0.0,
            frame_index: 0,
            ping_pong_forward: true,
            finished: false,
        }
    }

    /// Switch to a different animation clip.
    pub fn play(&mut self, clip_name: &str) {
        if self.current_clip == clip_name {
            return; // Already playing
        }
        self.current_clip = clip_name.to_string();
        self.frame_index = 0;
        self.timer = 0.0;
        self.ping_pong_forward = true;
        self.finished = false;
    }

    /// Get the currently active clip.
    pub fn active_clip(&self) -> Option<&AnimationClip> {
        self.clips.iter().find(|c| c.name == self.current_clip)
    }

    /// Get the current atlas frame index.
    pub fn current_atlas_index(&self) -> Option<usize> {
        self.active_clip()
            .and_then(|clip| clip.frames.get(self.frame_index))
            .copied()
    }
}

/// System: Advance animation frames based on timer.
pub fn animate_sprites_system(
    time: Res<Time>,
    mut query: Query<&mut AnimationController>,
) {
    let dt = time.delta_secs();

    for mut controller in &mut query {
        if controller.finished {
            continue;
        }

        let Some(clip) = controller.clips.iter().find(|c| c.name == controller.current_clip).cloned() else {
            continue;
        };

        controller.timer += dt;

        if controller.timer >= clip.frame_duration {
            controller.timer -= clip.frame_duration;

            match clip.loop_mode {
                LoopMode::Loop => {
                    controller.frame_index = (controller.frame_index + 1) % clip.frames.len();
                }
                LoopMode::Once => {
                    if controller.frame_index < clip.frames.len() - 1 {
                        controller.frame_index += 1;
                    } else {
                        controller.finished = true;
                    }
                }
                LoopMode::PingPong => {
                    if controller.ping_pong_forward {
                        if controller.frame_index < clip.frames.len() - 1 {
                            controller.frame_index += 1;
                        } else {
                            controller.ping_pong_forward = false;
                            if controller.frame_index > 0 {
                                controller.frame_index -= 1;
                            }
                        }
                    } else {
                        if controller.frame_index > 0 {
                            controller.frame_index -= 1;
                        } else {
                            controller.ping_pong_forward = true;
                            controller.frame_index += 1;
                        }
                    }
                }
            }
        }
    }
}

/// Helper: Create placeholder animation clips for the player.
/// (Used until real sprite sheets are available.)
pub fn placeholder_player_clips() -> Vec<AnimationClip> {
    vec![
        AnimationClip {
            name: "idle".to_string(),
            frames: vec![0, 1, 2, 3, 2, 1],
            frame_duration: 0.15,
            loop_mode: LoopMode::Loop,
        },
        AnimationClip {
            name: "walk".to_string(),
            frames: vec![4, 5, 6, 7, 8, 9, 10, 11],
            frame_duration: 0.1,
            loop_mode: LoopMode::Loop,
        },
        AnimationClip {
            name: "run".to_string(),
            frames: vec![12, 13, 14, 15, 16, 17, 18, 19],
            frame_duration: 0.08,
            loop_mode: LoopMode::Loop,
        },
        AnimationClip {
            name: "jump".to_string(),
            frames: vec![20, 21, 22, 23],
            frame_duration: 0.1,
            loop_mode: LoopMode::Once,
        },
        AnimationClip {
            name: "fall".to_string(),
            frames: vec![24, 25],
            frame_duration: 0.12,
            loop_mode: LoopMode::Loop,
        },
        AnimationClip {
            name: "attack_light".to_string(),
            frames: vec![26, 27, 28, 29, 30, 31],
            frame_duration: 0.04,
            loop_mode: LoopMode::Once,
        },
        AnimationClip {
            name: "attack_heavy".to_string(),
            frames: vec![32, 33, 34, 35, 36, 37, 38, 39, 40, 41],
            frame_duration: 0.05,
            loop_mode: LoopMode::Once,
        },
        AnimationClip {
            name: "dodge".to_string(),
            frames: vec![42, 43, 44, 45, 46, 47],
            frame_duration: 0.05,
            loop_mode: LoopMode::Once,
        },
        AnimationClip {
            name: "hurt".to_string(),
            frames: vec![48, 49, 50],
            frame_duration: 0.1,
            loop_mode: LoopMode::Once,
        },
        AnimationClip {
            name: "death".to_string(),
            frames: vec![51, 52, 53, 54, 55, 56, 57, 58],
            frame_duration: 0.12,
            loop_mode: LoopMode::Once,
        },
    ]
}
