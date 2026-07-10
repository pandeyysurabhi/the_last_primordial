using Godot;
using TheLastPrimordial.Combat;

namespace TheLastPrimordial.Player.States;

/// <summary>
/// Attack state — handles combo chain execution.
/// DMC5-inspired: reads from ComboTracker for current move data,
/// manages hitbox activation timing, allows dodge-cancelling,
/// and supports ground + aerial variants.
///
/// Transitions to: Idle/Run (combo end, ground), Fall (combo end, air),
/// Dash (dodge-cancel), next Attack (combo continue).
/// </summary>
public partial class PlayerAttackState : PlayerState
{
    private ComboData? _currentMove;
    private float _attackTimer;
    private float _attackDuration;
    private bool _hitboxActive;
    private bool _comboQueued;
    private ComboData.InputType _queuedInput;

    public override void Enter()
    {
        // Determine input type from what was just pressed
        var inputType = Input.IsActionPressed("attack_heavy")
            ? ComboData.InputType.Heavy
            : ComboData.InputType.Light;

        // Tell combo tracker if we're aerial
        P.ComboTracker.IsAerial = !P.IsOnFloor();

        // Register the input and get the move data
        _currentMove = P.ComboTracker.RegisterInput(inputType);

        if (_currentMove == null)
        {
            // No valid move — exit
            ExitToDefault();
            return;
        }

        _attackDuration = _currentMove.Duration;
        _attackTimer = 0f;
        _hitboxActive = false;
        _comboQueued = false;

        // Set hitbox damage values
        P.Hitbox.Damage = _currentMove.Damage;
        P.Hitbox.PostureDamage = _currentMove.PostureDamage;
        P.Hitbox.KnockbackForce = _currentMove.KnockbackForce;
        P.Hitbox.IsLauncher = _currentMove.IsLauncher;
        P.Hitbox.LaunchForce = _currentMove.LaunchForce;

        // Apply forward momentum
        if (_currentMove.ForwardMomentum > 0f)
        {
            P.Velocity = new Vector2(
                P.FacingDirection * _currentMove.ForwardMomentum,
                P.Velocity.Y
            );
        }

        // Style meter points for attack variety
        P.StyleMeter.AddPoints(5f, _currentMove.MoveName);

        // Aerial bonus
        if (!P.IsOnFloor())
        {
            P.StyleMeter.AddAerialBonus(3f);
        }

        // Launcher bonus
        if (_currentMove.IsLauncher)
        {
            P.StyleMeter.AddLauncherBonus();
        }
    }

    public override void Exit()
    {
        // Ensure hitbox is deactivated
        P.Hitbox.Deactivate();
        _hitboxActive = false;
    }

    public override void PhysicsUpdate(double delta)
    {
        if (_currentMove == null)
        {
            ExitToDefault();
            return;
        }

        _attackTimer += (float)delta;
        float progress = _attackTimer / _attackDuration;

        // Apply gravity if aerial (slight suspension for air combos)
        if (!P.IsOnFloor())
        {
            P.ApplyGravity(delta, 0.3f); // Reduced gravity during air attacks
        }

        // Hitbox activation based on animation timing
        if (!_hitboxActive && progress >= _currentMove.HitboxStartFraction)
        {
            P.Hitbox.Activate();
            _hitboxActive = true;
        }
        if (_hitboxActive && progress >= _currentMove.HitboxEndFraction)
        {
            P.Hitbox.Deactivate();
            _hitboxActive = false;
        }

        // Slow down horizontal movement during attack
        P.Velocity = new Vector2(
            Mathf.MoveToward(P.Velocity.X, 0f, 200f * (float)delta),
            P.Velocity.Y
        );

        P.MoveAndSlide();

        // Attack finished
        if (_attackTimer >= _attackDuration)
        {
            if (_comboQueued)
            {
                // Continue combo with queued input
                P.ComboTracker.IsAerial = !P.IsOnFloor();
                _currentMove = P.ComboTracker.RegisterInput(_queuedInput);
                if (_currentMove != null)
                {
                    // Restart attack with new move
                    _attackTimer = 0f;
                    _hitboxActive = false;
                    _comboQueued = false;

                    P.Hitbox.Damage = _currentMove.Damage;
                    P.Hitbox.PostureDamage = _currentMove.PostureDamage;
                    P.Hitbox.KnockbackForce = _currentMove.KnockbackForce;
                    P.Hitbox.IsLauncher = _currentMove.IsLauncher;
                    P.Hitbox.LaunchForce = _currentMove.LaunchForce;

                    if (_currentMove.ForwardMomentum > 0f)
                    {
                        P.Velocity = new Vector2(
                            P.FacingDirection * _currentMove.ForwardMomentum,
                            P.Velocity.Y
                        );
                    }

                    P.StyleMeter.AddPoints(5f, _currentMove.MoveName);
                    _attackDuration = _currentMove.Duration;
                    return;
                }
            }

            // End of combo chain
            P.ComboTracker.ResetCombo();
            ExitToDefault();
        }
    }

    public override void HandleInput(InputEvent @event)
    {
        // Queue next combo input (input buffering for smooth chains)
        if (@event.IsActionPressed("attack_light"))
        {
            _comboQueued = true;
            _queuedInput = ComboData.InputType.Light;
            return;
        }

        if (@event.IsActionPressed("attack_heavy"))
        {
            _comboQueued = true;
            _queuedInput = ComboData.InputType.Heavy;
            return;
        }

        // Dodge-cancel (DMC5-style: cancel attack into dash)
        if (@event.IsActionPressed("dodge") && P.CanDash)
        {
            if (_currentMove?.CanDodgeCancel == true)
            {
                P.ComboTracker.ResetCombo();
                Machine?.TransitionTo("Dash");
                return;
            }
        }

        // Jump-cancel (allows aerial combo extension)
        if (@event.IsActionPressed("jump") && P.IsOnFloor())
        {
            P.ComboTracker.ResetCombo();
            Machine?.TransitionTo("Jump");
            return;
        }
    }

    private void ExitToDefault()
    {
        if (P.IsOnFloor())
        {
            float dir = P.GetInputDirection();
            Machine?.TransitionTo(Mathf.Abs(dir) > 0.1f ? "Run" : "Idle");
        }
        else
        {
            Machine?.TransitionTo("Fall");
        }
    }
}
