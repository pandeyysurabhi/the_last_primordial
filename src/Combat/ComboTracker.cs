using Godot;
using System.Collections.Generic;

namespace TheLastPrimordial.Combat;

/// <summary>
/// Tracks combo state and manages combo chains.
/// DMC5-inspired: supports ground combos, aerial combos,
/// launchers, and timed input windows between combo steps.
///
/// Combo chains:
///   Ground: L-L-L (3-hit), L-L-H (launcher), H (overhead slam), L-pause-L (delayed)
///   Air:    L-L-L (aerial chain), H (slam down), L-L-H (juggle finisher)
/// </summary>
public partial class ComboTracker : Node
{
    /// <summary>Time window to input next combo move before chain resets.</summary>
    [Export] public float ComboWindowDuration { get; set; } = 0.5f;

    /// <summary>Whether the entity is currently in the air (affects which combos are available).</summary>
    public bool IsAerial { get; set; }

    /// <summary>Current step index in the active combo chain.</summary>
    public int CurrentStep { get; private set; }

    /// <summary>Whether a combo chain is currently active.</summary>
    public bool IsInCombo => CurrentStep > 0;

    /// <summary>The active combo move data.</summary>
    public ComboData? CurrentMove { get; private set; }

    private float _comboTimer;
    private bool _inputBuffered;
    private ComboData.InputType _bufferedInput;

    // --- Combo definitions (built in _Ready) ---
    private readonly List<ComboData> _groundLightChain = new();
    private readonly List<ComboData> _groundHeavyChain = new();
    private readonly List<ComboData> _airLightChain = new();
    private readonly List<ComboData> _airHeavyChain = new();
    private ComboData? _launcherMove; // L-L-H ground finisher

    // --- Signals ---
    [Signal] public delegate void ComboStartedEventHandler(ComboData move);
    [Signal] public delegate void ComboAdvancedEventHandler(ComboData move, int step);
    [Signal] public delegate void ComboFinishedEventHandler(int totalHits);
    [Signal] public delegate void ComboDroppedEventHandler();

    public override void _Ready()
    {
        BuildDefaultCombos();
    }

    /// <summary>
    /// Build the default combo chains. These use placeholder animation names
    /// that will map to actual AnimationPlayer clips when art is ready.
    /// </summary>
    private void BuildDefaultCombos()
    {
        // --- Ground Light Chain: L → L → L (3-hit slash combo) ---
        _groundLightChain.Add(new ComboData
        {
            MoveName = "ground_light_1", Input = ComboData.InputType.Light,
            AnimationName = "attack_light_1", Damage = 8f, PostureDamage = 10f,
            Duration = 0.3f, KnockbackForce = 60f, ForwardMomentum = 20f,
            HitboxStartFraction = 0.15f, HitboxEndFraction = 0.55f
        });
        _groundLightChain.Add(new ComboData
        {
            MoveName = "ground_light_2", Input = ComboData.InputType.Light,
            AnimationName = "attack_light_2", Damage = 10f, PostureDamage = 12f,
            Duration = 0.35f, KnockbackForce = 80f, ForwardMomentum = 25f,
            HitboxStartFraction = 0.1f, HitboxEndFraction = 0.5f
        });
        _groundLightChain.Add(new ComboData
        {
            MoveName = "ground_light_3", Input = ComboData.InputType.Light,
            AnimationName = "attack_light_3", Damage = 15f, PostureDamage = 18f,
            Duration = 0.4f, KnockbackForce = 120f, ForwardMomentum = 35f,
            HitboxStartFraction = 0.15f, HitboxEndFraction = 0.6f
        });

        // --- Ground Heavy: H (overhead slam, high posture damage) ---
        _groundHeavyChain.Add(new ComboData
        {
            MoveName = "ground_heavy_1", Input = ComboData.InputType.Heavy,
            AnimationName = "attack_heavy_1", Damage = 20f, PostureDamage = 30f,
            Duration = 0.6f, KnockbackForce = 180f, ForwardMomentum = 10f,
            HitboxStartFraction = 0.35f, HitboxEndFraction = 0.65f,
            CanDodgeCancel = true
        });

        // --- Launcher: L-L-H (pops enemy into air for juggle) ---
        _launcherMove = new ComboData
        {
            MoveName = "launcher", Input = ComboData.InputType.Heavy,
            AnimationName = "attack_launcher", Damage = 12f, PostureDamage = 20f,
            Duration = 0.45f, KnockbackForce = 50f, ForwardMomentum = 15f,
            IsLauncher = true, LaunchForce = 300f,
            HitboxStartFraction = 0.25f, HitboxEndFraction = 0.55f
        };

        // --- Air Light Chain: L → L → L (aerial slashes) ---
        _airLightChain.Add(new ComboData
        {
            MoveName = "air_light_1", Input = ComboData.InputType.Light,
            AnimationName = "air_attack_light_1", Damage = 7f, PostureDamage = 8f,
            Duration = 0.25f, KnockbackForce = 40f, IsAerial = true,
            HitboxStartFraction = 0.1f, HitboxEndFraction = 0.5f
        });
        _airLightChain.Add(new ComboData
        {
            MoveName = "air_light_2", Input = ComboData.InputType.Light,
            AnimationName = "air_attack_light_2", Damage = 9f, PostureDamage = 10f,
            Duration = 0.28f, KnockbackForce = 50f, IsAerial = true,
            HitboxStartFraction = 0.1f, HitboxEndFraction = 0.5f
        });
        _airLightChain.Add(new ComboData
        {
            MoveName = "air_light_3", Input = ComboData.InputType.Light,
            AnimationName = "air_attack_light_3", Damage = 14f, PostureDamage = 15f,
            Duration = 0.35f, KnockbackForce = 100f, IsAerial = true,
            HitboxStartFraction = 0.15f, HitboxEndFraction = 0.55f
        });

        // --- Air Heavy: H (slam down) ---
        _airHeavyChain.Add(new ComboData
        {
            MoveName = "air_slam", Input = ComboData.InputType.Heavy,
            AnimationName = "air_attack_heavy_1", Damage = 18f, PostureDamage = 25f,
            Duration = 0.4f, KnockbackForce = 200f, IsAerial = true,
            HitboxStartFraction = 0.3f, HitboxEndFraction = 0.7f
        });
    }

