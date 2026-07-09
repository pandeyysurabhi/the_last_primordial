//! Audio engine integration — music, SFX, ambient channels via bevy_kira_audio.

use bevy::prelude::*;
use bevy_kira_audio::prelude::*;


/// Plugin managing all game audio.
pub struct GameAudioPlugin;

impl Plugin for GameAudioPlugin {
    fn build(&self, app: &mut App) {
        app.add_plugins(AudioPlugin)
            .add_audio_channel::<MusicChannel>()
            .add_audio_channel::<SfxChannel>()
            .add_audio_channel::<AmbientChannel>()
            .init_resource::<AudioSettings>()
            .add_systems(
                Update,
                apply_volume_settings,
            );
    }
}

/// Typed audio channel for background music.
#[derive(Resource)]
pub struct MusicChannel;

/// Typed audio channel for sound effects.
#[derive(Resource)]
pub struct SfxChannel;

/// Typed audio channel for ambient sounds.
#[derive(Resource)]
pub struct AmbientChannel;

/// Global audio settings.
#[derive(Resource, Debug)]
pub struct AudioSettings {
    pub master_volume: f32,
    pub music_volume: f32,
    pub sfx_volume: f32,
    pub ambient_volume: f32,
    pub dialogue_volume: f32,
}

impl Default for AudioSettings {
    fn default() -> Self {
        Self {
            master_volume: 1.0,
            music_volume: 0.7,
            sfx_volume: 0.8,
            ambient_volume: 0.5,
            dialogue_volume: 1.0,
        }
    }
}

/// System: Apply volume settings to audio channels.
pub fn apply_volume_settings(
    settings: Res<AudioSettings>,
    music_channel: Res<AudioChannel<MusicChannel>>,
    sfx_channel: Res<AudioChannel<SfxChannel>>,
    ambient_channel: Res<AudioChannel<AmbientChannel>>,
) {
    if settings.is_changed() {
        music_channel.set_volume(settings.master_volume * settings.music_volume);
        sfx_channel.set_volume(settings.master_volume * settings.sfx_volume);
        ambient_channel.set_volume(settings.master_volume * settings.ambient_volume);
    }
}

/// Helper: Play a music track with crossfade.
pub fn play_music(
    music_channel: &AudioChannel<MusicChannel>,
    asset_server: &AssetServer,
    track_path: String,
) {
    // Stop current track with fade
    music_channel.stop().fade_out(AudioTween::linear(std::time::Duration::from_secs(2)));

    // Play new track with fade in
    let handle = asset_server.load(track_path);
    music_channel
        .play(handle)
        .looped()
        .fade_in(AudioTween::linear(std::time::Duration::from_secs(2)));
}

/// Helper: Play a one-shot SFX.
pub fn play_sfx(
    sfx_channel: &AudioChannel<SfxChannel>,
    asset_server: &AssetServer,
    sfx_path: String,
) {
    let handle = asset_server.load(sfx_path);
    sfx_channel.play(handle);
}

/// Helper: Start an ambient loop.
pub fn play_ambient(
    ambient_channel: &AudioChannel<AmbientChannel>,
    asset_server: &AssetServer,
    ambient_path: String,
) {
    ambient_channel.stop();
    let handle = asset_server.load(ambient_path);
    ambient_channel.play(handle).looped();
}
