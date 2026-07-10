using Godot;

namespace TheLastPrimordial.Combat;

/// <summary>
/// Resource holding combat statistics for any entity (player or enemy).
/// Create different .tres files per character/enemy type with unique values.
/// Inspired by Sekiro's posture system + DMC5's damage model.
/// </summary>
[GlobalClass]
public partial class CombatStats : Resource
{
    // --- Health ---

    [ExportGroup("Health")]
    [Export] public float MaxHealth { get; set; } = 100f;

    // --- Posture (Sekiro-style) ---

    [ExportGroup("Posture")]
    /// <summary>Max posture before breaking. Higher = harder to stagger.</summary>
    [Export] public float MaxPosture { get; set; } = 100f;

    /// <summary>How fast posture recovers per second when not being hit.</summary>
    [Export] public float PostureRecoveryRate { get; set; } = 15f;

    /// <summary>Delay in seconds before posture starts recovering after last hit.</summary>
    [Export] public float PostureRecoveryDelay { get; set; } = 1.5f;

    /// <summary>
    /// Whether lower HP means slower posture recovery (Sekiro mechanic).
    /// When true, posture recovery is multiplied by (currentHP / maxHP).
    /// </summary>
    [Export] public bool HpAffectsPostureRecovery { get; set; } = true;

    // --- Attack ---

    [ExportGroup("Attack")]
    [Export] public float AttackPower { get; set; } = 10f;
    [Export] public float PostureDamage { get; set; } = 15f;
    [Export] public float KnockbackForce { get; set; } = 150f;

    // --- Deflect (Sekiro-style) ---

    [ExportGroup("Deflect")]
    /// <summary>Window in seconds for a perfect deflect (Sekiro default ~0.2s).</summary>
    [Export] public float DeflectWindow { get; set; } = 0.2f;

    /// <summary>Posture damage dealt TO the attacker on perfect deflect.</summary>
    [Export] public float DeflectPostureDamage { get; set; } = 25f;

    /// <summary>Damage reduction when blocking (not perfect deflect). 0.0 = no damage, 1.0 = full damage.</summary>
    [Export(PropertyHint.Range, "0,1,0.05")] public float BlockDamageReduction { get; set; } = 0.2f;
}
