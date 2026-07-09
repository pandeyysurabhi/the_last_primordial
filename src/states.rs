//! Game states for The Last Primordial.
//! Controls which systems run at any given time.

use bevy::prelude::*;

/// Top-level game state controlling major game flow.
#[derive(States, Debug, Clone, Copy, PartialEq, Eq, Hash, Default)]
pub enum GameState {
    /// Main menu screen
    #[default]
    MainMenu,
    /// Loading assets and level data
    Loading,
    /// Active gameplay
    Playing,
    /// Game is paused (overlay menu)
    Paused,
    /// Dialogue/cutscene is active — gameplay frozen
    Dialogue,
    /// Cutscene playing
    Cutscene,
}
