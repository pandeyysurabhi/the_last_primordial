using Godot;

namespace TheLastPrimordial.Player.States;

/// <summary>
/// Dash state — quick burst of movement with invincibility frames.
/// Ignores gravity during dash. Spawns ghost afterimages.
/// Transitions to: Idle, Run, Fall (when dash ends).
/// </summary>
public partial class PlayerDashState : PlayerState
{
    private float _dashTimer;
    private float _afterimageTimer;
    private const float AfterimageInterval = 0.03f;

    public override void Enter()
    {
        _dashTimer = P.DashDuration;
        _afterimageTimer = 0f;

        // Set dash velocity in facing direction
        P.Velocity = new Vector2(P.FacingDirection * P.DashSpeed, 0f);

        // I-frames during dash
        P.HealthSystem.IsInvincible = true;

        // Start cooldown
        P.DashCooldownTimer = P.DashCooldown;

        P.EmitSignal(Player.SignalName.Dashed);
    }

    public override void Exit()
    {
        P.HealthSystem.IsInvincible = false;
    }

    public override void PhysicsUpdate(double delta)
    {
        // No gravity during dash — flat trajectory
        _dashTimer -= (float)delta;

        // Spawn afterimage ghosts
        _afterimageTimer -= (float)delta;
        if (_afterimageTimer <= 0f)
        {
            SpawnAfterimage();
            _afterimageTimer = AfterimageInterval;
        }

        if (_dashTimer <= 0f)
        {
            // Dash finished
            P.Velocity = new Vector2(P.Velocity.X * 0.3f, P.Velocity.Y); // Carry some momentum

            if (P.IsOnFloor())
            {
                float dir = P.GetInputDirection();
                Machine?.TransitionTo(Mathf.Abs(dir) > 0.1f ? "Run" : "Idle");
            }
            else
            {
                Machine?.TransitionTo("Fall");
            }
            return;
        }

        P.MoveAndSlide();
    }

    /// <summary>
    /// Spawn a fading ghost sprite at current position (dash afterimage effect).
    /// </summary>
    private void SpawnAfterimage()
    {
        var ghost = new Sprite2D();
        ghost.Texture = P.Sprite.Texture;
        ghost.RegionEnabled = P.Sprite.RegionEnabled;
        ghost.RegionRect = P.Sprite.RegionRect;
        ghost.Scale = P.Sprite.Scale;
        ghost.FlipH = P.Sprite.FlipH;
        ghost.GlobalPosition = P.Sprite.GlobalPosition;
        ghost.Modulate = new Color(0.5f, 0.3f, 0.8f, 0.6f); // Purple tint
        ghost.ZIndex = P.ZIndex - 1;

        // Add to scene tree (not as child of player, so it stays in place)
        P.GetParent().AddChild(ghost);

        // Fade out and delete
        var tween = ghost.CreateTween();
        tween.TweenProperty(ghost, "modulate:a", 0f, 0.3f);
        tween.TweenCallback(Callable.From(() => ghost.QueueFree()));
    }
}
