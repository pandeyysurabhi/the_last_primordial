using Godot;
using System;

namespace TheLastPrimordial
{
    /// <summary>
    /// Controls the game flow in Ashen Hollow: waking up, finding the key,
    /// unlocking the cottage, triggering the time freeze, claiming the Black Sword,
    /// and opening the escape route.
    /// </summary>
    public partial class AshenHollowController : Node2D
    {
        [Export] public string EscapeScenePath = "res://scenes/forest_escape.tscn";

        // References to scene nodes
        private Node2D _blackSword = null!;
        private Node2D _whisperingWell = null!;
        private CollisionShape2D _wellCollider = null!;
        private Node2D _forestGate = null!;

        // NPCs
        private Interactable _gideonInteractable = null!;
        private Interactable _valaInteractable = null!;
        private Interactable _liaInteractable = null!;
        
        // Triggers
        private Interactable _keyPickup = null!;
        private Control _keyPickupVisual = null!;
        private CollisionShape2D _keyPickupCollider = null!;
        private Interactable _cottageDoor = null!;
        private StaticBody2D _lockedCottageDoorNode = null!;
        private Interactable _burntCradle = null!;
        private Interactable _swordPickup = null!;
        private Area2D _escapeTrigger = null!;

        // Weather and environment
        private ColorRect _timeFreezeOverlay = null!;
        private CpuParticles2D? _stormRain;
        private double _lightningTimer = 0.0;
        private double _nextLightningTime = 6.0;
        private ColorRect _lightningOverlay = null!;
        private ColorRect _sleepingGiant = null!;
        private Node2D _frozenWaterfall = null!;
        private CollisionShape2D _waterfallCollider = null!;
        private Interactable _waterfallInteractable = null!;
        private CollisionShape2D _waterfallInteractableCollider = null!;

        // Cistern and Basement shortcut triggers
        private Interactable _cisternTrapdoor = null!;
        private Interactable _ladderToSurface = null!;
        private Interactable _basementTrapdoor = null!;
        private Interactable _basementTrapdoorUpper = null!;
        private bool _basementUnlocked = false;

        public override void _Ready()
        {
            // DialogueSystem setup
            if (DialogueSystem.Instance == null)
            {
                var ds = new DialogueSystem();
                AddChild(ds);
            }

            // Bind references
            _blackSword = GetNode<Node2D>("Environment/BlackSword");
            _whisperingWell = GetNode<Node2D>("Environment/WhisperingWell");
            _wellCollider = GetNode<CollisionShape2D>("Environment/WhisperingWell/CollisionShape2D");
            _forestGate = GetNode<Node2D>("Environment/ForestGate");
            
            _gideonInteractable = GetNode<Interactable>("NPCs/ElderGideon/Interactable");
            _valaInteractable = GetNode<Interactable>("NPCs/BlacksmithVala/Interactable");
            _liaInteractable = GetNode<Interactable>("NPCs/ShrineMaidenLia/Interactable");

            _keyPickup = GetNode<Interactable>("Environment/KeyPickup/Interactable");
            _keyPickupVisual = GetNode<Control>("Environment/KeyPickup/Visual");
            _keyPickupCollider = GetNode<CollisionShape2D>("Environment/KeyPickup/Interactable/CollisionShape2D");
            _cottageDoor = GetNode<Interactable>("Environment/LockedCottageDoor/Interactable");
            _lockedCottageDoorNode = GetNode<StaticBody2D>("Environment/LockedCottageDoor");
            _burntCradle = GetNode<Interactable>("Environment/BurntCradle/Interactable");
            _swordPickup = GetNode<Interactable>("Environment/BlackSword/Interactable");
            _escapeTrigger = GetNode<Area2D>("Environment/EscapeTrigger");

            _timeFreezeOverlay = GetNode<ColorRect>("CanvasLayer/TimeFreezeOverlay");
            _timeFreezeOverlay.Visible = false;
            _lightningOverlay = GetNode<ColorRect>("CanvasLayer/LightningOverlay");
            _lightningOverlay.Visible = false;
            _sleepingGiant = GetNode<ColorRect>("Background/SleepingGiantSilhouette");
            _stormRain = GetNodeOrNull<CpuParticles2D>("Environment/StormRain");
            _frozenWaterfall = GetNode<Node2D>("Environment/FrozenWaterfall");
            _waterfallCollider = GetNode<CollisionShape2D>("Environment/FrozenWaterfall/CollisionShape2D");
            _waterfallInteractable = GetNode<Interactable>("Environment/FrozenWaterfall/Interactable");
            _waterfallInteractableCollider = GetNode<CollisionShape2D>("Environment/FrozenWaterfall/Interactable/CollisionShape2D");
            _waterfallInteractableCollider.SetDeferred(CollisionShape2D.PropertyName.Disabled, true);

            _cisternTrapdoor = GetNode<Interactable>("Environment/CisternTrapdoor/Interactable");
            _ladderToSurface = GetNode<Interactable>("Environment/SewerCistern/LadderToSurface/Interactable");
            _basementTrapdoor = GetNode<Interactable>("Environment/SewerCistern/BasementTrapdoor/Interactable");
            _basementTrapdoorUpper = GetNode<Interactable>("Environment/BasementTrapdoorUpper/Interactable");

            // Connect signals
            _keyPickup.Interacted += OnKeyPickedUp;
            _cottageDoor.Interacted += OnCottageDoorInteracted;
            _burntCradle.Interacted += OnBurntCradleInteracted;
            _waterfallInteractable.Interacted += OnFrozenWaterfallInteracted;
            _swordPickup.Interacted += OnSwordPickedUp;
            _escapeTrigger.BodyEntered += OnEscapeTriggerEntered;

            _cisternTrapdoor.Interacted += OnCisternTrapdoorInteracted;
            _ladderToSurface.Interacted += OnLadderToSurfaceInteracted;
            _basementTrapdoor.Interacted += OnBasementTrapdoorInteracted;
            _basementTrapdoorUpper.Interacted += OnBasementTrapdoorUpperInteracted;

            _gideonInteractable.Interacted += OnGideonInteracted;
            _valaInteractable.Interacted += OnValaInteracted;
            _liaInteractable.Interacted += OnLiaInteracted;

            // Hide the sword initially
            _blackSword.Visible = false;
            _swordPickup.CollisionMask = 0; // Disable interaction

            // Set up initial dialog
            DialogueSystem.Show("Christ wakes up in Elder Gideon's Longhouse. The storm is howling outside.", 4f);
        }

