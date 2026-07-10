using Godot;

namespace TheLastPrimordial.Player.States;

/// <summary>
/// Hurt state — player took damage. Knockback, brief stun, i-frames.
/// Transitions to: Idle (on floor after recovery), Fall (in air after recovery).
/// </summary>
public partial class PlayerHurtState : PlayerState
{
    [Export] public float HurtDuration { get; set; } = 0.3f;
    [Export] public float KnockbackForce { get; set; } = 120f;
    [Export] public float IFrameDuration { get; set; } = 0.8f;

    private float _hurtTimer;
    private float _iFrameTimer;
    private float _blinkTimer;

    public override void Enter()
    {
        _hurtTimer = HurtDuration;
        _iFrameTimer = IFrameDuration;
        _blinkTimer = 0f;

        // Enable i-frames
        P.HealthSystem.IsInvincible = true;

        // Reset combo
        P.ComboTracker.ResetCombo();

        // Simple knockback away from attacker
        P.Velocity = new Vector2(-P.FacingDirection * KnockbackForce, -80f);
    }

    public override void Exit()
    {
        // Ensure i-frames end and sprite is visible
        P.HealthSystem.IsInvincible = false;
        P.Sprite.Modulate = Colors.White;
    }

    public override void PhysicsUpdate(double delta)
    {
        P.ApplyGravity(delta);

        // Slow down knockback
        P.Velocity = new Vector2(
            Mathf.MoveToward(P.Velocity.X, 0f, 300f * (float)delta),
            P.Velocity.Y
        );

        _hurtTimer -= (float)delta;
        _iFrameTimer -= (float)delta;

        // Blink effect during i-frames
        if (_iFrameTimer > 0f)
        {
            _blinkTimer += (float)delta;
            P.Sprite.Modulate = ((int)(_blinkTimer * 20f) % 2 == 0)
                ? Colors.White
                : new Color(1f, 1f, 1f, 0.3f);
        }
        else
        {
            P.HealthSystem.IsInvincible = false;
            P.Sprite.Modulate = Colors.White;
        }

        P.MoveAndSlide();

        // Transition out of stun
        if (_hurtTimer <= 0f)
        {
            if (P.IsOnFloor())
            {
                Machine?.TransitionTo("Idle");
            }
            else
            {
                Machine?.TransitionTo("Fall");
            }
        }
    }
}