    public override void _Process(double delta)
    {
        if (IsInCombo)
        {
            _comboTimer -= (float)delta;
            if (_comboTimer <= 0f)
            {
                int hits = CurrentStep;
                ResetCombo();
                EmitSignal(SignalName.ComboDropped);
            }
        }
    }

    /// <summary>
    /// Register an attack input. Returns the ComboData for the resulting move,
    /// or null if no valid move exists.
    /// </summary>
    public ComboData? RegisterInput(ComboData.InputType input)
    {
        ComboData? move;

        if (!IsInCombo)
        {
            // Starting a new combo
            move = GetFirstMove(input);
            if (move == null) return null;

            CurrentStep = 1;
            CurrentMove = move;
            _comboTimer = ComboWindowDuration;
            EmitSignal(SignalName.ComboStarted, move);
            return move;
        }
        else
        {
            // Continuing combo
            move = GetNextMove(input, CurrentStep);
            if (move == null)
            {
                // No valid continuation — end combo
                int hits = CurrentStep;
                ResetCombo();
                EmitSignal(SignalName.ComboFinished, hits);

                // Try starting a new combo with this input
                return RegisterInput(input);
            }

            CurrentStep++;
            CurrentMove = move;
            _comboTimer = ComboWindowDuration;
            EmitSignal(SignalName.ComboAdvanced, move, CurrentStep);
            return move;
        }
    }

    /// <summary>
    /// Refresh the combo window timer (call when attack animation starts playing).
    /// </summary>
    public void RefreshWindow()
    {
        _comboTimer = ComboWindowDuration;
    }

    /// <summary>
    /// Reset combo state completely.
    /// </summary>
    public void ResetCombo()
    {
        int hits = CurrentStep;
        CurrentStep = 0;
        CurrentMove = null;
        _comboTimer = 0f;
        _inputBuffered = false;

        if (hits > 0)
        {
            EmitSignal(SignalName.ComboFinished, hits);
        }
    }

    private ComboData? GetFirstMove(ComboData.InputType input)
    {
        if (IsAerial)
        {
            return input == ComboData.InputType.Light
                ? GetMoveFromChain(_airLightChain, 0)
                : GetMoveFromChain(_airHeavyChain, 0);
        }
        else
        {
            return input == ComboData.InputType.Light
                ? GetMoveFromChain(_groundLightChain, 0)
                : GetMoveFromChain(_groundHeavyChain, 0);
        }
    }

    private ComboData? GetNextMove(ComboData.InputType input, int currentStep)
    {
        if (IsAerial)
        {
            return input == ComboData.InputType.Light
                ? GetMoveFromChain(_airLightChain, currentStep)
                : GetMoveFromChain(_airHeavyChain, 0); // Heavy in air always = slam
        }
        else
        {
            if (input == ComboData.InputType.Heavy && currentStep >= 2 && _launcherMove != null)
            {
                // L-L-H → Launcher!
                return _launcherMove;
            }

            return input == ComboData.InputType.Light
                ? GetMoveFromChain(_groundLightChain, currentStep)
                : GetMoveFromChain(_groundHeavyChain, 0);
        }
    }

    private static ComboData? GetMoveFromChain(List<ComboData> chain, int index)
    {
        if (index < chain.Count) return chain[index];
        return null;
    }
}
