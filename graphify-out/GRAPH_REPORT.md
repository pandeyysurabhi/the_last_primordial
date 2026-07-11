# Graph Report - The_Last_Primordial  (2026-07-11)

## Corpus Check
- 233 files · ~203,992 words
- Verdict: corpus is large enough that graph structure adds value.

## Summary
- 755 nodes · 1089 edges · 57 communities (47 shown, 10 thin omitted)
- Extraction: 100% EXTRACTED · 0% INFERRED · 0% AMBIGUOUS
- Token cost: 0 input · 0 output

## Graph Freshness
- Built from commit: `7209b084`
- Run `git rev-parse HEAD` and compare to check if the graph is stale.
- Run `graphify update .` after code changes (no API cost).

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
- Part 3 – Death, Chaos, and the Cracks in Memory.md
- Part 4 – Creation, Destruction, and the Road to the Core.md
- Seven Realms.md
- Part 1 – The Impossible Child.md
- Part 1 – The Realm of Time.md
- Part 3 – The Black Sword and the Journey Begins.md
- Part 1 – The Truth of the Core.md
- Part 2 – The Burden of the Seven.md
- Part 4 – The Seven Realms.md
- Volume II – The Child of Choice.md
- Choice.md
- Part 3 – The Three Choices.md
- Volume IV – The Core and the Final Choice.md
- Player.StateMachine.States
- Player
- PlayerMovementSettings
- PlayerAirState
- Game Lore Vault Dashboard
- Story Chapters Index
- PlayerJumpState
- Part 1 – The Beginning of Everything
- Part 2 – The Collapse of Reality
- Part 2 – The Judgment of the Seven
- Part 2 – The Realms of Space and Life
- Part 3 – The Seven Primordials
- Part 4 – The Forgotten Law
- Part 4 – World Reference & Developer Bible
- graphify.md
- Lore Index.md

## God Nodes (most connected - your core abstractions)
1. `PlayerController` - 30 edges
2. `PlayerController` - 29 edges
3. `Player.StateMachine.States` - 20 edges
4. `KaelSetupWizard` - 17 edges
5. `PlayerState` - 14 edges
6. `PlayerAttackState` - 14 edges
7. `PlayerAirState` - 13 edges
8. `PlayerAttackState` - 11 edges
9. `Player` - 10 edges
10. `PlayerGroundedState` - 10 edges

## Surprising Connections (you probably didn't know these)
- `PlayerController` --references--> `KaelCombatSettings`  [EXTRACTED]
  src/Player/PlayerController.cs → Assets/Scripts/Player/Combat/KaelCombatSettings.cs
- `PlayerController` --references--> `IPlayerInput`  [EXTRACTED]
  src/Player/PlayerController.cs → Assets/Scripts/Player/Core/IPlayerInput.cs
- `PlayerController` --references--> `PlayerMovementSettings`  [EXTRACTED]
  src/Player/PlayerController.cs → Assets/Scripts/Player/Core/PlayerMovementSettings.cs
- `PlayerController` --references--> `PlayerStateMachine`  [EXTRACTED]
  src/Player/PlayerController.cs → Assets/Scripts/Player/StateMachine/PlayerStateMachine.cs
- `PlayerJumpState` --inherits--> `PlayerAirState`  [EXTRACTED]
  src/Player/States/PlayerJumpState.cs → Assets/Scripts/Player/StateMachine/States/PlayerAirState.cs

## Import Cycles
- None detected.

## Communities (57 total, 10 thin omitted)

### Community 0 - "Core Lore and Seven Kingdoms"
Cohesion: 0.08
Nodes (16): Kingdom of Chaos, Philosophy & Ideals, Kingdom of Creation, Philosophy & Ideals, Kingdom of Death, Philosophy & Ideals, Kingdom of Destruction, Philosophy & Ideals (+8 more)

### Community 1 - "Lore Realms and Spirits"
Cohesion: 0.06
Nodes (23): Seven Realm Spirits, The Spirits, Corrupted Belief, Narrative Encounter, Spirit of Chaos, Corrupted Belief, Narrative Encounter, Spirit of Creation (+15 more)

### Community 2 - "Choice Wielder Narrative"
Cohesion: 0.22
Nodes (7): Character Profile, Christ, Narrative Backstory, Immortal Woman, Profile, Last Choice Wielder, Profile