        public override void _Process(double delta)
        {
            if (!GameState.TimeFrozen)
            {
                _lightningTimer += delta;
                if (_lightningTimer >= _nextLightningTime)
                {
                    TriggerLightningFlash();
                }
            }
        }

        private void TriggerLightningFlash()
        {
            _lightningTimer = 0.0;
            _nextLightningTime = GD.RandRange(8.0, 16.0);

            // Flash overlay screen white-violet
            _lightningOverlay.Visible = true;
            _lightningOverlay.Modulate = new Color(0.9f, 0.9f, 1.0f, 0.7f);

            // Reveal sleeping giant background silhouette
            _sleepingGiant.Modulate = new Color(1f, 1f, 1f, 0.45f);

            var tween = CreateTween().SetParallel(true);
            tween.TweenProperty(_lightningOverlay, "modulate:a", 0.0f, 0.45f);
            tween.TweenProperty(_sleepingGiant, "modulate:a", 0.0f, 0.45f);
            tween.Finished += () => { _lightningOverlay.Visible = false; };
        }

        private void OnGideonInteracted()
        {
            if (GameState.HasBlackSword)
            {
                DialogueSystem.Instance.ShowText("Gideon: 'The covenant is dead! Run, Christ! Escape through the Eastern forest gate!'", 3f);
            }
            else if (GameState.TimeFrozen)
            {
                DialogueSystem.Instance.ShowText("Elder Gideon is completely frozen, his eyes wide in silent terror.", 3f);
            }
            else
            {
                GameState.TalkedToGideon = true;
                DialogueSystem.Instance.ShowText("Gideon: 'Your father Alaric carried a black blade that defied the Seven. Ask Vala at the forge, she knows where it was hidden.'", 4.5f);
            }
        }

        private void OnValaInteracted()
        {
            if (GameState.TimeFrozen)
            {
                DialogueSystem.Instance.ShowText("Vala's hammer is frozen mid-air, sparks suspended like burning dust.", 3f);
            }
            else if (!GameState.TalkedToGideon)
            {
                DialogueSystem.Instance.ShowText("Vala: 'Go talk to Gideon first, boy. He is the elder here.'", 3f);
            }
            else
            {
                GameState.TalkedToVala = true;
                DialogueSystem.Instance.ShowText("Vala: 'Alaric's blade... Vala remembers. But Gideon knows the key to his cottage where the time magic rests. Ask Lia, she knows Gideon's secrets.'", 4.5f);
            }
        }

