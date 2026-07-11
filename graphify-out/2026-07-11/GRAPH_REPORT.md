# Graph Report - .  (2026-07-11)

## Corpus Check
- 235 files · ~82,481 words
- Verdict: corpus is large enough that graph structure adds value.

## Summary
- 449 nodes · 1443 edges · 28 communities (21 shown, 7 thin omitted)
- Extraction: 100% EXTRACTED · 0% INFERRED · 0% AMBIGUOUS
- Token cost: 0 input · 0 output

## Community Hubs (Navigation)
- Core Lore and Seven Kingdoms
- Lore Realms and Spirits
- Choice Wielder Narrative
- Developer Bible and Game Design
- Editor and Setup Tools
- Player State Machine Core
- Player Grounded and Run States
- Player Godot Controller
- Player Movement and Input Settings
- Player Unity Controller
- Player Combat and Damage States
- Player Air and Jump States
- Player Wall and Grounded States
- Player Combat and Movement Config
- Player Attack State Combos
- Player Input Interface
- Player Base State Machine Actions
- Base MonoBehaviour Input
- Player Wall Jump State
- Player Wall Jump Handlers
- Player Fall State Actions
- Player Idle State Actions
- Player Jump State Actions
- Player Wall Slide State Actions
- Godot .NET Project Configuration
- Graphify Integration
- Graphify Settings

## God Nodes (most connected - your core abstractions)
1. `Story Chapters Index` - 142 edges
2. `Christ` - 109 edges
3. `The Core` - 61 edges
4. `Volume III – The Pilgrimage Through the Seven Realms` - 47 edges
5. `Choice Wielder` - 38 edges
6. `Choice` - 35 edges
7. `Seven Primordials` - 35 edges
8. `Volume II – The Child of Choice` - 35 edges
9. `The Black Sword` - 32 edges
10. `Volume I – The Birth of Reality` - 32 edges

## Surprising Connections (you probably didn't know these)
- `PlayerController` --references--> `KaelCombatSettings`  [EXTRACTED]
  src/Player/PlayerController.cs → Assets/Scripts/Player/Combat/KaelCombatSettings.cs
- `PlayerController` --references--> `IPlayerInput`  [EXTRACTED]
  src/Player/PlayerController.cs → Assets/Scripts/Player/Core/IPlayerInput.cs
- `PlayerController` --references--> `PlayerMovementSettings`  [EXTRACTED]
  src/Player/PlayerController.cs → Assets/Scripts/Player/Core/PlayerMovementSettings.cs
- `PlayerController` --references--> `PlayerStateMachine`  [EXTRACTED]
  src/Player/PlayerController.cs → Assets/Scripts/Player/StateMachine/PlayerStateMachine.cs
- `PlayerState` --references--> `PlayerStateMachine`  [EXTRACTED]
  src/Player/StateMachine/PlayerState.cs → Assets/Scripts/Player/StateMachine/PlayerStateMachine.cs

## Import Cycles
- None detected.

## Communities (28 total, 7 thin omitted)

### Community 0 - "Core Lore and Seven Kingdoms"
Cohesion: 0.13
Nodes (66): Choice Wielder, The Core, Kingdom of Chaos, Kingdom of Creation, Kingdom of Death, Kingdom of Destruction, Kingdom of Life, Kingdom of Space (+58 more)

### Community 1 - "Lore Realms and Spirits"
Cohesion: 0.13
Nodes (58): Christ, Spirit of Chaos, Spirit of Death, Spirit of Life, Spirit of Space, Spirit of Time, Realm of Chaos, Realm of Death (+50 more)

### Community 2 - "Choice Wielder Narrative"
Cohesion: 0.17
Nodes (36): Immortal Woman, Last Choice Wielder, The Black Sword, Part 1 – The Impossible Child, Part 2 – The Judgment of the Seven, Part 3 – The Black Sword and the Journey Begins, Part 4 – The Seven Realms, 1. The Endless Cycle (+28 more)

### Community 3 - "Developer Bible and Game Design"
Cohesion: 0.28
Nodes (31): Central Themes, Developer Bible Index, Future Expansion Opportunities, Gameplay Progression, Symbolism, Timeline, Game Lore Vault Dashboard, Choice (+23 more)

### Community 4 - "Editor and Setup Tools"
Cohesion: 0.14
Nodes (13): AnimationClip, AnimatorController, AnimatorState, float, int, string, KaelSetupWizard, Player.Editor (+5 more)

### Community 5 - "Player State Machine Core"
Cohesion: 0.12
Nodes (7): float, PlayerController, string, PlayerState, PlayerStateMachine, Player.StateMachine, PlayerStateMachine