### Community 3 - "Developer Bible and Game Design"
Cohesion: 0.17
Nodes (6): Central Themes, Developer Bible Index, Future Expansion Opportunities, Gameplay Progression, Symbolism, Timeline

### Community 4 - "Editor and Setup Tools"
Cohesion: 0.14
Nodes (13): AnimationClip, AnimatorController, AnimatorState, float, int, string, KaelSetupWizard, Player.Editor (+5 more)

### Community 5 - "Player State Machine Core"
Cohesion: 0.09
Nodes (10): float, PlayerController, string, PlayerState, PlayerStateMachine, double, PlayerController, string (+2 more)

### Community 6 - "Player Grounded and Run States"
Cohesion: 0.13
Nodes (4): float, PlayerGroundedState, PlayerRunState, PlayerIdleState

### Community 7 - "Player Godot Controller"
Cohesion: 0.12
Nodes (7): AnimatedSprite2D, AnimationPlayer, CharacterBody2D, ShapeCast2D, double, float, PlayerController

### Community 8 - "Player Movement and Input Settings"
Cohesion: 0.17
Nodes (11): Resource, bool, float, int, uint, Vector2, KaelCombatSettings, float (+3 more)

### Community 9 - "Player Unity Controller"
Cohesion: 0.12
Nodes (6): Animator, Collider2D, float, PlayerController, Rigidbody2D, Transform

### Community 10 - "Player Combat and Damage States"
Cohesion: 0.13
Nodes (8): IDamageable, bool, Collider2D, int, KaelCombatSettings, PlayerAttackState, Player.Combat, Vector3

### Community 11 - "Player Air and Jump States"
Cohesion: 0.18
Nodes (3): float, PlayerAirState, PlayerFallState

### Community 12 - "Player Wall and Grounded States"
Cohesion: 0.14
Nodes (5): PlayerWallSlideState, PlayerState, float, PlayerGroundedState, PlayerWallSlideState

### Community 13 - "Player Combat and Movement Config"
Cohesion: 0.33
Nodes (6): bool, float, int, LayerMask, Vector2, KaelCombatSettings

### Community 14 - "Player Attack State Combos"
Cohesion: 0.22
Nodes (4): bool, int, KaelCombatSettings, PlayerAttackState

### Community 15 - "Player Input Interface"
Cohesion: 0.22
Nodes (4): IPlayerInput, Node, float, PlayerInput

### Community 16 - "Player Base State Machine Actions"
Cohesion: 0.06
Nodes (29): 1. The Beginning, 2. The Core, 3. Humanity's Rise, 4. The Seven Kingdoms, 5. Discovery of the Core, 6. Humanity's Golden Age, 7. The First Mistake, 1. Humanity Crosses the Line (+21 more)

### Community 17 - "Base MonoBehaviour Input"
Cohesion: 0.33
Nodes (4): float, string, PlayerInput, MonoBehaviour

### Community 18 - "Player Wall Jump State"
Cohesion: 0.33
Nodes (3): bool, int, PlayerWallJumpState

### Community 19 - "Player Wall Jump Handlers"
Cohesion: 0.33
Nodes (3): bool, int, PlayerWallJumpState

### Community 23 - "Player Wall Slide State Actions"
Cohesion: 0.07
Nodes (23): Aetherion - Guardian of Creation, Profile, The Price of Becoming a Primordial (Narrative Context), Chronar - Guardian of Time, Profile, The Price of Becoming a Primordial (Narrative Context), Elaria - Guardian of Life, Profile (+15 more)

### Community 26 - "Graphify Settings"
Cohesion: 0.10
Nodes (14): 10. The Spirit of Life, 11. Meeting Elaria, 12. Christ Begins to Doubt, 1. Between the Realms, 2. The Realm of Space, 3. Philosophy of Space, 4. Gameplay Mechanics, 5. The Spirit of Space (+6 more)

### Community 28 - "Part 3 – Death, Chaos, and the Cracks in Memory.md"
Cohesion: 0.07
Nodes (15): Chapters, Introduction, Key Takeaways, Part 3 – Death, Chaos, and the Cracks in Memory, 10. The Cracks in Memory, 11. The Beginning of Doubt, 1. The Realm of Death, 2. The Weight of Memory (+7 more)