        private void OnLiaInteracted()
        {
            if (GameState.TimeFrozen)
            {
                DialogueSystem.Instance.ShowText("Lia is motionless, frozen tears hanging like crystal beads on her cheeks.", 3f);
            }
            else if (!GameState.TalkedToVala)
            {
                DialogueSystem.Instance.ShowText("Lia: 'I hear the well whispering... speak with Vala first, Christ.'", 3f);
            }
            else
            {
                GameState.TalkedToLia = true;
                if (GodotObject.IsInstanceValid(_keyPickupVisual))
                {
                    _keyPickupVisual.Visible = true;
                }
                if (GodotObject.IsInstanceValid(_keyPickupCollider))
                {
                    _keyPickupCollider.SetDeferred(CollisionShape2D.PropertyName.Disabled, false);
                }
                DialogueSystem.Instance.ShowText("Lia: 'The Whispering Well tells me Alaric's blade is locked on the church rooftop, but the cottage key is hidden under the loose floorboards in Gideon's house!'", 5f);
            }
        }

        private void OnKeyPickedUp()
        {
            if (GameState.HasCottageKey) return;
            GameState.HasCottageKey = true;
            DialogueSystem.Instance.ShowText("Picked up the Soot-Covered Cottage Key from under the loose floorboards.", 4f);
            
            // Vanish the key visual and its parent node
            var parent = _keyPickup.GetParent();
            if (GodotObject.IsInstanceValid(parent))
            {
                parent.QueueFree();
            }
        }

        private void OnCottageDoorInteracted()
        {
            if (GameState.CottageInvestigated) return;

            if (!GameState.HasCottageKey)
            {
                DialogueSystem.Instance.ShowText("The door to the burnt cottage is tightly bound by rusted iron chains and a glowing rune lock.", 4f);
            }
            else
            {
                DialogueSystem.Instance.ShowText("You unlock the chains. The lock shatters with a heavy metallic clank.", 4f);
                if (GodotObject.IsInstanceValid(_lockedCottageDoorNode))
                {
                    _lockedCottageDoorNode.QueueFree(); // Completely clear the door
                }
            }
        }

        private async void OnBurntCradleInteracted()
        {
            if (GameState.CottageInvestigated) return;
            GameState.CottageInvestigated = true;
            GameState.TimeFrozen = true;

            DialogueSystem.Instance.ShowText("The charred remains of a cradle. As you touch the soot, frost forms... A path to the church roof has opened!", 4.5f);
            _burntCradle.QueueFree();

            // Freeze waterfall into solid black ice platform
            _waterfallCollider.SetDeferred(CollisionShape2D.PropertyName.Disabled, false);
            _waterfallInteractableCollider.SetDeferred(CollisionShape2D.PropertyName.Disabled, false);
            var waterfallTween = CreateTween();
            waterfallTween.TweenProperty(_frozenWaterfall, "modulate", new Color(0.1f, 0.15f, 0.35f, 0.85f), 1.0f);

            // Freeze player briefly for effect
            var player = GetNodeOrNull<Player.PlayerController>("Player");
            if (player != null)
            {
                player.LockMovementControl(2.0f);
                player.SetVelocity(0, 0);
            }

            // Freeze rain particles
            if (_stormRain != null)
            {
                _stormRain.SpeedScale = 0f;
            }

            // Visual time freeze transition (fade in blue tint overlay)
            _timeFreezeOverlay.Visible = true;
            _timeFreezeOverlay.Modulate = new Color(0.2f, 0.4f, 1f, 0.0f);
            var tween = CreateTween();
            tween.TweenProperty(_timeFreezeOverlay, "modulate:a", 0.4f, 1.5f);

            // Modulate environment to desaturated colors
            var env = GetNode<Node2D>("Environment");
            var envTween = CreateTween();
            envTween.TweenProperty(env, "modulate", new Color(0.4f, 0.6f, 1.0f, 1.0f), 1.5f);

            await ToSignal(GetTree().CreateTimer(1.5f), SceneTreeTimer.SignalName.Timeout);

            // Show the floating Black Sword at the well
            _blackSword.Visible = true;
            _swordPickup.CollisionMask = 1; // Enable interaction
        }

