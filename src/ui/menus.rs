//! Menu systems — main menu, pause menu, settings.

use bevy::prelude::*;

use crate::states::GameState;

/// Marker for the main menu root.
#[derive(Component)]
pub struct MainMenuRoot;

/// Marker for the pause menu root.
#[derive(Component)]
pub struct PauseMenuRoot;

/// Identifiers for menu buttons.
#[derive(Component, Debug, Clone, Copy)]
pub enum MenuButton {
    NewGame,
    Continue,
    Settings,
    QuitGame,
    Resume,
    SaveGame,
    LoadGame,
    QuitToMenu,
}

/// Spawn the main menu UI.
pub fn spawn_main_menu(mut commands: Commands) {
    commands
        .spawn((
            Node {
                width: Val::Percent(100.0),
                height: Val::Percent(100.0),
                flex_direction: FlexDirection::Column,
                align_items: AlignItems::Center,
                justify_content: JustifyContent::Center,
                row_gap: Val::Px(12.0),
                ..default()
            },
            BackgroundColor(Color::srgba(0.03, 0.02, 0.06, 0.95)),
            MainMenuRoot,
            Name::new("MainMenu"),
        ))
        .with_children(|parent| {
            // Title
            parent.spawn((
                Text::new("THE LAST PRIMORDIAL"),
                TextFont {
                    font_size: FontSize::Px(42.0),
                    ..default()
                },
                TextColor(Color::srgb(0.85, 0.7, 1.0)),
                Node {
                    margin: UiRect::bottom(Val::Px(40.0)),
                    ..default()
                },
            ));

            // Subtitle
            parent.spawn((
                Text::new("The Eighth Law Awaits"),
                TextFont {
                    font_size: FontSize::Px(16.0),
                    ..default()
                },
                TextColor(Color::srgba(0.6, 0.5, 0.8, 0.7)),
                Node {
                    margin: UiRect::bottom(Val::Px(30.0)),
                    ..default()
                },
            ));

            // Buttons
            spawn_menu_button(parent, "New Game", MenuButton::NewGame);
            spawn_menu_button(parent, "Continue", MenuButton::Continue);
            spawn_menu_button(parent, "Settings", MenuButton::Settings);
            spawn_menu_button(parent, "Quit", MenuButton::QuitGame);
        });
}

/// Spawn the pause menu overlay.
pub fn spawn_pause_menu(mut commands: Commands) {
    commands
        .spawn((
            Node {
                width: Val::Percent(100.0),
                height: Val::Percent(100.0),
                flex_direction: FlexDirection::Column,
                align_items: AlignItems::Center,
                justify_content: JustifyContent::Center,
                row_gap: Val::Px(10.0),
                ..default()
            },
            BackgroundColor(Color::srgba(0.0, 0.0, 0.0, 0.7)),
            PauseMenuRoot,
            Name::new("PauseMenu"),
        ))
        .with_children(|parent| {
            parent.spawn((
                Text::new("PAUSED"),
                TextFont {
                    font_size: FontSize::Px(36.0),
                    ..default()
                },
                TextColor(Color::WHITE),
                Node {
                    margin: UiRect::bottom(Val::Px(24.0)),
                    ..default()
                },
            ));

            spawn_menu_button(parent, "Resume", MenuButton::Resume);
            spawn_menu_button(parent, "Save Game", MenuButton::SaveGame);
            spawn_menu_button(parent, "Load Game", MenuButton::LoadGame);
            spawn_menu_button(parent, "Quit to Menu", MenuButton::QuitToMenu);
        });
}

/// Helper: create a styled menu button.
fn spawn_menu_button(parent: &mut ChildSpawnerCommands, label: &str, button_id: MenuButton) {
    parent
        .spawn((
            Button,
            Node {
                width: Val::Px(220.0),
                height: Val::Px(40.0),
                justify_content: JustifyContent::Center,
                align_items: AlignItems::Center,
                border_radius: BorderRadius::all(Val::Px(4.0)),
                ..default()
            },
            BackgroundColor(Color::srgba(0.15, 0.1, 0.25, 0.8)),
            button_id,
        ))
        .with_children(|btn| {
            btn.spawn((
                Text::new(label.to_string()),
                TextFont {
                    font_size: FontSize::Px(18.0),
                    ..default()
                },
                TextColor(Color::srgb(0.9, 0.85, 1.0)),
            ));
        });
}

/// System: Handle button hover visual feedback.
pub fn button_hover_system(
    mut query: Query<(&Interaction, &mut BackgroundColor), (Changed<Interaction>, With<Button>)>,
) {
    for (interaction, mut bg) in &mut query {
        *bg = match interaction {
            Interaction::Pressed => BackgroundColor(Color::srgba(0.3, 0.2, 0.5, 0.9)),
            Interaction::Hovered => BackgroundColor(Color::srgba(0.2, 0.15, 0.35, 0.9)),
            Interaction::None => BackgroundColor(Color::srgba(0.15, 0.1, 0.25, 0.8)),
        };
    }
}

/// System: Handle button clicks in main menu.
pub fn main_menu_button_system(
    query: Query<(&Interaction, &MenuButton), (Changed<Interaction>, With<Button>)>,
    mut next_state: ResMut<NextState<GameState>>,
    mut exit: MessageWriter<AppExit>,
) {
    for (interaction, button) in &query {
        if *interaction != Interaction::Pressed {
            continue;
        }

        match button {
            MenuButton::NewGame => {
                next_state.set(GameState::Playing);
            }
            MenuButton::QuitGame => {
                exit.write(AppExit::Success);
            }
            _ => {} // Continue, Settings not yet implemented
        }
    }
}

/// System: Handle button clicks in pause menu.
pub fn pause_menu_button_system(
    query: Query<(&Interaction, &MenuButton), (Changed<Interaction>, With<Button>)>,
    mut next_state: ResMut<NextState<GameState>>,
) {
    for (interaction, button) in &query {
        if *interaction != Interaction::Pressed {
            continue;
        }

        match button {
            MenuButton::Resume => {
                next_state.set(GameState::Playing);
            }
            MenuButton::QuitToMenu => {
                next_state.set(GameState::MainMenu);
            }
            _ => {} // Save/Load handled by save system
        }
    }
}

/// System: Toggle pause with Escape key.
pub fn pause_toggle_system(
    keyboard: Res<ButtonInput<KeyCode>>,
    current_state: Res<State<GameState>>,
    mut next_state: ResMut<NextState<GameState>>,
) {
    if keyboard.just_pressed(KeyCode::Escape) {
        match current_state.get() {
            GameState::Playing => next_state.set(GameState::Paused),
            GameState::Paused => next_state.set(GameState::Playing),
            _ => {}
        }
    }
}

/// System: Cleanup main menu entities.
pub fn cleanup_main_menu(mut commands: Commands, query: Query<Entity, With<MainMenuRoot>>) {
    for entity in &query {
        commands.entity(entity).despawn();
    }
}

/// System: Cleanup pause menu entities.
pub fn cleanup_pause_menu(mut commands: Commands, query: Query<Entity, With<PauseMenuRoot>>) {
    for entity in &query {
        commands.entity(entity).despawn();
    }
}
