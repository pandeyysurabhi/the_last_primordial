using Godot;
using TheLastPrimordial.Combat;
using TheLastPrimordial.Core;

namespace TheLastPrimordial.Player;

/// <summary>
/// Main player controller for Kael. CharacterBody2D with movement parameters
/// inspired by Hollow Knight (tight platforming, wall mechanics, coyote time)
/// and combat parameters inspired by Sekiro + DMC5.
/// </summary>
public partial class Player : CharacterBody2D
{
    // ========================
    //  MOVEMENT PARAMETERS
    // ========================

    [ExportGroup("Movement")]
    [Export] public float MoveSpeed { get; set; } = 80f;
    [Export] public float Acceleration { get; set; } = 800f;
    [Export] public float Friction { get; set; } = 1000f;
    [Export] public float AirFriction { get; set; } = 400f;

    [ExportGroup("Jump")]
    [Export] public float JumpForce { get; set; } = 280f;
    /// <summary>Velocity multiplier when jump is released early (variable height).</summary>
    [Export] public float JumpCutMultiplier { get; set; } = 0.4f;
    [Export] public float CoyoteTime { get; set; } = 0.1f;
    [Export] public float JumpBufferTime { get; set; } = 0.12f;

    [ExportGroup("Gravity")]
    [Export] public float GravityScale { get; set; } = 980f;
    [Export] public float MaxFallSpeed { get; set; } = 300f;
    [Export] public float FastFallMultiplier { get; set; } = 1.5f;

    [ExportGroup("Dash")]
    [Export] public float DashSpeed { get; set; } = 250f;
    [Export] public float DashDuration { get; set; } = 0.15f;
    [Export] public float DashCooldown { get; set; } = 0.5f;

    [ExportGroup("Wall")]
    [Export] public float WallSlideSpeed { get; set; } = 40f;
    [Export] public float WallJumpHorizontalForce { get; set; } = 200f;
    [Export] public float WallJumpVerticalForce { get; set; } = 260f;
    /// <summary>Brief period after wall jump where horizontal input is ignored.</summary>
    [Export] public float WallJumpInputLockTime { get; set; } = 0.15f;

    // ========================
    //  STATE
    // ========================

    /// <summary>Which direction the player is facing. 1 = right, -1 = left.</summary>
    public int FacingDirection { get; set; } = 1;

    // Timers for input forgiveness
    public float CoyoteTimer { get; set; }
    public float JumpBufferTimer { get; set; }
    public float DashCooldownTimer { get; set; }
    public float WallJumpLockTimer { get; set; }

    public bool CanDash => DashCooldownTimer <= 0f;
    public bool HasJumpBuffer => JumpBufferTimer > 0f;
    public bool HasCoyoteTime => CoyoteTimer > 0f;

    // ========================
    //  NODE REFERENCES
    // ========================

    public StateMachine StateMachine { get; private set; } = null!;
    public Sprite2D Sprite { get; private set; } = null!;
    public AnimationPlayer AnimPlayer { get; private set; } = null!;
    public RayCast2D WallDetectorRight { get; private set; } = null!;
    public RayCast2D WallDetectorLeft { get; private set; } = null!;
    public HitboxComponent Hitbox { get; private set; } = null!;
    public HurtboxComponent Hurtbox { get; private set; } = null!;
    public HealthSystem HealthSystem { get; private set; } = null!;
    public DeflectSystem DeflectSystem { get; private set; } = null!;
    public ComboTracker ComboTracker { get; private set; } = null!;
    public StyleMeter StyleMeter { get; private set; } = null!;

    // ========================
    //  SIGNALS
    // ========================

    [Signal] public delegate void JumpedEventHandler();
    [Signal] public delegate void DashedEventHandler();
    [Signal] public delegate void WallJumpedEventHandler();
    [Signal] public delegate void LandedEventHandler();

