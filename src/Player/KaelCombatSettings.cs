using Godot;

namespace Player
{
    /// <summary>
    /// Designer-tunable combat parameters for Kael.
    /// Godot equivalent of Unity's KaelCombatSettings ScriptableObject.
    /// </summary>
    [GlobalClass]
    public partial class KaelCombatSettings : Resource
    {
        // ── Attack Timing ────────────────────────────────────────────────────
        [Export] public float AttackDuration      = 0.45f;
        [Export] public float HitboxActiveStart   = 0.1f;
        [Export] public float HitboxActiveEnd     = 0.35f;

        // ── Attack Physics ───────────────────────────────────────────────────
        [Export] public bool  LockMovementOnGroundAttack = true;
        [Export(PropertyHint.Range, "0,1")] public float AirAttackSpeedMultiplier = 0.3f;

        // ── Hitbox ───────────────────────────────────────────────────────────
        [Export] public Vector2 HitboxSize   = new Vector2(1.6f, 0.9f);
        [Export] public Vector2 HitboxOffset = new Vector2(0.9f, 0f);

        /// <summary>Physics layers that this attack can hit.</summary>
        [Export(PropertyHint.Layers2DPhysics)] public uint HittableLayers = 2;

        // ── Damage ───────────────────────────────────────────────────────────
        [Export] public float AttackDamage    = 15f;
        [Export] public float KnockbackForce  = 8f;

        // ── Combo ────────────────────────────────────────────────────────────
        [Export] public float ComboWindowDuration = 0.2f;
        [Export] public int   MaxComboCount       = 3;
    }
}
