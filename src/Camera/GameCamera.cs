using Godot;

namespace TheLastPrimordial.Camera;

/// <summary>
/// Game camera with Hollow Knight-inspired smooth follow, dead zones,
/// look-ahead, room boundaries, and screen shake.
/// 
/// Not a child of the player — follows independently for more control
/// over transitions, cutscenes, and room-based limits.
/// </summary>
public partial class GameCamera : Camera2D
{
    /// <summary>Path to the node to follow (typically the player).</summary>
    [Export] public NodePath? Target { get; set; }

    private Node2D? _target;

    [ExportGroup("Follow")]
    /// <summary>How fast the camera follows the target (higher = snappier).</summary>
    [Export(PropertyHint.Range, "1,20,0.5")] public float FollowSpeed { get; set; } = 6f;

    /// <summary>Look-ahead distance in the player's facing direction.</summary>
    [Export] public float LookAheadDistance { get; set; } = 20f;

    /// <summary>How fast the look-ahead interpolates.</summary>
    [Export(PropertyHint.Range, "1,10,0.5")] public float LookAheadSpeed { get; set; } = 3f;

    [ExportGroup("Dead Zone")]
    /// <summary>Horizontal dead zone — camera won't move until target exits this region.</summary>
    [Export] public float DeadZoneX { get; set; } = 8f;

    /// <summary>Vertical dead zone.</summary>
    [Export] public float DeadZoneY { get; set; } = 6f;

    [ExportGroup("Room Boundaries")]
    /// <summary>Whether to use room-based camera limits.</summary>
    [Export] public bool UseRoomLimits { get; set; } = true;

    // Internal state
    private Vector2 _currentLookAhead;
    private Vector2 _targetLookAhead;
    private float _shakeIntensity;
    private float _shakeTimer;
    private readonly RandomNumberGenerator _rng = new();

    public override void _Ready()
    {
        // Resolve target from NodePath
        if (Target != null && !Target.IsEmpty)
        {
            _target = GetNodeOrNull<Node2D>(Target);
        }

        // Camera starts at target position (no initial lerp lag)
        if (_target != null)
        {
            GlobalPosition = _target.GlobalPosition;
        }

        // Enable smoothing via our custom implementation (not Godot's built-in)
        PositionSmoothingEnabled = false; // We handle smoothing ourselves
    }

    public override void _Process(double delta)
    {
        if (_target == null) return;

        Vector2 targetPos = _target.GlobalPosition;
        Vector2 currentPos = GlobalPosition;

        // --- Dead Zone ---
        // Only move the camera if the target has moved outside the dead zone
        Vector2 diff = targetPos - currentPos;
        Vector2 moveTarget = currentPos;

        if (Mathf.Abs(diff.X) > DeadZoneX)
        {
            float sign = Mathf.Sign(diff.X);
            moveTarget.X = targetPos.X - sign * DeadZoneX;
        }

        if (Mathf.Abs(diff.Y) > DeadZoneY)
        {
            float sign = Mathf.Sign(diff.Y);
            moveTarget.Y = targetPos.Y - sign * DeadZoneY;
        }

        // --- Look Ahead ---
        // Shift camera slightly in the direction the player is facing
        if (_target is Player.Player player)
        {
            _targetLookAhead = new Vector2(player.FacingDirection * LookAheadDistance, 0f);
        }
        _currentLookAhead = _currentLookAhead.Lerp(_targetLookAhead, LookAheadSpeed * (float)delta);

        // --- Smooth Follow ---
        Vector2 finalTarget = moveTarget + _currentLookAhead;
        GlobalPosition = GlobalPosition.Lerp(finalTarget, FollowSpeed * (float)delta);

        // --- Screen Shake ---
        if (_shakeTimer > 0f)
        {
            _shakeTimer -= (float)delta;
            Vector2 shakeOffset = new Vector2(
                _rng.RandfRange(-_shakeIntensity, _shakeIntensity),
                _rng.RandfRange(-_shakeIntensity, _shakeIntensity)
            );
            Offset = shakeOffset;

            // Decay intensity
            _shakeIntensity = Mathf.Lerp(_shakeIntensity, 0f, 5f * (float)delta);
        }
        else
        {
            Offset = Vector2.Zero;
        }

        // --- Clamp to Room Limits ---
        if (UseRoomLimits)
        {
            ClampToLimits();
        }
    }

    /// <summary>
    /// Trigger screen shake (call on impacts, explosions, boss attacks).
    /// </summary>
    public void Shake(float intensity, float duration)
    {
        _shakeIntensity = Mathf.Max(_shakeIntensity, intensity);
        _shakeTimer = duration;
    }

    /// <summary>
    /// Instantly snap camera to target (use for room transitions).
    /// </summary>
    public void SnapToTarget()
    {
        if (_target == null) return;
        GlobalPosition = _target.GlobalPosition + _currentLookAhead;
    }

    /// <summary>
    /// Set camera room boundaries. Called by CameraZone triggers.
    /// </summary>
    public void SetRoomLimits(int left, int top, int right, int bottom)
    {
        LimitLeft = left;
        LimitTop = top;
        LimitRight = right;
        LimitBottom = bottom;
    }

    /// <summary>
    /// Smoothly transition to new room limits over duration.
    /// </summary>
    public void TransitionToRoomLimits(int left, int top, int right, int bottom, float duration = 0.5f)
    {
        var tween = CreateTween();
        tween.SetParallel(true);
        tween.TweenProperty(this, "limit_left", left, duration);
        tween.TweenProperty(this, "limit_top", top, duration);
        tween.TweenProperty(this, "limit_right", right, duration);
        tween.TweenProperty(this, "limit_bottom", bottom, duration);
    }

    private void ClampToLimits()
    {
        Vector2 viewportSize = GetViewportRect().Size / Zoom;
        Vector2 halfView = viewportSize / 2f;

        Vector2 pos = GlobalPosition;
        pos.X = Mathf.Clamp(pos.X, LimitLeft + halfView.X, LimitRight - halfView.X);
        pos.Y = Mathf.Clamp(pos.Y, LimitTop + halfView.Y, LimitBottom - halfView.Y);
        GlobalPosition = pos;
    }
}
