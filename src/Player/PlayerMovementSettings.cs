using Godot;

namespace Player
{
    /// <summary>
    /// Configuration data for character movement.
    /// In Godot, ScriptableObjects become Resources — create via right-click in FileSystem → New Resource.
    /// </summary>
    [GlobalClass]
    public partial class PlayerMovementSettings : Resource
    {
        // ── Horizontal Movement ──────────────────────────────────────────────
        [Export] public float MaxRunSpeed = 9f;
        [Export] public float RunAcceleration = 80f;
        [Export] public float RunDeceleration = 60f;
        [Export] public float AirAcceleration = 50f;
        [Export] public float AirDeceleration = 30f;

        // ── Jumping ──────────────────────────────────────────────────────────
        [Export] public float JumpForce = 15f;
        [Export] public float JumpGravityScale = 2.5f;
        [Export] public float FallGravityScale = 3.5f;
        [Export(PropertyHint.Range, "0,1")] public float JumpCutoffMultiplier = 0.5f;
        [Export] public float CoyoteTimeDuration = 0.12f;

        // ── Wall Mechanics ───────────────────────────────────────────────────
        [Export] public float WallSlideSpeed = 3f;
        [Export] public Vector2 WallJumpForce = new Vector2(8f, 15f);
        [Export] public float WallJumpControlLockDuration = 0.15f;

        // ── Collision Detection ──────────────────────────────────────────────
        [Export] public Vector2 GroundCheckOffset = new Vector2(0f, 2f);    // Godot Y is pixels from pivot
        [Export] public Vector2 GroundCheckSize   = new Vector2(26f, 4f);
        [Export] public Vector2 WallCheckOffset   = new Vector2(14f, 0f);
        [Export] public Vector2 WallCheckSize     = new Vector2(4f, 44f);

        /// <summary>Layer bitmask for ground surfaces. 1 = layer 1 in Godot project settings.</summary>
        [Export(PropertyHint.Layers2DPhysics)] public uint GroundLayer = 1;
        [Export(PropertyHint.Layers2DPhysics)] public uint WallLayer   = 1;
    }
}
