using Godot;
using Player.StateMachine;
using Player.StateMachine.States;

namespace Player
{
    /// <summary>
    /// Main player controller — Godot port of Unity's PlayerController.
    /// Unity → Godot mapping:
    ///   MonoBehaviour   → CharacterBody2D (handles physics + MoveAndSlide)
    ///   Rigidbody2D     → CharacterBody2D.Velocity
    ///   CapsuleCollider → CollisionShape2D (child node)
    ///   Animator        → AnimatedSprite2D (child node, drives individual-frame PNGs)
    ///   Physics2D.OverlapBox → ShapeCast2D child nodes
    ///   Update()        → _Process()
    ///   FixedUpdate()   → _PhysicsProcess()
    /// </summary>
    public partial class PlayerController : CharacterBody2D
    {
        // ── Exported References (assign in Inspector) ─────────────────────────
        [Export] public PlayerMovementSettings Settings { get; private set; } = null!;
        [Export] public KaelCombatSettings CombatSettings { get; private set; } = null!;

        // ── Child Node References (auto-found in _Ready) ──────────────────────
        public AnimationPlayer Animator { get; private set; } = null!;
        private AnimatedSprite2D _sprite = null!;

        public IPlayerInput InputReader  { get; private set; } = null!;
        public PlayerStateMachine StateMachine { get; private set; } = null!;

        // ── States ─────────────────────────────────────────────────────────────
        public PlayerIdleState      IdleState      { get; private set; } = null!;
        public PlayerRunState       RunState       { get; private set; } = null!;
        public PlayerJumpState      JumpState      { get; private set; } = null!;
        public PlayerFallState      FallState      { get; private set; } = null!;
        public PlayerWallSlideState WallSlideState { get; private set; } = null!;
        public PlayerWallJumpState  WallJumpState  { get; private set; } = null!;
        public PlayerAttackState    AttackState    { get; private set; } = null!;
        public PlayerTetherState    TetherState    { get; private set; } = null!;

        private bool _isSpectral = false;
        private CanvasLayer? _timeFreezeLayer;
        private ColorRect? _timeFreezeOverlayRect;

        // ── Collision State ───────────────────────────────────────────────────
        public bool IsGrounded      { get; private set; }
        public bool TouchingWall    { get; private set; }
        public int  WallDirection   { get; private set; }  // 1=Right  -1=Left  0=None
        public int  FacingDirection { get; private set; } = 1;

        // ── Control Lock ──────────────────────────────────────────────────────
        private double _controlLockTimer;
        public bool CanMove => _controlLockTimer <= 0.0;

        // ── Delta time (stored each tick so states can use it) ────────────────
        public float PhysicsDelta { get; private set; } = 1f / 60f;

        // ── ShapeCast Nodes (detect ground / walls without overlap queries) ───
        private ShapeCast2D _groundCheck = null!;
        private ShapeCast2D _wallCheckRight = null!;
        private ShapeCast2D _wallCheckLeft  = null!;

        // ── Gravity (pixels/s² stored as float to avoid repeated casting) ─────
        private float _gravity;

        // ── Lifecycle ─────────────────────────────────────────────────────────
        public override void _Ready()
        {
            _gravity = (float)ProjectSettings.GetSetting("physics/2d/default_gravity");

            // Create default resources if not assigned in the Inspector
            if (Settings == null)
            {
                Settings = new PlayerMovementSettings();
                GD.Print("[PlayerController] No PlayerMovementSettings assigned — using defaults.");
            }
            if (CombatSettings == null)
            {
                CombatSettings = new KaelCombatSettings();
                GD.Print("[PlayerController] No KaelCombatSettings assigned — using defaults.");
            }

            Animator    = GetNode<AnimationPlayer>("AnimationPlayer");
            _sprite     = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
            InputReader = GetNode<PlayerInput>("PlayerInput");

            // Ground / wall detection via ShapeCast2D children
            _groundCheck    = GetNode<ShapeCast2D>("GroundCheck");
            _wallCheckRight = GetNode<ShapeCast2D>("WallCheckRight");
            _wallCheckLeft  = GetNode<ShapeCast2D>("WallCheckLeft");

            // Build state machine
            StateMachine    = new PlayerStateMachine();
            IdleState       = new PlayerIdleState(this, StateMachine, "Idle");
            RunState        = new PlayerRunState(this, StateMachine, "Run");
            JumpState       = new PlayerJumpState(this, StateMachine, "Jump");
            FallState       = new PlayerFallState(this, StateMachine, "Fall");
            WallSlideState  = new PlayerWallSlideState(this, StateMachine, "WallSlide");
            WallJumpState   = new PlayerWallJumpState(this, StateMachine, "Jump");
            AttackState     = new PlayerAttackState(this, StateMachine, "Attack");
            TetherState     = new PlayerTetherState(this, StateMachine, "Jump");

            // Initialize collision mask to listen to default, physical, and spectral layers (1 + 2 + 4 = 7)
            CollisionLayer = 1u;
            CollisionMask = 7u;
            _groundCheck.CollisionMask = CollisionMask;
            _wallCheckRight.CollisionMask = CollisionMask;
            _wallCheckLeft.CollisionMask = CollisionMask;

            // Create time freeze screen overlay dynamically
            _timeFreezeLayer = new CanvasLayer();
            _timeFreezeLayer.Layer = 100; // Render above other canvas layers
            _timeFreezeOverlayRect = new ColorRect();
            _timeFreezeOverlayRect.SetAnchorsAndOffsetsPreset(Control.LayoutPreset.FullRect);
            _timeFreezeOverlayRect.Color = new Color(0.1f, 0.4f, 0.9f, 0.0f); // Ice blue transparent tint
            _timeFreezeOverlayRect.Visible = false;
            _timeFreezeLayer.AddChild(_timeFreezeOverlayRect);
            AddChild(_timeFreezeLayer);

            StateMachine.Initialize(IdleState);

            GD.Print("[PlayerController] Player initialized — ready for action!");
        }

