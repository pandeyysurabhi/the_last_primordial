using Godot;

namespace TheLastPrimordial.Combat;

/// <summary>
/// Sekiro-inspired deflect/parry system.
/// Tracks timing windows for perfect deflects vs regular blocks.
/// 
/// Block input pressed → starts a deflect window (e.g., 0.2s).
/// If hit during window → Perfect Deflect (enemy takes posture damage, player takes 0 HP).
/// If hit while blocking but outside window → Block (reduced HP damage, posture damage to self).
/// If hit while not blocking → Full hit.
/// </summary>
public partial class DeflectSystem : Node
{
    [Export] public HealthSystem? Health { get; set; }

    /// <summary>Duration of the perfect deflect window in seconds.</summary>
    [Export] public float DeflectWindowDuration { get; set; } = 0.2f;

    /// <summary>Posture damage dealt to attacker on perfect deflect.</summary>
    [Export] public float DeflectPostureDamage { get; set; } = 25f;

    /// <summary>HP damage multiplier when blocking (0 = no damage, 1 = full damage).</summary>
    [Export(PropertyHint.Range, "0,1,0.05")] public float BlockDamageMultiplier { get; set; } = 0.2f;

    /// <summary>Posture damage taken by self when blocking (not deflecting).</summary>
    [Export] public float BlockSelfPostureDamage { get; set; } = 10f;

    /// <summary>Whether the player is currently holding the block button.</summary>
    public bool IsBlocking { get; set; }

    /// <summary>Whether we're within the perfect deflect timing window.</summary>
    public bool IsInDeflectWindow { get; private set; }

    private float _deflectTimer;

    // --- Signals ---
    [Signal] public delegate void PerfectDeflectEventHandler(HitboxComponent attackerHitbox);
    [Signal] public delegate void BlockedHitEventHandler(HitboxComponent attackerHitbox);
    [Signal] public delegate void DeflectWindowExpiredEventHandler();

    public override void _Process(double delta)
    {
        if (IsInDeflectWindow)
        {
            _deflectTimer -= (float)delta;
            if (_deflectTimer <= 0f)
            {
                IsInDeflectWindow = false;
                EmitSignal(SignalName.DeflectWindowExpired);
            }
        }
    }

    /// <summary>
    /// Called when the player presses the block button.
    /// Opens the perfect deflect timing window.
    /// </summary>
    public void StartBlock()
    {
        IsBlocking = true;
        IsInDeflectWindow = true;
        _deflectTimer = DeflectWindowDuration;
    }

    /// <summary>
    /// Called when the player releases the block button.
    /// </summary>
    public void StopBlock()
    {
        IsBlocking = false;
        IsInDeflectWindow = false;
    }

    /// <summary>
    /// Process an incoming attack. Returns the result type.
    /// Call this from the HurtboxComponent hit handler BEFORE applying damage.
    /// </summary>
    public DeflectResult ProcessIncomingAttack(HitboxComponent hitbox)
    {
        if (IsInDeflectWindow)
        {
            // PERFECT DEFLECT — Sekiro's core mechanic
            // Player takes 0 damage, attacker takes heavy posture damage
            EmitSignal(SignalName.PerfectDeflect, hitbox);
            return DeflectResult.PerfectDeflect;
        }
        else if (IsBlocking)
        {
            // BLOCK — reduced HP damage, but player takes posture damage
            EmitSignal(SignalName.BlockedHit, hitbox);
            return DeflectResult.Blocked;
        }
        else
        {
            // FULL HIT — no mitigation
            return DeflectResult.Hit;
        }
    }

    /// <summary>
    /// Apply the damage based on deflect result. Returns modified damage values.
    /// </summary>
    public (float hpDamage, float postureDamage) GetModifiedDamage(
        DeflectResult result, float rawHpDamage, float rawPostureDamage)
    {
        return result switch
        {
            DeflectResult.PerfectDeflect => (0f, 0f), // Player takes nothing
            DeflectResult.Blocked => (rawHpDamage * BlockDamageMultiplier, BlockSelfPostureDamage),
            _ => (rawHpDamage, rawPostureDamage) // Full hit
        };
    }
}

/// <summary>
/// Result of deflect processing.
/// </summary>
public enum DeflectResult
{
    /// <summary>Full hit — no block active.</summary>
    Hit,
    /// <summary>Block active but outside deflect window — reduced damage, posture cost.</summary>
    Blocked,
    /// <summary>Within perfect deflect window — 0 damage to player, posture damage to attacker.</summary>
    PerfectDeflect
}
