using Godot;

namespace TheLastPrimordial.Combat;

/// <summary>
/// Hurtbox component — an Area2D representing an entity's vulnerable area.
/// Detects entering HitboxComponents and routes damage to the parent HealthSystem.
/// Respects invincibility (dash i-frames, hurt recovery).
/// </summary>
public partial class HurtboxComponent : Area2D
{
    /// <summary>
    /// Reference to the HealthSystem that receives damage from this hurtbox.
    /// Auto-detected from sibling nodes if not set.
    /// </summary>
    [Export] public HealthSystem? Health { get; set; }

    [Signal] public delegate void HurtboxHitEventHandler(HitboxComponent hitbox);

    public override void _Ready()
    {
        // Auto-find HealthSystem on parent
        if (Health == null)
        {
            var parent = GetParent();
            if (parent != null)
            {
                Health = parent.GetNodeOrNull<HealthSystem>("HealthSystem");
            }
        }

        AreaEntered += OnAreaEntered;
    }

    private void OnAreaEntered(Area2D area)
    {
        if (area is not HitboxComponent hitbox) return;

        // Don't hit ourselves
        if (hitbox.Owner == GetParent()) return;

        // Check invincibility
        if (Health != null && Health.IsInvincible) return;

        EmitSignal(SignalName.HurtboxHit, hitbox);

        // Route damage to HealthSystem
        if (Health != null)
        {
            var knockbackDir = hitbox.GetKnockbackDirection(GlobalPosition);
            Health.TakeDamage(hitbox.Damage, hitbox.PostureDamage, knockbackDir);
        }
    }
}