### Community 29 - "Part 4 – Creation, Destruction, and the Road to the Core.md"
Cohesion: 0.07
Nodes (15): Chapters, Introduction, Key Takeaways, Part 4 – Creation, Destruction, and the Road to the Core, 10. The Last Voice, 11. The End of the Journey, 1. The Realm of Creation, 2. Gameplay Mechanics (+7 more)

### Community 30 - "Seven Realms.md"
Cohesion: 0.08
Nodes (16): Pilgrimage & Trials Chapters, Realm of Chaos, Pilgrimage & Trials Chapters, Realm of Creation, Pilgrimage & Trials Chapters, Realm of Death, Pilgrimage & Trials Chapters, Realm of Destruction (+8 more)

### Community 31 - "Part 1 – The Impossible Child.md"
Cohesion: 0.10
Nodes (12): Chapters, Introduction, Key Takeaways, Part 1 – The Impossible Child, 1. The Endless Cycle, 2. The Last Choice Wielder, 3. Morvex's Greatest Mistake, 4. Judgment of the Seven (+4 more)

### Community 32 - "Part 1 – The Realm of Time.md"
Cohesion: 0.10
Nodes (12): Chapters, Introduction, Key Takeaways, Part 1 – The Realm of Time, 1. Why the Realm of Time Comes First, 2. Arrival, 3. Environmental Design, 4. Core Gameplay Mechanics (+4 more)

### Community 33 - "Part 3 – The Black Sword and the Journey Begins.md"
Cohesion: 0.10
Nodes (12): Chapters, Introduction, Key Takeaways, Part 3 – The Black Sword and the Journey Begins, 1. A Life Built on Fragments, 2. The Nightmares, 3. An Ordinary Boy, 4. The Return of the Black Sword (+4 more)

### Community 34 - "Part 1 – The Truth of the Core.md"
Cohesion: 0.11
Nodes (11): Chapters, Introduction, Key Takeaways, Part 1 – The Truth of the Core, 10. Christ's Realization, 1. The Heart of Reality, 2. The First Memory, 3. The Original Catastrophe (+3 more)

### Community 35 - "Part 2 – The Burden of the Seven.md"
Cohesion: 0.11
Nodes (11): Chapters, Introduction, Key Takeaways, Part 2 – The Burden of the Seven, 1. The Seven Enter the Core, 2. The Weight They Carried, 3. Why They Never Chose Successors, 4. The Truth About His Parents (+3 more)

### Community 36 - "Part 4 – The Seven Realms.md"
Cohesion: 0.11
Nodes (11): Chapters, Introduction, Key Takeaways, Part 4 – The Seven Realms, 1. The World Beyond the Physical, 2. Why the Realms Exist, 3. The Realm Spirits, 4. The Seven Trials (+3 more)

### Community 37 - "Volume II – The Child of Choice.md"
Cohesion: 0.16
Nodes (9): 1. Reality Notices the Impossible, 2. The Council of the Seven, 3. The Verdict, 4. Why Christ Was Spared, 5. The Final Night, 6. The Execution, 7. A Child's Memory, Parts (+1 more)

### Community 38 - "Choice.md"
Cohesion: 0.13
Nodes (12): Choice, Lore, The Cycle, Choice Wielder, Lore, Narrative Context, Profile, The Black Sword (+4 more)

### Community 39 - "Part 3 – The Three Choices.md"
Cohesion: 0.13
Nodes (9): Chapters, Introduction, Key Takeaways, Part 3 – The Three Choices, 1. The Final Chamber, 2. The Black Sword's Last Purpose, 3. The Core Offers Three Paths, 4. Epilogues (+1 more)

### Community 40 - "Volume IV – The Core and the Final Choice.md"
Cohesion: 0.18
Nodes (6): 5. The Price of Becoming a Primordial, 7. Why Every Choice Wielder Failed, 8. His Parents' Choice, 8. One Last Question, Parts, Volume IV – The Core and the Final Choice

### Community 41 - "Player.StateMachine.States"
Cohesion: 0.29
Nodes (3): Player.StateMachine.States, Player.StateMachine, PlayerRunState

### Community 43 - "PlayerMovementSettings"
Cohesion: 0.33
Nodes (5): float, LayerMask, Vector2, PlayerMovementSettings, ScriptableObject

