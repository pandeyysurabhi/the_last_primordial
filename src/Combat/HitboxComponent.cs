using Godot;

namespace TheLastPrimordial.Combat;

/// <summary>
/// Hitbox component — an Area2D representing an attack's damage area.
/// Attached to the attacker. Enabled/disabled per-frame by AnimationPlayer
/// to match attack animations precisely.
/// </summary>
public partial class HitboxComponent : Area2D
{
    [ExportGroup("Damage")]
    [Export] public float Damage { get; set; } = 10f;
    [Export] public float PostureDamage { get; set; } = 15f;

    [ExportGroup("Knockback")]
    [Export] public float KnockbackForce { get; set; } = 150f;

    /// <summary>
    /// The entity that owns this hitbox. Used to prevent self-hits
    /// and determine knockback direction.
    /// </summary>
    public new Node2D? Owner { get; set; }

    /// <summary>
    /// Whether this is a launcher attack (sends enemies airborne for juggle combos).
    /// </summary>
    [Export] public bool IsLauncher { get; set; }

    /// <summary>
    /// Vertical launch force for launcher attacks (DMC5-style juggle).
    /// </summary>
    [Export] public float LaunchForce { get; set; } = 300f;

    public override void _Ready()
    {
        // Hitboxes start disabled — activated by attack animations
        SetDeferred("monitoring", false);

        // Auto-detect owner
        Owner ??= GetParent<Node2D>();
    }

    /// <summary>
    /// Activate the hitbox (called by AnimationPlayer at attack frame).
    /// </summary>
    public void Activate()
    {
        Monitoring = true;
    }

    /// <summary>
    /// Deactivate the hitbox (called by AnimationPlayer after attack frame).
    /// </summary>
    public void Deactivate()
    {
        Monitoring = false;
    }

    /// <summary>
    /// Get knockback direction from this hitbox toward a target position.
    /// </summary>
    public Vector2 GetKnockbackDirection(Vector2 targetPosition)
    {
        if (Owner == null) return Vector2.Right;
        return (targetPosition - Owner.GlobalPosition).Normalized();
    }
}