    public override void _Ready()
    {
        // Get node references
        StateMachine = GetNode<StateMachine>("StateMachine");
        Sprite = GetNode<Sprite2D>("Sprite2D");
        AnimPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
        WallDetectorRight = GetNode<RayCast2D>("WallDetectorRight");
        WallDetectorLeft = GetNode<RayCast2D>("WallDetectorLeft");
        Hitbox = GetNode<HitboxComponent>("HitboxComponent");
        Hurtbox = GetNode<HurtboxComponent>("HurtboxComponent");
        HealthSystem = GetNode<HealthSystem>("HealthSystem");
        DeflectSystem = GetNode<DeflectSystem>("DeflectSystem");
        ComboTracker = GetNode<ComboTracker>("ComboTracker");
        StyleMeter = GetNode<StyleMeter>("StyleMeter");

        // Set hitbox owner
        Hitbox.Owner = this;

        // Connect health signals
        HealthSystem.DamageTaken += OnDamageTaken;
        HealthSystem.Died += OnDied;
        HealthSystem.PostureBroken += OnPostureBroken;

        GD.Print("Player initialized — Kael ready for action!");
    }

    public override void _Process(double delta)
    {
        // Decrement timers
        CoyoteTimer = Mathf.Max(0f, CoyoteTimer - (float)delta);
        JumpBufferTimer = Mathf.Max(0f, JumpBufferTimer - (float)delta);
        DashCooldownTimer = Mathf.Max(0f, DashCooldownTimer - (float)delta);
        WallJumpLockTimer = Mathf.Max(0f, WallJumpLockTimer - (float)delta);
    }

    // ========================
    //  HELPERS
    // ========================

    /// <summary>
    /// Get horizontal input direction (-1, 0, or 1).
    /// </summary>
    public float GetInputDirection()
    {
        return Input.GetAxis("move_left", "move_right");
    }

    /// <summary>
    /// Apply gravity to velocity. Capped at MaxFallSpeed.
    /// </summary>
    public void ApplyGravity(double delta, float multiplier = 1f)
    {
        if (!IsOnFloor())
        {
            float newVelY = Velocity.Y + GravityScale * multiplier * (float)delta;
            Velocity = new Vector2(Velocity.X, Mathf.Min(newVelY, MaxFallSpeed));
        }
    }

    /// <summary>
    /// Apply horizontal movement with acceleration/friction.
    /// </summary>
    public void ApplyHorizontalMovement(double delta, float direction, float speed)
    {
        if (WallJumpLockTimer > 0f) return; // Ignore input during wall jump lock

        if (Mathf.Abs(direction) > 0.1f)
        {
            // Accelerate
            float targetVelX = direction * speed;
            float accel = IsOnFloor() ? Acceleration : Acceleration * 0.8f;
            float newVelX = Mathf.MoveToward(Velocity.X, targetVelX, accel * (float)delta);
            Velocity = new Vector2(newVelX, Velocity.Y);
        }
        else
        {
            // Decelerate (friction)
            float fric = IsOnFloor() ? Friction : AirFriction;
            float newVelX = Mathf.MoveToward(Velocity.X, 0f, fric * (float)delta);
            Velocity = new Vector2(newVelX, Velocity.Y);
        }
    }

    /// <summary>
    /// Flip the sprite and hitbox/hurtbox based on facing direction.
    /// </summary>
    public void UpdateFacing(float direction)
    {
        if (direction == 0f) return;

        FacingDirection = direction > 0f ? 1 : -1;
        Sprite.FlipH = FacingDirection < 0;

        // Mirror hitbox position
        var hitboxPos = Hitbox.Position;
        hitboxPos.X = Mathf.Abs(hitboxPos.X) * FacingDirection;
        Hitbox.Position = hitboxPos;
    }

    /// <summary>
    /// Check if the player is touching a wall (via raycasts).
    /// Returns -1 (wall on left), 1 (wall on right), or 0 (no wall).
    /// </summary>
    public int GetWallDirection()
    {
        if (WallDetectorRight.IsColliding()) return 1;
        if (WallDetectorLeft.IsColliding()) return -1;
        return 0;
    }

    /// <summary>
    /// Whether the player is touching a wall and can wall slide.
    /// </summary>
    public bool IsTouchingWall()
    {
        return GetWallDirection() != 0;
    }

    // ========================
    //  EVENT HANDLERS
    // ========================

    private void OnDamageTaken(float amount, Vector2 knockbackDir)
    {
        StyleMeter.OnDamageTaken(amount);

        // Transition to hurt state (unless already dead or in i-frames)
        if (!HealthSystem.IsDead)
        {
            StateMachine.TransitionTo("Hurt");
        }
    }

    private void OnDied()
    {
        StateMachine.TransitionTo("Death");
    }

    private void OnPostureBroken()
    {
        // Player stagger — brief vulnerability (like Sekiro)
        GD.Print("Player posture broken! Staggered!");
        // Could transition to a special stagger state
    }
}