        private void OnSwordPickedUp()
        {
            if (GameState.HasBlackSword) return;
            GameState.HasBlackSword = true;
            GameState.TimeFrozen = false;

            DialogueSystem.Instance.ShowText("Voices of previous wielders echo: 'We stood... we fell. You were born of choice... choose wisely!'", 4.5f);
            _blackSword.Visible = false;

            // Resume rain particles
            if (_stormRain != null)
            {
                _stormRain.SpeedScale = 1f;
            }

            // Melt waterfall back to transparent fluid
            _waterfallCollider.SetDeferred(CollisionShape2D.PropertyName.Disabled, true);
            var waterfallMelt = CreateTween();
            waterfallMelt.TweenProperty(_frozenWaterfall, "modulate", new Color(0.2f, 0.45f, 0.9f, 0.0f), 0.5f);

            // Shatter time freeze (remove overlay and restore colors)
            var tween = CreateTween().SetParallel(true);
            tween.TweenProperty(_timeFreezeOverlay, "modulate:a", 0.0f, 0.5f);
            var env = GetNode<Node2D>("Environment");
            tween.TweenProperty(env, "modulate", new Color(1f, 1f, 1f, 1f), 0.5f);

            // Well implodes - hide the well visuals and remove collision
            _whisperingWell.Visible = false;
            _wellCollider.SetDeferred(CollisionShape2D.PropertyName.Disabled, true);

            // Open the Eastern Forest Gate by freeing it
            if (GodotObject.IsInstanceValid(_forestGate))
            {
                _forestGate.QueueFree();
            }

            // Gideon shouts warning
            GetTree().CreateTimer(2f).Timeout += () => {
                DialogueSystem.Instance.ShowText("Gideon: 'The covenant is dead! The Seven have felt the breach! Run, Christ! Escape through the Eastern gate!'", 4f);
            };
        }

        private void OnEscapeTriggerEntered(Node2D body)
        {
            if (body is Player.PlayerController && GameState.HasBlackSword)
            {
                GD.Print("[AshenHollowController] Escaping to forest!");
                GetTree().CallDeferred(SceneTree.MethodName.ChangeSceneToFile, EscapeScenePath);
            }
        }

        private void OnCisternTrapdoorInteracted()
        {
            var player = GetNodeOrNull<Player.PlayerController>("Player");
            if (player != null)
            {
                player.GlobalPosition = new Vector2(-170f, 420f);
                DialogueSystem.Instance.ShowText("You descend into the cold, damp stone vaults of the Church Cistern.", 3.5f);
            }
        }

        private void OnLadderToSurfaceInteracted()
        {
            var player = GetNodeOrNull<Player.PlayerController>("Player");
            if (player != null)
            {
                player.GlobalPosition = new Vector2(-170f, 140f);
                DialogueSystem.Instance.ShowText("Climbing back up to the church courtyard.", 3.5f);
            }
        }

        private void OnFrozenWaterfallInteracted()
        {
            var player = GetNodeOrNull<Player.PlayerController>("Player");
            if (player != null)
            {
                player.GlobalPosition = new Vector2(-40f, -130f); // Teleport directly to the church roof
                DialogueSystem.Instance.ShowText("You climb up the frozen waterfall flow directly onto the high church rooftop.", 3.5f);
            }
        }

        private void OnBasementTrapdoorInteracted()
        {
            var player = GetNodeOrNull<Player.PlayerController>("Player");
            if (player != null)
            {
                if (!_basementUnlocked)
                {
                    _basementUnlocked = true;
                    DialogueSystem.Instance.ShowText("You slide the heavy iron bolt. The trapdoor to Gideon's house is now unlocked from below!", 4.5f);
                }
                
                player.GlobalPosition = new Vector2(80f, 140f);
            }
        }

        private void OnBasementTrapdoorUpperInteracted()
        {
            if (!_basementUnlocked)
            {
                DialogueSystem.Instance.ShowText("A wooden cellar trapdoor. It is securely bolted from the underside.", 4f);
            }
            else
            {
                var player = GetNodeOrNull<Player.PlayerController>("Player");
                if (player != null)
                {
                    player.GlobalPosition = new Vector2(80f, 420f);
                    DialogueSystem.Instance.ShowText("Descending into the cellar vaults.", 3f);
                }
            }
        }
    }
}