        public override void _Process(double delta)
        {
            if (_controlLockTimer > 0.0) _controlLockTimer -= delta;

            // Handle Phase Shift ability (Dimensional Slip)
            if (InputReader.PhaseShiftDown)
            {
                TogglePhase();
            }

            // Handle Time Freeze ability (Chronostasis Pulse)
            if (InputReader.TimeFreezeDown)
            {
                TriggerTimeFreeze(4f);
            }

            // Handle Gravity Tether entry (Vector Pull)
            if (InputReader.TetherHeld && StateMachine.CurrentState != TetherState)
            {
                Node2D? nearestRift = FindNearestGravityRift(250f); // 250px range
                if (nearestRift != null)
                {
                    TetherState.SetTargetRift(nearestRift);
                    StateMachine.ChangeState(TetherState);
                }
            }

            StateMachine.CurrentState.LogicUpdate();
        }

        public override void _PhysicsProcess(double delta)
        {
            PhysicsDelta = (float)delta;
            PerformCollisionChecks();
            StateMachine.CurrentState.PhysicsUpdate();

            // Apply gravity manually (CharacterBody2D does not auto-apply gravity)
            if (!IsOnFloor())
                Velocity += new Vector2(0, _gravity * GravityScale * PhysicsDelta);

            MoveAndSlide();

            // Check if player fell out of bounds (Y > 320)
            if (GlobalPosition.Y > 320f)
            {
                GD.Print("[PlayerController] Player fell out of bounds! Reloading scene.");
                GetTree().ReloadCurrentScene();
            }
        }

        // ── Gravity Scale (replaces Rigidbody2D.gravityScale) ────────────────
        public float GravityScale { get; set; } = 1f;

        // ── Velocity Helpers (matching Unity API surface exactly) ─────────────
        public void SetVelocity(float x, float y)  => Velocity = new Vector2(x, y);
        public void SetVelocityX(float x)           => Velocity = new Vector2(x, Velocity.Y);
        public void SetVelocityY(float y)           => Velocity = new Vector2(Velocity.X, y);

        // ── Control Lock ──────────────────────────────────────────────────────
        public void LockMovementControl(float duration) => _controlLockTimer = duration;

        // ── Facing / Flip ─────────────────────────────────────────────────────
        public void CheckIfShouldFlip(float horizontalInput)
        {
            if      (horizontalInput > 0 && FacingDirection == -1) Flip();
            else if (horizontalInput < 0 && FacingDirection ==  1) Flip();
        }

        private void Flip()
        {
            FacingDirection *= -1;
            if (_sprite != null)
                _sprite.FlipH = FacingDirection == -1;
        }

        // ── Animation ─────────────────────────────────────────────────────────
        /// <summary>
        /// States call this on Enter. Delegates directly to AnimatedSprite2D.Play().
        /// Animation names match SpriteFrames resource: Idle, Run, Walk, Jump, Fall,
        /// WallSlide, Attack, Attack2, Hurt.
        /// </summary>
        public void PlayAnimation(string animName)
        {
            if (_sprite == null) return;
            if (_sprite.Animation == animName && _sprite.IsPlaying()) return;
            _sprite.Play(animName);
        }

        // ── Collision Detection ───────────────────────────────────────────────
        private void PerformCollisionChecks()
        {
            // Ground: IsOnFloor() is the reliable built-in for CharacterBody2D
            IsGrounded = IsOnFloor();

            // Walls: use ShapeCast2D children
            bool touchRight = _wallCheckRight.IsColliding();
            bool touchLeft  = _wallCheckLeft.IsColliding();

            if      (touchRight) { TouchingWall = true;  WallDirection =  1; }
            else if (touchLeft)  { TouchingWall = true;  WallDirection = -1; }
            else                 { TouchingWall = false; WallDirection =  0; }
        }