### Community 45 - "Game Lore Vault Dashboard"
Cohesion: 0.40
Nodes (4): 🛠️ [[Developer Bible]], Game Lore Vault Dashboard, 🔮 [[Lore Index|Lore Database]], 📖 [[Story Index|Story Chapters]]

### Community 46 - "Story Chapters Index"
Cohesion: 0.40
Nodes (5): Story Chapters Index, [[Volume I – The Birth of Reality]], [[Volume II – The Child of Choice]], [[Volume III – The Pilgrimage Through the Seven Realms]], [[Volume IV – The Core and the Final Choice]]

### Community 48 - "Part 1 – The Beginning of Everything"
Cohesion: 0.50
Nodes (4): Chapters, Introduction, Key Takeaways, Part 1 – The Beginning of Everything

### Community 49 - "Part 2 – The Collapse of Reality"
Cohesion: 0.50
Nodes (4): Chapters, Introduction, Key Takeaways, Part 2 – The Collapse of Reality

### Community 50 - "Part 2 – The Judgment of the Seven"
Cohesion: 0.50
Nodes (4): Chapters, Introduction, Key Takeaways, Part 2 – The Judgment of the Seven

### Community 51 - "Part 2 – The Realms of Space and Life"
Cohesion: 0.50
Nodes (4): Chapters, Introduction, Key Takeaways, Part 2 – The Realms of Space and Life

### Community 52 - "Part 3 – The Seven Primordials"
Cohesion: 0.50
Nodes (4): Chapters, Introduction, Key Takeaways, Part 3 – The Seven Primordials

### Community 53 - "Part 4 – The Forgotten Law"
Cohesion: 0.50
Nodes (4): Chapters, Introduction, Key Takeaways, Part 4 – The Forgotten Law

### Community 54 - "Part 4 – World Reference & Developer Bible"
Cohesion: 0.50
Nodes (3): Chapters, Introduction, Part 4 – World Reference & Developer Bible

## Knowledge Gaps
- **249 isolated node(s):** `Player.Editor`, `net8.0`, `Godot.NET.Sdk/4.3.0`, `graphify`, `Workflow: graphify` (+244 more)
  These have ≤1 connection - possible missing edges or undocumented components.
- **10 thin communities (<3 nodes) omitted from report** — run `graphify query` to explore isolated nodes.

## Suggested Questions
_Questions this graph is uniquely positioned to answer:_

- **Why does `PlayerController` connect `Player Unity Controller` to `Player State Machine Core`, `Player Grounded and Run States`, `Player.StateMachine.States`, `Player Combat and Damage States`, `PlayerMovementSettings`, `Player Wall and Grounded States`, `Player Combat and Movement Config`, `Player Input Interface`, `Base MonoBehaviour Input`, `Player Wall Jump State`, `Player Fall State Actions`, `Player Idle State Actions`, `Player Jump State Actions`?**
  _High betweenness centrality (0.029) - this node is a cross-community bridge._
- **Why does `PlayerController` connect `Player Godot Controller` to `Player State Machine Core`, `Player Grounded and Run States`, `Player.StateMachine.States`, `Player Combat and Damage States`, `PlayerMovementSettings`, `Player Wall and Grounded States`, `Player Combat and Movement Config`, `Player Input Interface`, `Player Wall Jump State`, `Player Fall State Actions`, `Player Idle State Actions`, `Player Jump State Actions`?**
  _High betweenness centrality (0.027) - this node is a cross-community bridge._
- **Why does `PlayerStateMachine` connect `Player State Machine Core` to `Player Unity Controller`, `Player Godot Controller`?**
  _High betweenness centrality (0.015) - this node is a cross-community bridge._
- **What connects `Player.Editor`, `net8.0`, `Godot.NET.Sdk/4.3.0` to the rest of the system?**
  _249 weakly-connected nodes found - possible documentation gaps or missing edges._
- **Should `Core Lore and Seven Kingdoms` be split into smaller, more focused modules?**
  _Cohesion score 0.08333333333333333 - nodes in this community are weakly interconnected._
- **Should `Lore Realms and Spirits` be split into smaller, more focused modules?**
  _Cohesion score 0.06451612903225806 - nodes in this community are weakly interconnected._
- **Should `Editor and Setup Tools` be split into smaller, more focused modules?**
  _Cohesion score 0.13675213675213677 - nodes in this community are weakly interconnected._