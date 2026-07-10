using Godot;

namespace TheLastPrimordial.Combat;

/// <summary>
/// Data definition for a single combo move.
/// Combo trees are built by linking moves via NextMoves.
/// Inspired by DMC5's combo chain system.
/// </summary>
[GlobalClass]
public partial class ComboData : Resource
{
    public enum InputType { Light, Heavy }

    /// <summary>Unique identifier for this move.</summary>
    [Export] public string MoveName { get; set; } = "";

    /// <summary>Which button triggers this move in the combo chain.</summary>
    [Export] public InputType Input { get; set; } = InputType.Light;

    /// <summary>Animation to play for this move.</summary>
    [Export] public string AnimationName { get; set; } = "attack_light_1";

    // --- Damage ---
    [ExportGroup("Damage")]
    [Export] public float Damage { get; set; } = 10f;
    [Export] public float PostureDamage { get; set; } = 15f;
    [Export] public float KnockbackForce { get; set; } = 100f;

    // --- Launcher (DMC5 juggle) ---
    [ExportGroup("Launcher")]
    /// <summary>If true, launches the enemy into the air for juggle combos.</summary>
    [Export] public bool IsLauncher { get; set; }
    [Export] public float LaunchForce { get; set; } = 300f;

    // --- Timing ---
    [ExportGroup("Timing")]
    /// <summary>Duration of this attack animation in seconds.</summary>
    [Export] public float Duration { get; set; } = 0.3f;

    /// <summary>Time within animation when hitbox activates (fraction 0-1).</summary>
    [Export(PropertyHint.Range, "0,1,0.05")] public float HitboxStartFraction { get; set; } = 0.2f;

    /// <summary>Time within animation when hitbox deactivates (fraction 0-1).</summary>
    [Export(PropertyHint.Range, "0,1,0.05")] public float HitboxEndFraction { get; set; } = 0.6f;

    // --- Combo Flow ---
    [ExportGroup("Combo Flow")]
    /// <summary>Whether this move can be cancelled into a dodge (DMC5 dodge-cancel).</summary>
    [Export] public bool CanDodgeCancel { get; set; } = true;

    /// <summary>Whether this is an aerial-only move.</summary>
    [Export] public bool IsAerial { get; set; }

    /// <summary>Forward momentum applied during this attack.</summary>
    [Export] public float ForwardMomentum { get; set; } = 30f;
}
