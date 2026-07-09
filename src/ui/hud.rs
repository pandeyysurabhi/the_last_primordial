//! HUD overlay — health bar, stamina bar, Black Sword indicator.

use bevy::prelude::*;

use crate::components::*;

/// Marker for the HUD root node.
#[derive(Component)]
pub struct HudRoot;

/// Marker for the health bar fill.
#[derive(Component)]
pub struct HealthBarFill;

/// Marker for the stamina bar fill.
#[derive(Component)]
pub struct StaminaBarFill;

/// Spawn the HUD overlay.
pub fn spawn_hud(mut commands: Commands) {
    commands
        .spawn((
            Node {
                width: Val::Percent(100.0),
                height: Val::Percent(100.0),
                flex_direction: FlexDirection::Column,
                padding: UiRect::all(Val::Px(16.0)),
                ..default()
            },
            HudRoot,
            // Only visible during gameplay
            Name::new("HUD"),
        ))
        .with_children(|parent| {
            // ── Top-left bars container ──
            parent
                .spawn(Node {
                    flex_direction: FlexDirection::Column,
                    row_gap: Val::Px(4.0),
                    ..default()
                })
                .with_children(|bars| {
                    // Health label + bar
                    spawn_bar(bars, "HP", Color::srgb(0.8, 0.15, 0.15), HealthBarFill);
                    // Stamina label + bar
                    spawn_bar(bars, "SP", Color::srgb(0.15, 0.7, 0.3), StaminaBarFill);
                });
        });
}

/// Helper: create a labeled bar (background + fill).
fn spawn_bar(parent: &mut ChildSpawnerCommands, label: &str, fill_color: Color, marker: impl Component) {
    parent
        .spawn(Node {
            flex_direction: FlexDirection::Row,
            align_items: AlignItems::Center,
            column_gap: Val::Px(8.0),
            ..default()
        })
        .with_children(|row| {
            // Label
            row.spawn((
                Text::new(label.to_string()),
                TextFont {
                    font_size: FontSize::Px(14.0),
                    ..default()
                },
                TextColor(Color::WHITE),
            ));

            // Bar background
            row.spawn((
                Node {
                    width: Val::Px(180.0),
                    height: Val::Px(14.0),
                    ..default()
                },
                BackgroundColor(Color::srgba(0.1, 0.1, 0.1, 0.8)),
            ))
            .with_children(|bg| {
                // Bar fill
                bg.spawn((
                    Node {
                        width: Val::Percent(100.0),
                        height: Val::Percent(100.0),
                        ..default()
                    },
                    BackgroundColor(fill_color.into()),
                    marker,
                ));
            });
        });
}

/// System: Update health bar width based on player health.
pub fn update_health_bar(
    player_q: Query<&Health, With<Player>>,
    mut bar_q: Query<&mut Node, With<HealthBarFill>>,
) {
    let Ok(health) = player_q.single() else {
        return;
    };
    for mut node in &mut bar_q {
        node.width = Val::Percent(health.fraction() * 100.0);
    }
}

/// System: Update stamina bar width based on player stamina.
pub fn update_stamina_bar(
    player_q: Query<&Stamina, With<Player>>,
    mut bar_q: Query<&mut Node, With<StaminaBarFill>>,
) {
    let Ok(stamina) = player_q.single() else {
        return;
    };
    for mut node in &mut bar_q {
        node.width = Val::Percent(stamina.fraction() * 100.0);
    }
}