        // ── Custom Metroidvania Gameplay Mechanics ───────────────────────────
        private void SpawnPhaseSparks()
        {
            var sparks = new CpuParticles2D();
            sparks.Amount = 24;
            sparks.Lifetime = 0.5f;
            sparks.OneShot = true;
            sparks.Explosiveness = 0.85f;
            sparks.Spread = 180f;
            sparks.Gravity = new Vector2(0, 0);
            sparks.InitialVelocityMin = 50f;
            sparks.InitialVelocityMax = 120f;
            sparks.ScaleAmountMin = 1.0f;
            sparks.ScaleAmountMax = 3.0f;
            sparks.Color = _isSpectral ? new Color(0.7f, 0.4f, 1.0f, 0.9f) : new Color(0.9f, 0.95f, 1.0f, 0.9f);
            
            GetParent().AddChild(sparks);
            sparks.GlobalPosition = GlobalPosition + new Vector2(0, -18);
            
            var timer = GetTree().CreateTimer(0.6f);
            timer.Timeout += () => {
                if (GodotObject.IsInstanceValid(sparks))
                {
                    sparks.QueueFree();
                }
            };
        }

        private void TogglePhase()
        {
            _isSpectral = !_isSpectral;
            GD.Print("[PlayerController] Phase shifted to: ", _isSpectral ? "Spectral" : "Physical");

            // Notify all PhasePlatforms in the active scene to swap visual/collision state
            GetTree().CallGroup("PhaseActive", "OnPhaseChanged", _isSpectral);

            // Spawn visual sparks at Kael's position
            SpawnPhaseSparks();

            // Tween visual modulation for transition feedback on Kael himself
            var tween = CreateTween().SetParallel(true);
            if (_isSpectral)
            {
                tween.TweenProperty(_sprite, "modulate", new Color(0.7f, 0.4f, 1.0f, 1.0f), 0.2f);
            }
            else
            {
                tween.TweenProperty(_sprite, "modulate", new Color(1.0f, 1.0f, 1.0f, 1.0f), 0.2f);
            }
        }

        private async void TriggerTimeFreeze(float duration)
        {
            GD.Print("[PlayerController] Chronostasis Pulse activated!");

            // Freeze all nodes in the "TimeFreezable" group
            var freezables = GetTree().GetNodesInGroup("TimeFreezable");
            foreach (var node in freezables)
            {
                if (node is TimeFreezableHazard hazard)
                {
                    hazard.Freeze();
                }
            }

            // Visual flash representation
            var flash = CreateTween();
            flash.TweenProperty(this, "modulate", new Color(1.5f, 1.5f, 2.0f, 1.0f), 0.1f);
            flash.TweenProperty(this, "modulate", new Color(1.0f, 1.0f, 1.0f, 1.0f), 0.1f);

            // Smooth fade-in overlay for time freeze visual effect
            if (_timeFreezeOverlayRect != null)
            {
                _timeFreezeOverlayRect.Visible = true;
                _timeFreezeOverlayRect.Modulate = new Color(1f, 1f, 1f, 0f);
                var overlayTween = CreateTween();
                overlayTween.TweenProperty(_timeFreezeOverlayRect, "modulate:a", 0.4f, 0.25f);
            }

            // Wait for duration (non-blocking)
            await ToSignal(GetTree().CreateTimer(duration), SceneTreeTimer.SignalName.Timeout);

            GD.Print("[PlayerController] Time resumes.");

            // Unfreeze
            var freezablesEnd = GetTree().GetNodesInGroup("TimeFreezable");
            foreach (var node in freezablesEnd)
            {
                if (node is TimeFreezableHazard hazard)
                {
                    hazard.Unfreeze();
                }
            }

            // Smooth fade-out of universal time freeze overlay
            if (_timeFreezeOverlayRect != null)
            {
                var overlayTween = CreateTween();
                overlayTween.TweenProperty(_timeFreezeOverlayRect, "modulate:a", 0f, 0.25f);
                overlayTween.Finished += () => { _timeFreezeOverlayRect.Visible = false; };
            }
        }

        private Node2D? FindNearestGravityRift(float maxDistance)
        {
            var rifts = GetTree().GetNodesInGroup("GravityRift");
            Node2D? nearest = null;
            float nearestDist = maxDistance;

            foreach (var node in rifts)
            {
                if (node is Node2D rift)
                {
                    float dist = GlobalPosition.DistanceTo(rift.GlobalPosition);
                    if (dist < nearestDist)
                    {
                        nearestDist = dist;
                        nearest = rift;
                    }
                }
            }
            return nearest;
        }
    }
}