### Community 6 - "Player Grounded and Run States"
Cohesion: 0.11
Nodes (6): float, PlayerGroundedState, PlayerRunState, Player.StateMachine.States, PlayerIdleState, PlayerRunState

### Community 7 - "Player Godot Controller"
Cohesion: 0.12
Nodes (8): AnimationPlayer, CharacterBody2D, ShapeCast2D, Sprite2D, double, float, int, PlayerController

### Community 8 - "Player Movement and Input Settings"
Cohesion: 0.11
Nodes (13): Player, Resource, IPlayerInput, bool, float, int, uint, Vector2 (+5 more)

### Community 9 - "Player Unity Controller"
Cohesion: 0.12
Nodes (6): Animator, Collider2D, float, PlayerController, Rigidbody2D, Transform

### Community 10 - "Player Combat and Damage States"
Cohesion: 0.13
Nodes (8): IDamageable, bool, Collider2D, int, KaelCombatSettings, PlayerAttackState, Player.Combat, Vector3

### Community 11 - "Player Air and Jump States"
Cohesion: 0.13
Nodes (5): float, PlayerAirState, PlayerFallState, bool, PlayerJumpState

### Community 12 - "Player Wall and Grounded States"
Cohesion: 0.13
Nodes (6): PlayerState, float, PlayerAirState, float, PlayerGroundedState, PlayerWallSlideState

### Community 13 - "Player Combat and Movement Config"
Cohesion: 0.17
Nodes (11): bool, float, int, LayerMask, Vector2, KaelCombatSettings, float, LayerMask (+3 more)

### Community 14 - "Player Attack State Combos"
Cohesion: 0.22
Nodes (4): bool, int, KaelCombatSettings, PlayerAttackState

### Community 15 - "Player Input Interface"
Cohesion: 0.22
Nodes (4): IPlayerInput, Node, float, PlayerInput

### Community 16 - "Player Base State Machine Actions"
Cohesion: 0.25
Nodes (4): double, PlayerController, string, PlayerState

### Community 17 - "Base MonoBehaviour Input"
Cohesion: 0.33
Nodes (4): float, string, PlayerInput, MonoBehaviour

### Community 18 - "Player Wall Jump State"
Cohesion: 0.33
Nodes (3): bool, int, PlayerWallJumpState

### Community 19 - "Player Wall Jump Handlers"
Cohesion: 0.33
Nodes (3): bool, int, PlayerWallJumpState

## Knowledge Gaps
- **5 isolated node(s):** `Player.Editor`, `net8.0`, `Godot.NET.Sdk/4.3.0`, `graphify`, `graphify`
  These have ≤1 connection - possible missing edges or undocumented components.
- **7 thin communities (<3 nodes) omitted from report** — run `graphify query` to explore isolated nodes.

## Suggested Questions
_Questions this graph is uniquely positioned to answer:_

- **Why does `PlayerController` connect `Player Unity Controller` to `Player State Machine Core`, `Player Grounded and Run States`, `Player Combat and Damage States`, `Player Combat and Movement Config`, `Player Input Interface`, `Base MonoBehaviour Input`, `Player Wall Jump State`, `Player Fall State Actions`, `Player Idle State Actions`, `Player Jump State Actions`, `Player Wall Slide State Actions`?**
  _High betweenness centrality (0.082) - this node is a cross-community bridge._
- **Why does `PlayerController` connect `Player Godot Controller` to `Player State Machine Core`, `Player Grounded and Run States`, `Player Combat and Damage States`, `Player Combat and Movement Config`, `Player Input Interface`, `Player Wall Jump State`, `Player Fall State Actions`, `Player Idle State Actions`, `Player Jump State Actions`, `Player Wall Slide State Actions`?**
  _High betweenness centrality (0.081) - this node is a cross-community bridge._
- **Why does `Story Chapters Index` connect `Core Lore and Seven Kingdoms` to `Lore Realms and Spirits`, `Choice Wielder Narrative`, `Developer Bible and Game Design`?**
  _High betweenness centrality (0.076) - this node is a cross-community bridge._
- **What connects `Player.Editor`, `net8.0`, `Godot.NET.Sdk/4.3.0` to the rest of the system?**
  _5 weakly-connected nodes found - possible documentation gaps or missing edges._
- **Should `Core Lore and Seven Kingdoms` be split into smaller, more focused modules?**
  _Cohesion score 0.1258741258741259 - nodes in this community are weakly interconnected._
- **Should `Lore Realms and Spirits` be split into smaller, more focused modules?**
  _Cohesion score 0.12825166364186327 - nodes in this community are weakly interconnected._
- **Should `Editor and Setup Tools` be split into smaller, more focused modules?**
  _Cohesion score 0.13675213675213677 - nodes in this community are weakly interconnected._