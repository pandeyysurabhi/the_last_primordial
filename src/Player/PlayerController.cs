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
    ///   Animator        → AnimationPlayer (child node)
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
        private Sprite2D _sprite = null!;
        private double   _animTimer;
        private int      _animFrame;
        // Spritesheet layout: 6 columns x 4 rows (170x256 px each cell in 1024x1024)
        // Row 0 = Idle(4), Row 1 = Run(6), Row 2 = Jump/Fall(4), Row 3 = Attack(5)
        private int _animRowOffset;  // top-left frame index for current anim
        private int _animFrameCount;
        private float _animFps;
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
            _sprite     = GetNode<Sprite2D>("Sprite2D");
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

            StateMachine.Initialize(IdleState);

            GD.Print("[PlayerController] Kael initialized — ready for action!");
        }

        public override void _Process(double delta)
        {
            if (_controlLockTimer > 0.0) _controlLockTimer -= delta;
            StateMachine.CurrentState.LogicUpdate();
            TickAnimation(delta);
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
            Scale = new Vector2(Scale.X * -1, Scale.Y);
        }

        // ── Animation ─────────────────────────────────────────────────────────
        /// <summary>
        /// States call this on Enter. We map anim name → spritesheet row.
        /// </summary>
        public void PlayAnimation(string animName)
        {
            // Map state name → (rowOffset, frameCount, fps)
            (int row, int count, float fps) = animName switch
            {
                "Idle"      => (0, 4, 6f),
                "Run"       => (6, 6, 12f),  // row 1 starts at frame index 6
                "Jump"      => (12, 2, 8f),  // row 2 starts at frame index 12
                "Fall"      => (14, 2, 8f),  // row 2 frames 2-3
                "WallSlide" => (14, 2, 6f),
                "Attack"    => (18, 5, 14f), // row 3 starts at frame index 18
                _           => (0, 4, 6f)
            };
            _animRowOffset   = row;
            _animFrameCount  = count;
            _animFps         = fps;
            _animFrame       = 0;
            _animTimer       = 0;
            if (_sprite != null)
                _sprite.Frame = _animRowOffset;
        }

        private void TickAnimation(double delta)
        {
            if (_sprite == null || _animFrameCount <= 0) return;
            _animTimer += delta;
            double frameDuration = 1.0 / _animFps;
            if (_animTimer >= frameDuration)
            {
                _animTimer -= frameDuration;
                _animFrame = (_animFrame + 1) % _animFrameCount;
                _sprite.Frame = _animRowOffset + _animFrame;
            }
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
    }
}
