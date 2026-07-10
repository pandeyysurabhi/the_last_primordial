using Godot;

namespace TheLastPrimordial.Combat;

/// <summary>
/// Manages HP and Posture for any combat entity (player, enemy, boss).
/// Implements Sekiro-style posture mechanics: posture builds on block/hit,
/// recovers over time (slower at low HP), and breaks into a vulnerable state.
/// </summary>
public partial class HealthSystem : Node
{
    [Export] public CombatStats? Stats { get; set; }

    // --- Current values ---
    public float CurrentHealth { get; private set; }
    public float CurrentPosture { get; private set; }

    /// <summary>Normalized HP (0-1) for UI display.</summary>
    public float HealthPercent => Stats != null ? CurrentHealth / Stats.MaxHealth : 0f;

    /// <summary>Normalized posture (0-1) for UI display.</summary>
    public float PosturePercent => Stats != null ? CurrentPosture / Stats.MaxPosture : 0f;

    public bool IsDead { get; private set; }

    /// <summary>True when posture bar is full → entity is staggered/vulnerable.</summary>
    public bool IsPostureBroken { get; private set; }

    /// <summary>Whether this entity is currently invincible (dash i-frames, hurt recovery).</summary>
    public bool IsInvincible { get; set; }

    // Timer tracking for posture recovery delay
    private float _timeSinceLastHit;

    // --- Signals ---
    [Signal] public delegate void HealthChangedEventHandler(float currentHealth, float maxHealth);
    [Signal] public delegate void PostureChangedEventHandler(float currentPosture, float maxPosture);
    [Signal] public delegate void PostureBrokenEventHandler();
    [Signal] public delegate void PostureRecoveredEventHandler();
    [Signal] public delegate void DiedEventHandler();
    [Signal] public delegate void DamageTakenEventHandler(float amount, Vector2 knockbackDir);

    public override void _Ready()
    {
        if (Stats == null)
        {
            GD.PushWarning($"HealthSystem on '{GetParent().Name}': No CombatStats assigned!");
            return;
        }
        CurrentHealth = Stats.MaxHealth;
        CurrentPosture = 0f;
    }

    public override void _Process(double delta)
    {
        if (Stats == null || IsDead) return;

        _timeSinceLastHit += (float)delta;

        // Posture recovery (Sekiro-style: delayed, slower at low HP)
        if (!IsPostureBroken && _timeSinceLastHit >= Stats.PostureRecoveryDelay && CurrentPosture > 0f)
        {
            float recoveryRate = Stats.PostureRecoveryRate;

            // Lower HP = slower posture recovery (core Sekiro mechanic)
            if (Stats.HpAffectsPostureRecovery)
            {
                recoveryRate *= HealthPercent;
            }

            CurrentPosture = Mathf.Max(0f, CurrentPosture - recoveryRate * (float)delta);
            EmitSignal(SignalName.PostureChanged, CurrentPosture, Stats.MaxPosture);
        }
    }

    /// <summary>
    /// Apply HP damage and posture damage. Core damage function.
    /// </summary>
    public void TakeDamage(float hpDamage, float postureDamage, Vector2 knockbackDir)
    {
        if (IsDead || IsInvincible) return;
        if (Stats == null) return;

        // HP damage
        CurrentHealth = Mathf.Max(0f, CurrentHealth - hpDamage);
        EmitSignal(SignalName.HealthChanged, CurrentHealth, Stats.MaxHealth);
        EmitSignal(SignalName.DamageTaken, hpDamage, knockbackDir);

        // Posture damage
        TakePostureDamage(postureDamage);

        _timeSinceLastHit = 0f;

        // Death check
        if (CurrentHealth <= 0f)
        {
            Die();
        }
    }

    /// <summary>
    /// Apply posture damage only (e.g., from blocking — reduced HP damage but posture still fills).
    /// </summary>
    public void TakePostureDamage(float amount)
    {
        if (IsDead || Stats == null) return;

        CurrentPosture = Mathf.Min(Stats.MaxPosture, CurrentPosture + amount);
        EmitSignal(SignalName.PostureChanged, CurrentPosture, Stats.MaxPosture);
        _timeSinceLastHit = 0f;

        if (CurrentPosture >= Stats.MaxPosture && !IsPostureBroken)
        {
            BreakPosture();
        }
    }

    /// <summary>
    /// Heal HP by amount. Clamps to MaxHealth.
    /// </summary>
    public void Heal(float amount)
    {
        if (IsDead || Stats == null) return;
        CurrentHealth = Mathf.Min(Stats.MaxHealth, CurrentHealth + amount);
        EmitSignal(SignalName.HealthChanged, CurrentHealth, Stats.MaxHealth);
    }

    /// <summary>
    /// Reset posture to 0 (after recovering from posture break).
    /// </summary>
    public void ResetPosture()
    {
        if (Stats == null) return;
        CurrentPosture = 0f;
        IsPostureBroken = false;
        EmitSignal(SignalName.PostureChanged, CurrentPosture, Stats.MaxPosture);
        EmitSignal(SignalName.PostureRecovered);
    }

    private void BreakPosture()
    {
        IsPostureBroken = true;
        EmitSignal(SignalName.PostureBroken);
    }

    private void Die()
    {
        IsDead = true;
        EmitSignal(SignalName.Died);
    }

    /// <summary>
    /// Full reset (e.g., respawn).
    /// </summary>
    public void FullReset()
    {
        if (Stats == null) return;
        CurrentHealth = Stats.MaxHealth;
        CurrentPosture = 0f;
        IsPostureBroken = false;
        IsDead = false;
        _timeSinceLastHit = 0f;
        EmitSignal(SignalName.HealthChanged, CurrentHealth, Stats.MaxHealth);
        EmitSignal(SignalName.PostureChanged, CurrentPosture, Stats.MaxPosture);
    }
}
