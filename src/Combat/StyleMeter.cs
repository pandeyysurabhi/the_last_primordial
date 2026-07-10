using Godot;

namespace TheLastPrimordial.Combat;

/// <summary>
/// DMC5-inspired style meter. Tracks combat performance and assigns a rank.
/// Ranks: D → C → B → A → S → SS → SSS
///
/// Design rules:
/// - Varied attacks give more points (same move spam = diminishing returns)
/// - Deflects give big points, perfect deflects give huge points
/// - Taking damage drops the meter significantly
/// - Points decay over time if player is passive (keeps pressure on)
/// - Aerial combos, launcher juggles give bonus multipliers
/// </summary>
public partial class StyleMeter : Node
{
    public enum StyleRank { D, C, B, A, S, SS, SSS }

    // Thresholds for each rank transition
    private static readonly float[] RankThresholds = { 0f, 20f, 50f, 100f, 180f, 300f, 500f };

    /// <summary>Points decay per second when not performing actions.</summary>
    [Export] public float DecayRate { get; set; } = 8f;

    /// <summary>How much damage drops the style points.</summary>
    [Export] public float DamagePenaltyMultiplier { get; set; } = 5f;

    /// <summary>Multiplier applied when the same move is used consecutively.</summary>
    [Export(PropertyHint.Range, "0,1,0.05")] public float RepeatPenalty { get; set; } = 0.3f;

    public float CurrentPoints { get; private set; }
    public StyleRank CurrentRank { get; private set; } = StyleRank.D;

    private string _lastMoveName = "";
    private int _sameMoveSeries;

    // --- Signals ---
    [Signal] public delegate void StyleRankChangedEventHandler(int newRank);
    [Signal] public delegate void StylePointsChangedEventHandler(float points, float nextThreshold);

    public override void _Process(double delta)
    {
        if (CurrentPoints > 0f)
        {
            CurrentPoints = Mathf.Max(0f, CurrentPoints - DecayRate * (float)delta);
            UpdateRank();
            EmitSignal(SignalName.StylePointsChanged, CurrentPoints, GetNextThreshold());
        }
    }

    /// <summary>
    /// Award points for performing a combat action.
    /// </summary>
    public void AddPoints(float basePoints, string moveName)
    {
        float multiplier = 1f;

        // Diminishing returns for repeating the same move (DMC5 core design)
        if (moveName == _lastMoveName)
        {
            _sameMoveSeries++;
            multiplier = Mathf.Pow(RepeatPenalty, _sameMoveSeries);
        }
        else
        {
            _sameMoveSeries = 0;
            multiplier = 1.2f; // Variety bonus
        }

        _lastMoveName = moveName;
        CurrentPoints += basePoints * multiplier;
        UpdateRank();
        EmitSignal(SignalName.StylePointsChanged, CurrentPoints, GetNextThreshold());
    }

    /// <summary>Award bonus points for perfect deflects (Sekiro-style).</summary>
    public void AddDeflectPoints(bool isPerfect)
    {
        float points = isPerfect ? 30f : 10f;
        AddPoints(points, isPerfect ? "perfect_deflect" : "block");
    }

    /// <summary>Award bonus points for aerial combos (DMC5-style).</summary>
    public void AddAerialBonus(float basePoints)
    {
        AddPoints(basePoints * 1.5f, "aerial_attack");
    }

    /// <summary>Award bonus for launcher juggle initiation.</summary>
    public void AddLauncherBonus()
    {
        AddPoints(20f, "launcher");
    }

    /// <summary>
    /// Penalize the style meter when player takes damage.
    /// </summary>
    public void OnDamageTaken(float damage)
    {
        CurrentPoints = Mathf.Max(0f, CurrentPoints - damage * DamagePenaltyMultiplier);
        _sameMoveSeries = 0;
        UpdateRank();
        EmitSignal(SignalName.StylePointsChanged, CurrentPoints, GetNextThreshold());
    }

    /// <summary>
    /// Reset style meter to D rank.
    /// </summary>
    public void Reset()
    {
        CurrentPoints = 0f;
        CurrentRank = StyleRank.D;
        _lastMoveName = "";
        _sameMoveSeries = 0;
        EmitSignal(SignalName.StyleRankChanged, (int)CurrentRank);
    }

    private void UpdateRank()
    {
        var newRank = StyleRank.D;
        for (int i = RankThresholds.Length - 1; i >= 0; i--)
        {
            if (CurrentPoints >= RankThresholds[i])
            {
                newRank = (StyleRank)i;
                break;
            }
        }

        if (newRank != CurrentRank)
        {
            CurrentRank = newRank;
            EmitSignal(SignalName.StyleRankChanged, (int)newRank);
        }
    }

    private float GetNextThreshold()
    {
        int nextIndex = (int)CurrentRank + 1;
        return nextIndex < RankThresholds.Length ? RankThresholds[nextIndex] : RankThresholds[^1];
    }

    /// <summary>Get display string for the current rank.</summary>
    public string GetRankString()
    {
        return CurrentRank switch
        {
            StyleRank.D => "D",
            StyleRank.C => "C",
            StyleRank.B => "B",
            StyleRank.A => "A",
            StyleRank.S => "S",
            StyleRank.SS => "SS",
            StyleRank.SSS => "SSS",
            _ => "D"
        };
    }
}
